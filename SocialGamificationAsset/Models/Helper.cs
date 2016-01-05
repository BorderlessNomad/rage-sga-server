using System;
using System.Collections.Generic;

namespace SocialGamificationAsset.Models
{
	public static class Helper
	{
		private static Random rng = new Random();

		public static void Shuffle<T>(this IList<T> list, int limit = -1)
		{
			int n = list.Count;
			if (limit != -1)
			{
				n = limit;
			}
			while (n > 1)
			{
				n--;
				int k = rng.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
	}
}
