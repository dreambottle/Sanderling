using System;

namespace Commons.Geometry
{
	public struct Vector2i
	{
		// Fields
		public int X;

		public int Y;

		// Indexer
		public int this[int index]
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
		public Vector2i(int xy)
		{
			this.X = xy;
			this.Y = xy;
		}

		public Vector2i(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		// Properties
		public float sqrMagnitude => (float)X * X + (float)Y * Y;

		public float magnitude => (float)Math.Sqrt(sqrMagnitude);

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

		public static Vector2i Scale(Vector2i a, Vector2i b)
		{
			return new Vector2i(
				a.X * b.X,
				a.Y * b.Y
			);
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

			var iterator = Vector2i.zero;
			for (iterator.X = from.X; iterator.X < to.X; iterator.X++)
			{
				for (iterator.Y = from.Y; iterator.Y < to.Y; iterator.Y++)
				{
					body(iterator);
				}
			}
		}

		// ToString
		public override string ToString()
		{
			return string.Format("({0}, {1})", X, Y);
		}

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

		public static Vector2i operator *(int d, Vector2i a)
		{
			return new Vector2i(
				d * a.X,
				d * a.Y
			);
		}

		public static Vector2i operator *(Vector2i a, int d)
		{
			return d * a;
		}

		public static Vector2i operator /(Vector2i a, int d)
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
			return (a - b).magnitude;
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

		public static int Dot(Vector2i lhs, Vector2i rhs)
		{
			return lhs.X * rhs.X +
				   lhs.Y * rhs.Y;
		}

		public static float Magnitude(Vector2i a)
		{
			return a.magnitude;
		}

		public static float SqrMagnitude(Vector2i a)
		{
			return a.sqrMagnitude;
		}

		// Default values
		public static Vector2i down
		{
			get { return new Vector2i(0, -1); }
		}

		public static Vector2i up
		{
			get { return new Vector2i(0, +1); }
		}

		public static Vector2i left
		{
			get { return new Vector2i(-1, 0); }
		}

		public static Vector2i right
		{
			get { return new Vector2i(+1, 0); }
		}

		public static Vector2i one
		{
			get { return new Vector2i(+1, +1); }
		}

		public static Vector2i zero
		{
			get { return new Vector2i(0, 0); }
		}

		// Constants
		public const int X_INDEX = 0;
		public const int Y_INDEX = 1;
	}

}
