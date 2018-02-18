using System.Collections.Generic;
using System.Linq;

namespace ericmclachlan.Portfolio
{
    public class PermutationHelper
    {
        public static List<T[]> GeneratePermutation<T>(IEnumerable<T> elements, int depth)
        {
            var results = new List<T[]>();
            var wip = new List<T>();
            GeneratePermutation_Recursive(elements, depth, wip, results);
            return results;
        }

        private static void GeneratePermutation_Recursive<T>(IEnumerable<T> elements, int i, IList<T> wip, IList<T[]> results)
        {
            if (i == 0)
            {
                results.Add(wip.ToArray());
                return;
            }
            foreach (var element in elements)
            {
                wip.Insert(wip.Count, element);
                GeneratePermutation_Recursive(elements, i - 1, wip, results);
                wip.RemoveAt(wip.Count - 1);
            }
        }
    }
}
