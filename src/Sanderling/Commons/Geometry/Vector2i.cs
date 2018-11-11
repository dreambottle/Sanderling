using System;

namespace Commons.Geometry
{
	public struct Vector2i
	{
		// Fields
		public long X;

		public long Y;

		// Indexer
		public long this[int index]
		{
			get
			{
				switch (index)
				{
					case X_INDEX:
						return this.X;
					case Y_INDEX:
						return this.Y;
					default:
						throw new IndexOutOfRangeException("Invalid Vector2i index!");
				}
			}
			set
			{
				switch (index)
				{
					case X_INDEX:
						this.X = value;
						break;
					case Y_INDEX:
						this.Y = value;
						break;
					default:
						throw new IndexOutOfRangeException("Invalid Vector2i index!");
				}
			}
		}

		// Constructors
		public Vector2i(long xy)
		{
			this.X = xy;
			this.Y = xy;
		}

		public Vector2i(long x, long y)
		{
			this.X = x;
			this.Y = y;
		}

		// Properties
		public float SqrMagnitude => (float)X * X + (float)Y * Y;

		public float Magnitude => (float)Math.Sqrt(SqrMagnitude);

		public bool IsWithinBounds(Vector2i from, Vector2i to)
		{
			return this.X >= from.X && this.X < to.X &&
				   this.Y >= from.Y && this.Y < to.Y;
		}

		// Set
		public void Set(int new_x, int new_y)
		{
			this.X = new_x;
			this.Y = new_y;
		}

		// Scaling
		public void Scale(Vector2i scale)
		{
			X *= scale.X;
			Y *= scale.Y;
		}

		public void Scale(Vector2f scale)
		{
			X = (long)(X * scale.X);
			Y = (long)(Y * scale.Y);
		}

		public void Scale(float xScale, float yScale)
		{
			X = (long)(X * xScale);
			Y = (long)(Y * yScale);
		}

		// Rotations
		public void RotateCW()
		{
			var old_x = X;
			X = Y;
			Y = -old_x;
		}

		public void RotateCCW()
		{
			var old_x = X;
			X = -Y;
			Y = old_x;
		}

		public static Vector2i RotateCW(Vector2i a)
		{
			return new Vector2i(a.Y, -a.X);
		}

		public static Vector2i RotateCCW(Vector2i a)
		{
			return new Vector2i(-a.Y, a.X);
		}

		// Loops
		public static void RectLoop(Vector2i from, Vector2i to, Action<Vector2i> body)
		{
			if (body == null)
			{
				throw new ArgumentNullException(nameof(body));
			}

			var iterator = Vector2i.Zero;
			for (iterator.X = from.X; iterator.X < to.X; iterator.X++)
			{
				for (iterator.Y = from.Y; iterator.Y < to.Y; iterator.Y++)
				{
					body(iterator);
				}
			}
		}

		// ToString
		public override string ToString() => $"{{{nameof(Vector2i)}}}({X}, {Y})";

		// Operators
		public static Vector2i operator +(Vector2i a, Vector2i b)
		{
			return new Vector2i(
				a.X + b.X,
				a.Y + b.Y
			);
		}

		public static Vector2i operator -(Vector2i a)
		{
			return new Vector2i(
				-a.X,
				-a.Y
			);
		}

		public static Vector2i operator -(Vector2i a, Vector2i b)
		{
			return a + (-b);
		}

		public static Vector2i operator *(long d, Vector2i a)
		{
			return new Vector2i(
				d * a.X,
				d * a.Y
			);
		}

		public static Vector2i operator *(Vector2i a, long d)
		{
			return d * a;
		}

		public static Vector2i operator /(Vector2i a, long d)
		{
			return new Vector2i(
				a.X / d,
				a.Y / d
			);
		}

		// Equality
		public static bool operator ==(Vector2i lhs, Vector2i rhs)
		{
			return lhs.X == rhs.X && lhs.Y == rhs.Y;
		}

		public static bool operator !=(Vector2i lhs, Vector2i rhs)
		{
			return !(lhs == rhs);
		}

		public override bool Equals(object other)
		{
			if (!(other is Vector2i))
			{
				return false;
			}
			return this == (Vector2i)other;
		}

		public bool Equals(Vector2i other)
		{
			return this == other;
		}

		public override int GetHashCode()
		{
			return (X.GetHashCode() << 6) ^ Y.GetHashCode();
		}

		// Static methods

		public static float Distance(Vector2i a, Vector2i b)
		{
			return (a - b).Magnitude;
		}

		public static Vector2i Min(Vector2i lhs, Vector2i rhs)
		{
			return new Vector2i(
				Math.Min(lhs.X, rhs.X),
				Math.Min(lhs.Y, rhs.Y)
			);
		}

		public static Vector2i Max(Vector2i a, Vector2i b)
		{
			return new Vector2i(
				Math.Max(a.X, b.X),
				Math.Max(a.Y, b.Y)
			);
		}

		public static long Dot(Vector2i lhs, Vector2i rhs)
		{
			return lhs.X * rhs.X +
				   lhs.Y * rhs.Y;
		}

		public static float MagnitudeOf(Vector2i a)
		{
			return a.Magnitude;
		}

		public static float SqrMagnitudeOf(Vector2i a)
		{
			return a.SqrMagnitude;
		}

		// Default values
		public static Vector2i Down
		{
			get { return new Vector2i(0, -1); }
		}

		public static Vector2i Up
		{
			get { return new Vector2i(0, +1); }
		}

		public static Vector2i Left
		{
			get { return new Vector2i(-1, 0); }
		}

		public static Vector2i Right
		{
			get { return new Vector2i(+1, 0); }
		}

		public static Vector2i One
		{
			get { return new Vector2i(+1, +1); }
		}

		public static Vector2i Zero
		{
			get { return new Vector2i(0, 0); }
		}

		// Constants
		public const int X_INDEX = 0;
		public const int Y_INDEX = 1;
	}

}
