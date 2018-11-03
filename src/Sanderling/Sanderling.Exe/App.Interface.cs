using Bib3;
using Bib3.Synchronization;
using BotEngine.Interface;
using Sanderling.Interface;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sanderling.Exe
{
    /// <summary>
    /// This Type must reside in an Assembly that can be resolved by the default assembly resolver.
    /// </summary>
    public class InterfaceAppDomainSetup
    {
        static InterfaceAppDomainSetup()
        {
            BotEngine.Interface.InterfaceAppDomainSetup.Setup();
        }
    }

    partial class App
    {
        readonly Sensor sensor = new Sensor();

        readonly object measurementInvalidationLock = new object();
        public Int64? fromScriptMeasurementInvalidationTime = null;
        public bool isMeasurementInvalidated = false;

        InterfaceAppDomainSetup TriggerSetup = new InterfaceAppDomainSetup();

        public UI.InterfaceToEve InterfaceToEveControl => Window?.Main?.Interface;

        public int? EveOnlineClientProcessId =>
            InterfaceToEveControl?.ProcessChoice?.ChoosenProcessId;

        FromProcessMeasurement<MemoryMeasurementEvaluation> MemoryMeasurementLast;

        //readonly Bib3.RateLimit.IRateLimitStateInt MemoryMeasurementRequestRateLimit = new Bib3.RateLimit.RateLimitStateIntSingle();

        int MeasurementConsideredYoungAge = 300;
        int MotionInvalidateMeasurementDelay = 100; //400;
        int StandardMeasurementThrottleInterval = 3000; // 4000

        Int64? FromMotionExecutionMemoryMeasurementTimeMin =>
            MotorLock.BranchOnTryEnter(() =>
            {
                var motionLastTime = MotionLastTime;

                if (!motionLastTime.HasValue)
                    return null;

                return motionLastTime.Value + MotionInvalidateMeasurementDelay;

            },
            () => (Int64?)Int64.MaxValue);

        Int64? MeasurementRequiredDelayTime => new[]
            {
                FromMotionExecutionMemoryMeasurementTimeMin,
                fromScriptMeasurementInvalidationTime,
            }.Max();

        Int64? ThrottledTimeForMeasurement
        {
            get
            {
                lock (measurementInvalidationLock)
                {
                    if (isMeasurementInvalidated)
                    {
                        return this.MeasurementRequiredDelayTime;
                    }
                }
                return 0;
                //return (MemoryMeasurementLast?.Begin + StandardMeasurementThrottleInterval) ?? 0;
            }
        }

        // deprecate
        //FromProcessMeasurement<MemoryMeasurementEvaluation> MemoryMeasurementIfRecentEnough
        //{
        //    get
        //    {
        //        var MemoryMeasurementLast = this.MemoryMeasurementLast;
        //        var MeasurementRecentEnoughTime = this.MeasurementRequiredDelayTime;

        //        if (MemoryMeasurementLast?.Begin < MeasurementRecentEnoughTime)
        //            return null;

        //        return MemoryMeasurementLast;
        //    }
        //}

        FromProcessMeasurement<MemoryMeasurementEvaluation> FromScriptRequestMemoryMeasurementEvaluation()
        {
            lock (measurementInvalidationLock)
            {
                if (!isMeasurementInvalidated)
                {
                    // Ideally, should get rid of this code.
                    if (GetTimeStopwatch() - MemoryMeasurementLast?.End < MeasurementConsideredYoungAge)
                    {
                        return MemoryMeasurementLast;
                    }
                    else
                    {
                        // Invalidate with a standard interval from last measurement
                        isMeasurementInvalidated = true;
                        fromScriptMeasurementInvalidationTime = MemoryMeasurementLast?.Begin + StandardMeasurementThrottleInterval;
                    }
                }
            }

            for (;;)
            {
                lock (measurementInvalidationLock)
                {
                    if (!isMeasurementInvalidated)
                    {
                        return MemoryMeasurementLast;
                    }
                }
                Thread.Sleep(75);
            }

            //var BeginTime = GetTimeStopwatch();

            //while (true)
            //{
            //    var MemoryMeasurementIfRecentEnough = this.MemoryMeasurementIfRecentEnough;

            //    if (null != MemoryMeasurementIfRecentEnough)
            //        return MemoryMeasurementIfRecentEnough;

            //    var RequestAge = GetTimeStopwatch() - BeginTime;

            //    if (Sanderling.Script.Impl.HostToScript.FromScriptRequestMemoryMeasurementDelayMax < RequestAge)
            //        return null;    //	Timeout

            //    Thread.Sleep(44);
            //}
        }

        void FromScriptInvalidateMeasurement(int delayToMeasurementMilli)
        {
            // Not sure if the call will come from a different thread. Locking just in case.
            lock (measurementInvalidationLock)
            {
                fromScriptMeasurementInvalidationTime = Math.Max(
                    fromScriptMeasurementInvalidationTime ?? int.MinValue,
                    GetTimeStopwatch() + Math.Min(delayToMeasurementMilli, 10000)
                    );
                isMeasurementInvalidated = true;
            }
        }

        public void TakeMeasurementNow()
        {
            if (EveOnlineClientProcessId.HasValue)
            {
                var EveOnlineClientProcessId = this.EveOnlineClientProcessId;
                Task.Run(() => MeasurementMemoryTake(EveOnlineClientProcessId.Value, null /*Environment.TickCount*/));
            }
        }

        public void TakeMeasurementThrottled()
        {
            var throttledTimeForMeasurement = this.ThrottledTimeForMeasurement ?? 0;
            if (EveOnlineClientProcessId.HasValue
                && throttledTimeForMeasurement <= GetTimeStopwatch()
                )
            {
                var eveOnlineClientProcessId = this.EveOnlineClientProcessId;
                //MeasurementMemoryTake(eveOnlineClientProcessId.Value, throttledTimeForMeasurement);
                Task.Run(() => MeasurementMemoryTake(eveOnlineClientProcessId.Value, throttledTimeForMeasurement));
            }
        }

        void MeasurementMemoryTake(int processId, Int64? measurementBeginTimeMinMilli)
        {
            var MeasurementRaw = measurementBeginTimeMinMilli.HasValue
                ? sensor.MeasurementTake(processId, measurementBeginTimeMinMilli.Value)
                : sensor.MeasurementTakeNewRequest(processId)?.MemoryMeasurement;

            if (null == MeasurementRaw)
                return;

            lock (measurementInvalidationLock)
            {
                MemoryMeasurementLast = MeasurementRaw?.MapValue(value => new Interface.MemoryMeasurementEvaluation(
                    MeasurementRaw,
                    MemoryMeasurementLast?.Value?.MemoryMeasurementAccumulation as Accumulator.MemoryMeasurementAccumulator));

                isMeasurementInvalidated = false;
            }
        }
    }
}
