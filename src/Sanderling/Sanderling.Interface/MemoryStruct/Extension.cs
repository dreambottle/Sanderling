using System;
using System.Linq;
using System.Collections.Generic;
using BotEngine;
using Bib3;
using Commons.Geometry;

namespace Sanderling.Interface.MemoryStruct
{
	static public class Extension
	{
		static public IObjectIdInMemory AsObjectIdInMemory(this Int64 id)
		{
			return new ObjectIdInMemory(new ObjectIdInt64(id));
		}

		static public T Largest<T>(this IEnumerable<T> source)
			where T : class, IUIElement =>
			source.OrderByDescending(item => item?.Region.Area() ?? -1)
			?.FirstOrDefault();

		static public IEnumerable<object> EnumerateReferencedTransitive(
			this object parent) =>
			Bib3.RefNezDiferenz.Extension.EnumMengeRefAusNezAusWurzel(parent, FromInterfaceResponse.UITreeComponentTypeHandlePolicyCache);

		static public IEnumerable<IUIElement> EnumerateReferencedUIElementTransitive(
			this object parent) =>
			EnumerateReferencedTransitive(parent)
			?.OfType<IUIElement>();

		static public T CopyByPolicyMemoryMeasurement<T>(this T toBeCopied)
			where T : class =>
			Bib3.RefBaumKopii.RefBaumKopiiStatic.ObjektKopiiErsctele(toBeCopied, new Bib3.RefBaumKopii.Param(null, FromInterfaceResponse.SerialisPolicyCache));

		static public IUIElement WithRegion(this IUIElement @base, RectInt? region) =>
			null == @base ? null : new UIElement(@base) { Region = region ?? RectInt.Empty };

		static public IUIElement WithRegionSizePivotAtCenter(this IUIElement @base, Vector2i regionSize) =>
			@base?.WithRegion(@base.Region.WithSizePivotAtCenter(regionSize));

		static public IUIElement WithRegionSizeBoundedMaxPivotAtCenter(this IUIElement @base, Vector2i regionSizeMax) =>
			@base?.WithRegion(@base.Region.WithSizeLimitPivotAtCenter(regionSizeMax));

		static public Vector2i? RegionCenter(
			this IUIElement uiElement) =>
			(uiElement?.Region)?.Center();

		static public Vector2i? RegionSize(
			this IUIElement uiElement) =>
			(uiElement?.Region)?.Size();

		static public Vector2i? RegionCornerLeftTop(
			this IUIElement uiElement) => uiElement?.Region.MinPoint();

		static public Vector2i? RegionCornerRightBottom(
			this IUIElement uiElement) => uiElement?.Region.MaxPoint();

		static public IEnumerable<ITreeViewEntry> EnumerateChildNodeTransitive(
			this ITreeViewEntry treeViewEntry) =>
			treeViewEntry?.EnumerateNodeFromTreeBFirst(node => node.Child);

		static public IEnumerable<T> OrderByCenterDistanceToPoint<T>(
			this IEnumerable<T> sequence,
			Vector2i point)
			where T : IUIElement =>
			sequence?.OrderBy(element => (point - element?.RegionCenter())?.SqrMagnitude ?? Int64.MaxValue);

		static public IEnumerable<T> OrderByCenterVerticalDown<T>(
			this IEnumerable<T> source)
			where T : IUIElement =>
			source?.OrderBy(element => element?.RegionCenter()?.Y ?? int.MaxValue);

		static public IEnumerable<T> OrderByNearestPointOnLine<T>(
			this IEnumerable<T> sequence,
			Vector2i lineVector,
			Func<T, Vector2i?> getPointRepresentingElement)
		{
			var LineVectorMagnitude = (long)lineVector.Magnitude;

			if (null == getPointRepresentingElement || LineVectorMagnitude < 1)
				return sequence;

			var LineVectorNormalizedMilli = (lineVector * 1000) / LineVectorMagnitude;

			return
				sequence?.Select(element =>
				{
					Int64? LocationOnLine = null;

					var PointRepresentingElement = getPointRepresentingElement(element);

					if (PointRepresentingElement.HasValue)
					{
						LocationOnLine = PointRepresentingElement.Value.X * LineVectorNormalizedMilli.X
							+ PointRepresentingElement.Value.Y * LineVectorNormalizedMilli.Y;
					}

					return new { Element = element, LocationOnLine };
				})
				?.OrderBy(elementAndLocation => elementAndLocation.LocationOnLine)
				?.Select(elementAndLocation => (T)elementAndLocation.Element);
		}
	}
}
