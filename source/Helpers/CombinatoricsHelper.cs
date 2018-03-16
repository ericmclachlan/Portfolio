using System;
using System.Collections.Generic;
using System.Linq;

namespace ericmclachlan.Portfolio
{
    public class CombinatoricsHelper
    {
        #region Factorials

        private static long[] _factorialCache = new long[] { 1, 1, 2, 6, 24, 120, 720, 5040, 40320, 362880, 3628800, 39916800, 479001600, 6227020800, 87178291200, 1307674368000, 20922789888000, 355687428096000, 6402373705728000, 121645100408832000, 2432902008176640000 };

        /// <summary>Returns <c>n</c>!, where 0 <= n <= 20.</summary>
        public static long Factorial(int n)
        {
            // Validation:
            if (n < 0)
                throw new ArgumentOutOfRangeException(nameof(n), "The factorial of a negative number is undefined.");
            if (n >= _factorialCache.Length)
                throw new ArgumentOutOfRangeException(nameof(n), "The requested factorial is too big for a signed 64-bit integer.");

            //if (_factorialCache[n] == 0)
            //{
            //    if (n < 2)
            //        _factorialCache[n] = 1;
            //    else
            //        _factorialCache[n] = n * Factorial(n - 1);
            //}

            return _factorialCache[n];
        }

        #endregion

        #region GenerateNTuples

        /// <summary><para>
        /// Generated |elements|^n n-tuples from <c>elements</c>.
        /// </para><para>
        /// This is different from permutations which do not allow for sets with repeating elements, like { '0', '0', '0' }.
        /// </para></summary>
        /// <param name="n">The length of the n-tuples. e.g. If n is 2, each tuple will consist of 2 elements.</param>
        public static IList<IList<T>> GenerateNTuples<T>(IEnumerable<T> elements, int n)
        {
            var results = new List<IList<T>>();
            // Work-In-Progress: Contains a transient permutation of elements.
            var wip = new List<T>();    
            GenerateNTuples_Recursive(elements, n, wip, results);
            return results;
        }

        private static void GenerateNTuples_Recursive<T>(
            IEnumerable<T> elements
            , int choose
            , IList<T> wip
            , IList<IList<T>> results)
        {
            if (choose == 0)
            {
                results.Add(wip.ToArray());
                return;
            }

            foreach (var element in elements)
            {
                wip.Insert(wip.Count, element);

                GenerateNTuples_Recursive(elements, choose - 1, wip, results);

                wip.RemoveAt(wip.Count - 1);
            }
        }

        #endregion
        
        #region GeneratePermutations

        public static IList<IList<T>> GeneratePermutations<T>(IList<T> elements)
        {
            var results = new List<IList<T>>();
            List<T> availableElements = elements.ToList();
            List<T> workingSet = new List<T>();
            Permute_Recursive(results, availableElements, workingSet);
            return results;
        }

        private static void Permute_Recursive<T>(List<IList<T>> results, List<T> availableElements, List<T> workingSet)
        {
            // Tranfer an element from the availableElements to the working set and follow the inductive principle.
            for (int i = 0; i < availableElements.Count; i++)
            {
                T chosenElement = availableElements[i];

                // Choose an element and transfer it from the availableElements set to the workingSet.
                availableElements.RemoveAt(i);
                workingSet.Add(chosenElement);

                if (availableElements.Count == 0)
                {
                    // We have completed a permutation.
                    results.Add(workingSet.ToArray());
                }
                else
                {
                    Permute_Recursive(results, availableElements, workingSet);
                }

                // Reverse the chose so that we are back where we were at the start of this loop.
                availableElements.Insert(i, chosenElement);
                workingSet.RemoveAt(workingSet.Count - 1);
            }
        }

        #endregion
    }
}
