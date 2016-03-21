using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SocialGamificationAsset.Helpers
{
    public class GenericHelper
    {
        /**
		 * The modern version of the Fisher–Yates shuffle aka 'Algorithm P'
		 * -- To shuffle an array a of n elements (indices 0..n-1):
		 * for i from n−1 down to 1 do
		 *		j ← random integer such that 0 ≤ j ≤ i
		 *		exchange a[j] and a[i]
		 */

        public static IList<T> Shuffle<T>(IList<T> list, int limit = 1)
        {
            var r = new Random();
            for (var i = list.Count - 1; i >= 1; i--)
            {
                // 0 ≤ j ≤ i
                var j = r.Next(0, i + 1);
                var value = list[i];
                list[i] = list[j];
                list[j] = value;
            }

            return list.Take(limit).ToList();
        }

        public static string SanitizeString(string str)
        {
            return Path.GetInvalidFileNameChars()
                       .Aggregate(str, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
    }
}
