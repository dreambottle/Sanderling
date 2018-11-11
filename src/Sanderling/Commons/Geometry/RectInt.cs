using System;
using System.Collections.Generic;
using System.Text;

namespace Commons.Geometry
{
	public struct RectInt
	{
		// maybe store it as center + size instead ???
		public long X0;
		public long Y0;
		public long X1;
		public long Y1;

		public static RectInt Empty { get; } = new RectInt(0, 0, 0, 0);

		public RectInt(RectInt other) : this(other.X0, other.Y0, other.X1, other.Y1) { }

		private RectInt(long x0, long y0, long x1, long y1)
		{
			X0 = x0;
			X1 = x1;
			Y0 = y0;
			Y1 = y1;
		}

		public static RectInt FromCenterAndSize(Vector2i center, Vector2i size)
		{
			var halfSize = size / 2;
			// may be larger than halfSize by 1 on either axis, if size is not a multiple of 2.
			var otherHalfSize = size - halfSize;
			var minPoint = center - halfSize;
			var maxPoint = center + otherHalfSize; // maxPoint will go larger
			return FromMinAndMaxPoints(minPoint, maxPoint);
		}

		public static RectInt FromCenterAndHalfSize(Vector2i center, Vector2i halfSize)
		{
			return new RectInt(center.X - halfSize.X, center.Y - halfSize.Y, center.X + halfSize.X, center.Y + halfSize.Y);
		}

		public static RectInt FromMinAndMaxPoints(long minPointX, long minPointY, long maxPointX, long maxPointY)
		{
			return new RectInt
			{
				X0 = minPointX,
				Y0 = minPointY,
				X1 = maxPointX,
				Y1 = maxPointY,
			};
		}

		public static RectInt FromMinAndMaxPoints(Vector2i minPoint, Vector2i maxPoint)
		{
			return new RectInt(minPoint.X, minPoint.Y, maxPoint.X, maxPoint.Y);
		}


		public override bool Equals(object other)
		{
			if (!(other is RectInt))
			{
				return false;
			}
			else
			{
				return this == (RectInt)other;
			}
		}
		public override int GetHashCode()
		{
			return (X0.GetHashCode() << 6) ^ Y0.GetHashCode() << 4 ^
				(X1.GetHashCode() << 2) ^ Y1.GetHashCode();
		}

		public override string ToString()
		{
			return $"{nameof(RectInt)}(MinX={X0}, MinY={Y0}, MaxX={X1}, MaxY={Y1})";
		}

		public static RectInt operator +(RectInt rect, Vector2i vec2i)
		{
			return new RectInt(rect.X0 + vec2i.X, rect.Y0 + vec2i.Y,
				rect.X1 + vec2i.X, rect.Y1 + vec2i.Y);
		}

		public static RectInt operator -(RectInt rect, Vector2i vec2i)
		{
			return new RectInt(rect.X0 - vec2i.X, rect.Y0 - vec2i.Y,
				rect.X1 - vec2i.X, rect.Y1 - vec2i.Y);
		}

		public static RectInt operator *(RectInt rect, long factor)
		{
			return new RectInt(rect.X0 * factor, rect.Y0 * factor,
				rect.X1 * factor, rect.Y1 * factor);
		}

		public static bool operator ==(RectInt rect0, RectInt rect1)
		{
			return ReferenceEquals(rect0, rect1) ||
				(rect0.X0 == rect1.X0 &&
				rect0.Y0 == rect1.Y0 &&
				rect0.X1 == rect1.X1 &&
				rect0.Y1 == rect1.Y1);
		}

		public static bool operator !=(RectInt rect0, RectInt rect1)
		{
			return !(rect0 == rect1);
		}


		public Vector2i MinPoint() => new Vector2i(Math.Min(X0, X1), Math.Min(Y0, Y1));

		public Vector2i MaxPoint() => new Vector2i(Math.Max(X0, X1), Math.Max(Y0, Y1));

