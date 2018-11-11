﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Commons.Utils
{
	public static class Math2
	{
		private static readonly Int64[] PowBase10Cache = {
			1,
			10,
			100,
			1000,
			10000,
			100000,
			1000000,
			10000000,
			100000000,
			1000000000,
			10000000000,
			100000000000,
			1000000000000,
			10000000000000,
			100000000000000,
			1000000000000000,
			10000000000000000,
			100000000000000000,
			1000000000000000000,
			};

		/// <summary>
		/// Efficient 10 to the power of n, which accepts and returns Int64 values.
		/// Cached values up until 10^18
		/// </summary>
		/// <param name="exp"></param>
		/// <returns></returns>
		public static Int64 IPowBase10(int exp)
		{
			return exp < PowBase10Cache.Length ? PowBase10Cache[exp] : IPow(10L, exp);
		}

		public static Int64 IPow(Int64 baseVal, int exp)
		{
			Int64 result = 1;
			while (exp > 0)
			{
				if ((exp & 1) != 0)
				{
					result *= baseVal;
				}
				exp >>= 1;
				baseVal *= baseVal;
			}
			return result;
		}
	}
}
