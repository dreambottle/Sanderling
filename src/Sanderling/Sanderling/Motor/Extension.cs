using BotEngine.Common;
using BotEngine.Motor;
using System;
using System.Collections.Generic;
using System.Linq;
using Sanderling.Interface.MemoryStruct;
using Bib3;
using BotEngine.Windows;
using Sanderling.Interface.Compat;

namespace Sanderling.Motor
{
	static public class Extension
	{
		public const int MotionMouseWaypointSafetyMarginMin = 2;

		public const int MotionMouseWaypointSafetyMarginAdditional = 0;

		static public IEnumerable<IUIElement> EnumerateSetElementExcludedFromOcclusion(this IMemoryMeasurement memoryMeasurement) => new[]
			{
				memoryMeasurement?.ModuleButtonTooltip,
			}.WhereNotDefault();

		static public IEnumerable<Motion> AsSequenceMotion(
			this MotionParam motion,
			IMemoryMeasurement memoryMeasurement)
		{
			if (null == motion)
			{
				yield break;
			}

			if (motion?.WindowToForeground ?? false)
				yield return new Motion(null, windowToForeground: true);

			var SetElementExcludedFromOcclusion = memoryMeasurement?.EnumerateSetElementExcludedFromOcclusion()?.ToArray();

			var Random = new Random((int)Environment.TickCount);

			var MouseListWaypoint = motion?.MouseListWaypoint;

			var mouseButtonDownMotion = new Motion(null, motion?.MouseButton);
			var mouseButtonUpMotion = new Motion(null, null, motion?.MouseButton);

			for (int WaypointIndex = 0; WaypointIndex < (MouseListWaypoint?.Length ?? 0); WaypointIndex++)
			{
				var MouseWaypoint = MouseListWaypoint[WaypointIndex];

				var WaypointUIElement = MouseWaypoint?.UIElement;

				WaypointUIElement = (WaypointUIElement as Accumulation.IRepresentingMemoryObject)?.RepresentedMemoryObject as UIElement ?? WaypointUIElement;

				var WaypointRegionReplacement = MouseWaypoint.RegionReplacement;

				var WaypointUIElementCurrent =
					WaypointUIElement.GetInstanceWithIdFromCLRGraph(memoryMeasurement, Interface.FromInterfaceResponse.SerialisPolicyCache);

				if (null == WaypointUIElementCurrent)
				{
					throw new ApplicationException("mouse waypoint not anymore contained in UITree");
				}

				var WaypointUIElementRegion = WaypointUIElementCurrent.RegionInteraction?.Region;

				if (!WaypointUIElementRegion.HasValue)
				{
					throw new ArgumentException("Waypoint UIElement has no Region to interact with");
				}

				if (WaypointRegionReplacement.HasValue)
				{
					WaypointUIElementRegion = WaypointRegionReplacement.Value + WaypointUIElementRegion.Value.Center();
				}

				WaypointUIElementCurrent = WaypointUIElementCurrent.WithRegion(WaypointUIElementRegion.Value);

				var WaypointRegionPortionVisible =
					WaypointUIElementCurrent.GetOccludedUIElementRemainingRegion(
						memoryMeasurement,
						c => SetElementExcludedFromOcclusion?.Contains(c) ?? false)
					//	remaining region is contracted to provide a safety margin.
					?.Select(portionVisible => portionVisible.ResizedPivotAtCenter(-MotionMouseWaypointSafetyMarginMin * 2))
					?.Where(portionVisible => portionVisible.Area() > 0)
					?.ToArray();

				var WaypointRegionPortionVisibleLargestPatch =
					WaypointRegionPortionVisible
					?.OrderByDescending(patch => Math.Min(patch.X1 - patch.X0, patch.Y1 - patch.Y0))
					?.FirstOrNull();

				//if (!(0 < WaypointRegionPortionVisibleLargestPatch?.Side0Length() &&
				//	0 < WaypointRegionPortionVisibleLargestPatch?.Side1Length()))
				if (!WaypointRegionPortionVisibleLargestPatch.HasValue)
				{
					throw new ApplicationException("mouse waypoint region remaining after occlusion is too small");
				}

				var Point =
					WaypointRegionPortionVisibleLargestPatch.Value
					.ResizedPivotAtCenter(-MotionMouseWaypointSafetyMarginAdditional * 2)
					.RandomPointInRectangle(Random);

				yield return new Motion(Point.AsVektor2dInt());

				if (0 == WaypointIndex)
				{
					yield return mouseButtonDownMotion;

					for (int repetitionIndex = 0; repetitionIndex < motion?.MouseButtonRepetitionCount; repetitionIndex++)
					{
						yield return mouseButtonUpMotion;
						yield return mouseButtonDownMotion;
					}
				}
			}

			yield return mouseButtonUpMotion;

			var MotionKeyDown = motion?.KeyDown;
			var MotionKeyUp = motion?.KeyUp;

			if (null != MotionKeyDown)
			{
				yield return new Motion(null, keyDown: MotionKeyDown);
			}

			if (null != MotionKeyUp)
			{
				yield return new Motion(null, keyUp: MotionKeyUp);
			}

			var MotionTextEntry = motion?.TextEntry;

			if (0 < MotionTextEntry?.Length)
				yield return new Motion(null, textEntry: MotionTextEntry);
		}

		static public Bib3.Geometrik.Vektor2DInt? ClientToScreen(this IntPtr hWnd, Bib3.Geometrik.Vektor2DInt locationInClient)
		{
			var structWinApi = locationInClient.AsWindowsPoint();

			if (!BotEngine.WinApi.User32.ClientToScreen(hWnd, ref structWinApi))
				return null;

			return structWinApi.AsVektor2DInt();
		}
	}
}
