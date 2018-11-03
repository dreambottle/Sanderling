using System;

namespace Sanderling.MemoryReading.Production
{
	public struct Vector2DSingle
	{
		public float A;

		public float B;

		public Vector2DSingle(float a, float b)
		{
			this.A = a;
			this.B = b;
		}

		public Vector2DSingle(Vector2DSingle from)
			:
			this(from.A, from.B)
		{
		}

		static public Vector2DSingle operator -(Vector2DSingle minuend, Vector2DSingle subtrahend) =>
			new Vector2DSingle(minuend.A - subtrahend.A, minuend.B - subtrahend.B);

		static public Vector2DSingle operator -(Vector2DSingle subtrahend) =>
			new Vector2DSingle(0, 0) - subtrahend;

		static public Vector2DSingle operator +(Vector2DSingle vektor0, Vector2DSingle vektor1) =>
			new Vector2DSingle(vektor0.A + vektor1.A, vektor0.B + vektor1.B);

		static public Vector2DSingle operator /(Vector2DSingle dividend, double divisor) =>
			new Vector2DSingle((float)(dividend.A / divisor), (float)(dividend.B / divisor));

		static public Vector2DSingle operator *(Vector2DSingle vektor0, double faktor) =>
			new Vector2DSingle((float)(vektor0.A * faktor), (float)(vektor0.B * faktor));

		static public Vector2DSingle operator *(double faktor, Vector2DSingle vektor0) =>
			new Vector2DSingle((float)(vektor0.A * faktor), (float)(vektor0.B * faktor));

		static public bool operator ==(Vector2DSingle vektor0, Vector2DSingle vektor1) =>
			vektor0.A == vektor1.A && vektor0.B == vektor1.B;

		static public bool operator !=(Vector2DSingle vektor0, Vector2DSingle vektor1) =>
			!(vektor0 == vektor1);

		override public bool Equals(object obj)
		{
			if (!(obj is Vector2DSingle))
				return false;

			var AlsVektor = (Vector2DSingle)obj;

			if (null == AlsVektor)
				return false;

			return this == AlsVektor;
		}

		override public int GetHashCode() =>
			A.GetHashCode() ^ B.GetHashCode();

		public double BetraagQuadriirt => A * A + B * B;

		public double Betraag => Math.Sqrt(BetraagQuadriirt);

		public Vector2DSingle Normalized()
		{
			var Betraag = this.Betraag;

			return new Vector2DSingle((float)(this.A / Betraag), (float)(this.B / Betraag));
		}

		public void Normalize()
		{
			var Length = this.Betraag;

			this.A = (float)(this.A / Length);
			this.B = (float)(this.B / Length);
		}

		static public double Skalarprodukt(Vector2DSingle vektor0, Vector2DSingle vektor1) =>
			vektor0.A * vektor1.A + vektor0.B * vektor1.B;

		override public string ToString() =>
			"{" + GetType().Name + "}(" + A.ToString() + " | " + B.ToString() + ")";
	}
}
