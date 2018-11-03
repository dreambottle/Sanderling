using Bib3;
using Bib3.Synchronization;
using BotEngine.Interface;
using Optimat.EveOnline;
using Sanderling.Interface;
using Sanderling.Interface.MemoryStruct;
using System;
using System.Linq;
using System.Threading;

namespace Sanderling
{
	public class Sensor
	{
		class MemoryMeasurementInitReport
		{
			public MemoryMeasurementInitParam Param;

			public SictProcessMitIdAuswertWurzelSuuce ForDerived;

			public Int64[] SetRootAdr;
		}

		class MemoryMeasurementReport
		{
			public MemoryMeasurementInitReport DerivedFrom;

			public FromProcessMeasurement<GbsNodeInfo> Raw;

			public FromProcessMeasurement<IMemoryMeasurement> ViewInterface;
		}

		const int EveOnlineSensoGbsMengeAstAnzaalScrankeMax = 50000;
		const int EveOnlineSensoGbsAstListeChildAnzaalScrankeMax = 0x200;
		const int EveOnlineSensoGbsSuuceTiifeScrankeMax = 0x30;

		readonly object MeasurementLock = new object();

		FromProcessMeasurement<MemoryMeasurementInitReport> MemoryMeasurementInitLastReport;

		FromProcessMeasurement<MemoryMeasurementReport> MemoryMeasurementLastReport;

		FromProcessMeasurement<MemoryMeasurementInitParam> MemoryMeasurementInitLast =>
			MemoryMeasurementInitLastReport?.MapValue(report => report?.Param);

		FromProcessMeasurement<IMemoryMeasurement> MemoryMeasurementLast =>
			MemoryMeasurementLastReport?.Value?.ViewInterface;

		public FromInterfaceResponse ClientRequest(ToInterfaceRequest request)
		{
			if (null == request)
				return null;

			try
			{
				var MemoryMeasurementInitTake = null != request.MemoryMeasurementInitTake;

				if (MemoryMeasurementInitTake)
					this.MemoryMeasurementInitTake(request.MemoryMeasurementInitTake);

				if (request.MemoryMeasurementTake)
					MemoryMeasurementTake();

				return new FromInterfaceResponse
				{
					MemoryMeasurementInit =
					(MemoryMeasurementInitTake || request.MemoryMeasurementInitGetLast) ? MemoryMeasurementInitLast : null,

					MemoryMeasurement =
						request.MemoryMeasurementTake || request.MemoryMeasurementGetLast ? MemoryMeasurementLast : null,

					MemoryMeasurementInProgress = MeasurementLock.IsLocked(),
				};
			}
			catch
			{
				return new FromInterfaceResponse
				{
				};
			}
		}

		FromProcessMeasurement<MemoryMeasurementInitReport> MemoryMeasurementInitTake(MemoryMeasurementInitParam param)
		{
			if (null == param)
				return null;

			lock (MeasurementLock)
			{
				var StartTimeMilli = Environment.TickCount;

				var Measurement = new MemoryMeasurementInitReport
				{
					Param = param,
				};

				var ProcessId = param.ProcessId;

				var SearchRoot = new SictProcessMitIdAuswertWurzelSuuce(ProcessId);

				SearchRoot.Read();

				Measurement.ForDerived = SearchRoot;

				Measurement.SetRootAdr =
					SearchRoot?.GbsMengeWurzelObj
					    ?.Select(rootObj => rootObj?.HerkunftAdrese)
                        //?.WhereNotNullSelectValue()
                        ?.Where(x => (x.HasValue))?.Select(x => x.Value)
                        ?.ToArray();

				var EndTimeMilli = Environment.TickCount;

				var ProcessMeasurementReport = new FromProcessMeasurement<MemoryMeasurementInitReport>(
					Measurement,
					StartTimeMilli,
					EndTimeMilli,
					ProcessId);

				Thread.MemoryBarrier();

				return MemoryMeasurementInitLastReport = ProcessMeasurementReport;
			}
		}

		FromProcessMeasurement<MemoryMeasurementReport> MemoryMeasurementTake()
		{
			lock (MeasurementLock)
			{
				var StartTimeMilli = Environment.TickCount;

				var MeasurementInit = MemoryMeasurementInitLastReport;

				var DerivedFrom = MeasurementInit?.Value;

				var Measurement = new MemoryMeasurementReport
				{
					DerivedFrom = DerivedFrom,
				};

				var ProcessId = MeasurementInit?.ProcessId ?? 0;

				var MeasurementInitPortionForDerived = DerivedFrom?.ForDerived;

				Sanderling.Parse.Culture.InvokeInParseCulture(() =>
				{
					var ScnapscusAuswert =
						new SictProzesAuswertZuusctandScpezGbsBaum(
							new ProcessMemoryReader(ProcessId),
							MeasurementInitPortionForDerived,
							EveOnlineSensoGbsAstListeChildAnzaalScrankeMax,
							EveOnlineSensoGbsMengeAstAnzaalScrankeMax,
							EveOnlineSensoGbsSuuceTiifeScrankeMax);

					ScnapscusAuswert.BerecneScrit();

					var GbsBaumDirekt = ScnapscusAuswert.GbsWurzelHauptInfo;

					Measurement.Raw =
						new FromProcessMeasurement<GbsNodeInfo>(GbsBaumDirekt, StartTimeMilli, Environment.TickCount, ProcessId);

					var GbsBaumAuswert =
						Optimat.EveOnline.AuswertGbs.Extension.SensorikScnapscusKonstrukt(
							ScnapscusAuswert.GbsWurzelHauptInfo, int.MaxValue);

					Measurement.ViewInterface =
						new FromProcessMeasurement<IMemoryMeasurement>(GbsBaumAuswert, StartTimeMilli, Environment.TickCount, ProcessId);
				});

				var EndTimeMilli = Environment.TickCount;

				var ProcessMeasurementReport = new FromProcessMeasurement<MemoryMeasurementReport>(
					Measurement,
					StartTimeMilli,
					EndTimeMilli,
					ProcessId);

				Thread.MemoryBarrier();

				return MemoryMeasurementLastReport = ProcessMeasurementReport;
			}
		}
	}
}