		public Vector2i Size() => new Vector2i(X1 - X0, Y1 - Y0);

		public Vector2i Center() => new Vector2i(
				(X0 + X1) / 2,
				(Y0 + Y1) / 2);

		public Vector2f CenterFloat() => new Vector2f(
				(X0 + X1) / 2f,
				(Y0 + Y1) / 2f);

		public long Area() => Math.Abs((X1 - X0) * (Y1 - Y0));

		// Transformations & other operations

		public RectInt? Intersection(RectInt? other)
		{
			if (!other.HasValue) return null;

			var minPoint0 = this.MinPoint();
			var maxPoint0 = this.MaxPoint();
			var minPoint1 = other.Value.MinPoint();
			var maxPoint1 = other.Value.MaxPoint();

			var resultMinPointX = Math.Max(minPoint0.X, minPoint1.X);
			var resultMinPointY = Math.Max(minPoint0.Y, minPoint1.Y);
			var resultMaxPointX = Math.Min(maxPoint0.X, maxPoint1.X);
			var resultMaxPointY = Math.Min(maxPoint0.Y, maxPoint1.Y);

			if (resultMaxPointX < resultMinPointX || resultMaxPointY < resultMinPointY)
			{
				return null;
			}

			return new RectInt(resultMinPointX, resultMinPointY, resultMaxPointX, resultMaxPointY);
		}
		
		/// <summary>
		/// Equivalent to WithSizeExpandedPivotAtCenter
		/// </summary>
		/// <param name="difference"></param>
		/// <returns>A new RectInt</returns>
		public RectInt ResizedPivotAtCenter(Vector2i difference)
		{
			var newSize = Size() + difference;
			if (newSize.X <= 0 || newSize.Y <= 0)
			{
				newSize = Vector2i.Zero;
			}
			return RectInt.FromCenterAndSize(Center(), newSize);
		}

		public RectInt WithSizeLimitPivotAtCenter(Vector2i sizeLimit)
		{
			var oldSize = Size();
			var newSize = new Vector2i(
				Math.Min(oldSize.X, sizeLimit.X),
				Math.Min(oldSize.Y, sizeLimit.Y));
			return newSize == oldSize
				? this
				: RectInt.FromCenterAndSize(Center(), newSize);
		}

		public RectInt WithSizePivotAtCenter(Vector2i size)
		{
			return RectInt.FromCenterAndSize(Center(), size);
		}

		/// <summary>
		/// Iterates over rectangles, which are left if you subtract an antersecting rectangle from this one.
		/// If there is no intersection, yields this.
		/// </summary>
		/// <param name="other"></param>
		/// <returns>A new RectInt</returns>
		public IEnumerable<RectInt> DiffRectangles(RectInt other)
		{
			var intersection = Intersection(other);
			if (!intersection.HasValue)
			{
				yield return this;
			}
			else
			{
				var minPointInt = intersection.Value.MinPoint();
				var maxPointInt = intersection.Value.MaxPoint();

				var minPointThis = MinPoint();
				var maxPointThis = MaxPoint();

				if (minPointThis.Y < minPointInt.Y)
				{
					yield return new RectInt(
						minPointThis.X,
						minPointThis.Y,
						maxPointThis.X,
						minPointInt.Y
						);
				}

				if (minPointThis.X < minPointInt.X)
				{
					yield return new RectInt(
						minPointThis.X,
						minPointInt.Y,
						minPointInt.X,
						maxPointInt.Y
						);
				}

				if (maxPointThis.X > maxPointInt.X)
				{
					yield return new RectInt(
						maxPointInt.X,
						minPointInt.Y,
						maxPointThis.X,
						maxPointInt.Y
						);
				}

				if (maxPointThis.Y > maxPointInt.Y)
				{
					yield return new RectInt(
						minPointThis.X,
						maxPointInt.Y,
						maxPointThis.X,
						maxPointThis.Y
						);
				}
			}
		}

	}
}
