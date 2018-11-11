using System;

namespace Commons.Geometry
{
	public struct Vector3i
	{
		// Fields
		public int X;

		public int Y;

		public int Z;


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
					case Z_INDEX:
						return this.Z;
					default:
						throw new IndexOutOfRangeException("Invalid Vector3i index!");
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
					case Z_INDEX:
						this.Z = value;
						break;
					default:
						throw new IndexOutOfRangeException("Invalid Vector3i index!");
				}
			}
		}

		// Constructors
		public Vector3i(int x, int y)
		{
			this.X = x;
			this.Y = y;
			this.Z = 0;
		}

		public Vector3i(int x, int y, int z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		// Properties
		public float SqrMagnitude => (float)X * X + (float)Y * Y + (float)Z * Z;

		public float Magnitude => (float)Math.Sqrt(SqrMagnitude);

		public bool IsWithinBounds(Vector3i from, Vector3i to)
		{
			return this.X >= from.X && this.X < to.X &&
				   this.Y >= from.Y && this.Y < to.Y &&
				   this.Z >= from.Z && this.Z < to.Z;
		}

		// Set
		public void Set(int new_x, int new_y, int new_z)
		{
			this.X = new_x;
			this.Y = new_y;
			this.Z = new_z;
		}

		// Scaling
		public void Scale(Vector3i scale)
		{
			X *= scale.X;
			Y *= scale.Y;
			Z *= scale.Z;
		}

		public static Vector3i Scale(Vector3i a, Vector3i b)
		{
			return new Vector3i(
				a.X * b.X,
				a.Y * b.Y,
				a.Z * b.Z
			);
		}

		// Rotations

		public void RotateCW(int axis)
		{
			int temp;
			switch (axis)
			{
				case 0:
					temp = Y;
					Y = -Z;
					Z = temp;
					break;
				case 1:
					temp = X;
					X = Z;
					Z = -temp;
					break;
				case 2:
					temp = X;
					X = -Y;
					Y = temp;
					break;
				default:
					break;
			}
		}

		public void RotateCCW(int axis)
		{
			int temp;
			switch (axis)
			{
				case 0:
					temp = Y;
					Y = Z;
					Z = -temp;
					break;
				case 1:
					temp = X;
					X = -Z;
					Z = temp;
					break;
				case 2:
					temp = X;
					X = Y;
					Y = -temp;
					break;
				default:
					break;
			}
		}

		// Loops
		public enum LoopOrder : int
		{
			ZYX,
			ZXY,
			XZY,
			YZX,
			YXZ,
			XYZ
		}

		private static readonly int[,] loopCoords = new int[,]{
			{2,1,0},
			{2,0,1},
			{0,2,1},
			{1,2,0},
			{1,0,2},
			{0,1,2}
		};

		private static int GetCoord(LoopOrder loopOrder, int loopLevel)
		{
			return loopCoords[(int)loopOrder, loopLevel];
		}

		public static void CubeLoop(Vector3i from, Vector3i to, Action<Vector3i> body)
		{
			if (body == null)
			{
				throw new ArgumentNullException(nameof(body));
			}

			var iterator = Vector3i.Zero;
			for (iterator.X = from.X; iterator.X < to.X; iterator.X++)
			{
				for (iterator.Y = from.Y; iterator.Y < to.Y; iterator.Y++)
				{
					for (iterator.Z = from.Z; iterator.Z < to.Z; iterator.Z++)
					{
						body(iterator);
					}
				}
			}
		}

		// ToString
		public override string ToString() => $"{{{nameof(Vector3i)}}}({X}, {Y}, {Z})";

		// Operators
		public static Vector3i operator +(Vector3i a, Vector3i b)
		{
			return new Vector3i(
				a.X + b.X,
				a.Y + b.Y,
				a.Z + b.Z
			);
		}

		public static Vector3i operator -(Vector3i a)
		{
			return new Vector3i(
				-a.X,
				-a.Y,
				-a.Z
			);
		}

		public static Vector3i operator -(Vector3i a, Vector3i b)
		{
			return a + (-b);
		}

		public static Vector3i operator *(int d, Vector3i a)
		{
			return new Vector3i(
				d * a.X,
				d * a.Y,
				d * a.Z
			);
		}

		public static Vector3i operator *(Vector3i a, int d)
		{
			return d * a;
		}

		public static Vector3i operator /(Vector3i a, int d)
		{
			return new Vector3i(
				a.X / d,
				a.Y / d,
				a.Z / d
			);
		}

		// Equality
		public static bool operator ==(Vector3i lhs, Vector3i rhs)
		{
			return lhs.X == rhs.X &&
				   lhs.Y == rhs.Y &&
				   lhs.Z == rhs.Z;
		}

		public static bool operator !=(Vector3i lhs, Vector3i rhs)
		{
			return !(lhs == rhs);
		}

		public override bool Equals(object other)
		{
			if (!(other is Vector3i))
			{
				return false;
			}
			return this == (Vector3i)other;
		}

		public bool Equals(Vector3i other)
		{
			return this == other;
		}

		public override int GetHashCode()
		{
			return this.X.GetHashCode() ^ this.Y.GetHashCode() << 2 ^ this.Z.GetHashCode() >> 2;
		}

		// Static Methods

		public static float Distance(Vector3i a, Vector3i b)
		{
			return (a - b).Magnitude;
		}

		public static Vector3i Min(Vector3i lhs, Vector3i rhs)
		{
			return new Vector3i(
				Math.Min(lhs.X, rhs.X),
				Math.Min(lhs.Y, rhs.Y),
				Math.Min(lhs.Z, rhs.Z)
			);
		}

		public static Vector3i Max(Vector3i lhs, Vector3i rhs)
		{
			return new Vector3i(
				Math.Max(lhs.X, rhs.X),
				Math.Max(lhs.Y, rhs.Y),
				Math.Max(lhs.Z, rhs.Z)
			);
		}

		public static int Dot(Vector3i lhs, Vector3i rhs)
		{
			return lhs.X * rhs.X +
				   lhs.Y * rhs.Y +
				   lhs.Z * rhs.Z;
		}

		public static Vector3i Cross(Vector3i lhs, Vector3i rhs)
		{
			return new Vector3i(
				lhs.Y * rhs.Z - lhs.Z * rhs.Y,
				lhs.Z * rhs.X - lhs.X * rhs.Z,
				lhs.X * rhs.Y - lhs.Y * rhs.X
			);
		}

		public static float MagnitudeOf(Vector3i a)
		{
			return a.Magnitude;
		}

		public static float SqrMagnitudeOf(Vector3i a)
		{
			return a.SqrMagnitude;
		}

		// Default values
		public static Vector3i Back
		{
			get { return new Vector3i(0, 0, -1); }
		}

		public static Vector3i Forward
		{
			get { return new Vector3i(0, 0, 1); }
		}

		public static Vector3i Down
		{
			get { return new Vector3i(0, -1, 0); }
		}

		public static Vector3i Up
		{
			get { return new Vector3i(0, +1, 0); }
		}

		public static Vector3i Left
		{
			get { return new Vector3i(-1, 0, 0); }
		}

		public static Vector3i Right
		{
			get { return new Vector3i(+1, 0, 0); }
		}

		public static Vector3i One
		{
			get { return new Vector3i(+1, +1, +1); }
		}

		public static Vector3i Zero
		{
			get { return new Vector3i(0, 0, 0); }
		}

		// Constants
		public const int X_INDEX = 0;
		public const int Y_INDEX = 1;
		public const int Z_INDEX = 2;
	}
}
