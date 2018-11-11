using NUnit.Framework;
using System.Diagnostics;
using System.Linq;
using Bib3.Geometrik;
using System;

namespace Sanderling.Test.Exe
{
	public class Other
	{
		class EnumerateReferencedObjectTransitiveTest
		{
			public object Ref;

			public object Ref1;
		}

		[Test]
		public void TestBib3RectTransformations()
		{
			var rect = RectInt.FromCenterAndSize(new Vektor2DInt(6, 11), new Vektor2DInt(5, 7));
			var rect0 = new RectInt(4, 5, 12, 17);

			Console.WriteLine($"Console RECT0={rect0}; Center={rect0.Center()}");
			Console.WriteLine($"Console RECT={rect}; Center={rect.Center()}");

			var diffRects = rect.Diferenz(new RectInt(4, 7, 10, 12)).ToArray();
			foreach (var r in diffRects)
			{
				Console.WriteLine($"DiffRect={r}");
			}

			var withSizePivotAtCenter = rect.WithSizePivotAtCenter(new Vektor2DInt(3, 3));
			Console.WriteLine($"withSizePivotAtCenter(3,3)={withSizePivotAtCenter}");

			var withSizeExpandedPivotAtCenter = rect.WithSizeExpandedPivotAtCenter(new Vektor2DInt(-3, 3));
			Console.WriteLine($"withSizeExpandedPivotAtCenter(-3, 3)={withSizeExpandedPivotAtCenter}");

			var withSizeBoundedMaxPivotAtCenter = rect.WithSizeBoundedMaxPivotAtCenter(new Vektor2DInt(7, 7));
			Console.WriteLine($"withSizeBoundedMaxPivotAtCenter(7,7)={withSizeBoundedMaxPivotAtCenter}");

		}

		// todo split up and add assertions (make this an actual test)
		[Test]
		public void TestCommontRectTransformations()
		{
			var rect = Commons.Geometry.RectInt.FromCenterAndSize(
				new Commons.Geometry.Vector2i(6, 11), new Commons.Geometry.Vector2i(5, 7));
			var rect0 = Commons.Geometry.RectInt.FromMinAndMaxPoints(4, 5, 12, 17);

			Console.WriteLine($"Console RECT0={rect0}; Center={rect0.Center()}");
			Console.WriteLine($"Console RECT={rect}; Center={rect.Center()}");

			var diffRects = rect.DiffRectangles(Commons.Geometry.RectInt.FromMinAndMaxPoints(4, 7, 10, 12)).ToArray();
			foreach (var r in diffRects)
			{
				Console.WriteLine($"DiffRect={r}");
			}

			var withSizeSameCenter = rect.WithSizePivotAtCenter(new Commons.Geometry.Vector2i(2, 2));
			Console.WriteLine($"withSizeSameCenter(2,2)={withSizeSameCenter}");

			var withBoundedSizePivotAtCenter = rect.WithSizeLimitPivotAtCenter(new Commons.Geometry.Vector2i(7, 7));
			Console.WriteLine($"withBoundedSizePivotAtCenter(7,7)={withBoundedSizePivotAtCenter}");

			var resizedPivotAtCenter = rect.ResizedPivotAtCenter(new Commons.Geometry.Vector2i(-3, 3));
			Console.WriteLine($"resizedPivotAtCenter(-3, 3)={resizedPivotAtCenter}");

		}

		[Test]
		public void EnumerateReferencedTransitive_NoCycle()
		{
			var Obj0 = new EnumerateReferencedObjectTransitiveTest();

			var Obj1 = new EnumerateReferencedObjectTransitiveTest { Ref = Obj0, Ref1 = Obj0 };

			Obj0.Ref = Obj1;
			Obj0.Ref1 = Obj0;

			var Enumerated = Interface.MemoryStruct.Extension.EnumerateReferencedTransitive(Obj0)?.ToArray();

			Assert.AreEqual(2, Enumerated.Length, "Enumerated.Count");

			Assert.IsTrue(Enumerated.Contains(Obj0));
			Assert.IsTrue(Enumerated.Contains(Obj1));
		}
	}
}
