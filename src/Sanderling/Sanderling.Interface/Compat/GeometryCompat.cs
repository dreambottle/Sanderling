using Bib3.Geometrik;
using Commons.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanderling.Interface.Compat
{
	public static class GeometryCompat
	{
		public static Vektor2DInt AsVektor2dInt(this Vector2i from) => new Vektor2DInt(from.X, from.Y);

		public static Vector2i AsVector2i(this Vektor2DInt from) => new Vector2i(from.A, from.B);
	}
}
