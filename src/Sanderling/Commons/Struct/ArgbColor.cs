using System;
using System.Collections.Generic;
using System.Text;

namespace Commons.Struct
{
	public class ArgbColor
	{
		public int? AMilli;
		public int? RMilli;
		public int? GMilli;
		public int? BMilli;

		public ArgbColor(int? aMilli, int? rMilli, int? gMilli, int? bMilli)
		{
			AMilli = aMilli;
			RMilli = rMilli;
			GMilli = gMilli;
			BMilli = bMilli;
		}

		public bool AllNotNull()
		{
			return AMilli != null &&
				RMilli != null &&
				GMilli != null &&
				BMilli != null;
		}
	}
}
