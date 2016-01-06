using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialGamificationAsset.Models
{
	public static class Helper
	{
		private static Random rng = new Random();

		/**
		 * The modern version of the Fisher–Yates shuffle aka 'Algorithm P'
		 * -- To shuffle an array a of n elements (indices 0..n-1):
		 * for i from n−1 downto 1 do
		 *		j ← random integer such that 0 ≤ j ≤ i
		 *		exchange a[j] and a[i]
		 */

		public static IList<T> Shuffle<T>(IList<T> list, int limit = 1)
		{
			Random r = new Random();
			for (int i = list.Count - 1; i >= 1; i--)
			{
				// 0 ≤ j ≤ i
				int j = r.Next(0, i + 1);
				T value = list[i];
				list[i] = list[j];
				list[j] = value;
			}

			return list.Take(limit).ToList();
		}
	}
}
