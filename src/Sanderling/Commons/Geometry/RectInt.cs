using System;
using System.Collections.Generic;
using System.Text;

namespace Commons.Geometry
{
	public struct RectInt
	{
		private static readonly RectInt EMPTY_ZERO_RECT = new RectInt(0, 0, 0, 0);

		public long MinX;
		public long MinY;
		public long MaxX;
		public long MaxY;

		public static RectInt Empty => EMPTY_ZERO_RECT;

		public RectInt(RectInt other)
		{
			MinX = other.MinX;
			MinY = other.MinY;
			MaxX = other.MaxX;
			MaxY = other.MaxY;
		}

		public RectInt(long minX, long minY, long maxX, long maxY)
		{
			MinX = minX;
			MinY = minY;
			MaxX = maxX;
			MaxY = maxY;
		}

		public static RectInt FromCenterAndSize(Vector2i center, Vector2i size)
		{
			return new RectInt(center.X - size.X, center.Y - size.Y, center.X + size.X, center.Y + size.Y);
		}

		public static RectInt FromMinPointAndMaxPoint(Vector2i minPoint, Vector2i maxPoint)
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
		//public override int GetHashCode()
		//{
		//	return (MinX.GetHashCode() << 6) ^ MinY.GetHashCode() << 4 ^
		//		MaxX.;
		//}
		//public override string ToString()
		//{
		//	return
		//}

		public static RectInt operator +(RectInt rect, Vector2i vec2i)
		{
			return new RectInt(rect.MinX + vec2i.X, rect.MinY + vec2i.Y,
				rect.MaxX + vec2i.X, rect.MaxY + vec2i.Y);
		}

		public static RectInt operator -(RectInt rect, Vector2i vec2i)
		{
			return new RectInt(rect.MinX - vec2i.X, rect.MinY - vec2i.Y,
				rect.MaxX - vec2i.X, rect.MaxY - vec2i.Y);
		}

		public static RectInt operator *(RectInt rect, long factor)
		{
			return new RectInt(rect.MinX * factor, rect.MinY * factor,
				rect.MaxX * factor, rect.MaxY * factor);
		}

		public static bool operator ==(RectInt rect0, RectInt rect1)
		{
			return ReferenceEquals(rect0, rect1) ||
				(rect0.MinX == rect1.MinX &&
				rect0.MinY == rect1.MinY &&
				rect0.MaxX == rect1.MaxX &&
				rect0.MaxY == rect1.MaxY);
		}

		public static bool operator !=(RectInt rect0, RectInt rect1)
		{
			return !(rect0 == rect1);
		}
	}
}
