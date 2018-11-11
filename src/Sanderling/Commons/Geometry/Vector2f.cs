using System;

namespace Commons.Geometry
{
	public struct Vector2f
	{
		public float X;

		public float Y;

		public Vector2f(float x, float y)
		{
			this.X = x;
			this.Y = y;
		}

		public Vector2f(Vector2f from)
			:
			this(from.X, from.Y)
		{
		}

		static public Vector2f operator -(Vector2f minuend, Vector2f subtrahend) =>
			new Vector2f(minuend.X - subtrahend.X, minuend.Y - subtrahend.Y);

		static public Vector2f operator -(Vector2f subtrahend) =>
			new Vector2f(0, 0) - subtrahend;

		static public Vector2f operator +(Vector2f vektor0, Vector2f vektor1) =>
			new Vector2f(vektor0.X + vektor1.X, vektor0.Y + vektor1.Y);

		static public Vector2f operator /(Vector2f dividend, double divisor) =>
			new Vector2f((float)(dividend.X / divisor), (float)(dividend.Y / divisor));

		static public Vector2f operator *(Vector2f vektor0, double faktor) =>
			new Vector2f((float)(vektor0.X * faktor), (float)(vektor0.Y * faktor));

		static public Vector2f operator *(double faktor, Vector2f vektor0) =>
			new Vector2f((float)(vektor0.X * faktor), (float)(vektor0.Y * faktor));

		static public bool operator ==(Vector2f vektor0, Vector2f vektor1) =>
			vektor0.X == vektor1.X && vektor0.Y == vektor1.Y;

		static public bool operator !=(Vector2f vektor0, Vector2f vektor1) =>
			!(vektor0 == vektor1);

		override public bool Equals(object obj)
		{
			if (!(obj is Vector2f))
				return false;

			var otherVector = (Vector2f)obj;

			if (null == otherVector)
				return false;

			return this == otherVector;
		}

		override public int GetHashCode() =>
			X.GetHashCode() ^ Y.GetHashCode();

		public float SqrMagnitude => (float)X * (float)X + (float)Y * (float)Y;

		public double Magnitude => Math.Sqrt(SqrMagnitude);

		/// <summary>
		/// Returns a new vector.
		/// </summary>
		/// <returns></returns>
		public Vector2f Normalized()
		{
			var magnitude = this.Magnitude;
			return new Vector2f((float)(this.X / magnitude), (float)(this.Y / magnitude));
		}

		/// <summary>
		/// Mutable operation
		/// </summary>
		public void Normalize()
		{
			var Length = this.Magnitude;

			this.X = (float)(this.X / Length);
			this.Y = (float)(this.Y / Length);
		}

		static public double ScalarProduct(Vector2f vektor0, Vector2f vektor1) =>
			vektor0.X * vektor1.X + vektor0.Y * vektor1.Y;

		override public string ToString() =>
			$"{{{nameof(Vector2f)}}}({X}, {Y})";
	}
}
