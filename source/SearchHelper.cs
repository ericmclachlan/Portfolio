using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ericmclachlan.Portfolio
{
    public static class SearchHelper
    {
        /// <summary>
        /// Returns the index at which <c>input</c> would exist in a sorted collection.
        /// </summary>
        /// <returns>
        /// <para>
        /// If <c>input</c> does exists in the collection, this method returns the index of the <b>first</b> occurance of <c>input</c>.
        /// </para><para>
        /// If <c>input</c> doesn't exist, the position at which <c>input</c> would be inserted is returned instead.
        /// </para>
        /// </returns>
        public static int BinarySearch<T>(this IList<T> collection, T input)
            where T : IComparable<T>
        {
            // Delegate the work to the private method.
            return BinarySearch_Recurse(collection, input, 0, collection.Count - 1);
        }

        private static int BinarySearch_Recurse<T>(IList<T> collection, T input, int startIndex, int endIndex)
            where T : IComparable<T>
        {
            // TODO: I think there's a bug with this implementation:

            if (startIndex > endIndex)
                return startIndex;

            // Assert that the input is sorted.
            var isSorted = collection[startIndex].CompareTo(collection[endIndex]) >= 0;
            if (!isSorted)
                Debug.Assert(false);

            int midpoint = startIndex + ((endIndex - startIndex) / 2);
            if (input.CompareTo(collection[midpoint]) > 0)
                return BinarySearch_Recurse(collection, input, midpoint + 1, endIndex);
            else
                return BinarySearch_Recurse(collection, input, startIndex, midpoint - 1);
        }


        /// <summary>Given a list of <c>items</c>, the indices of the top <c>N</c> items, will be returned.</summary>
        public static IList<int> GetMaxNItems<T>(int N, IList<T> items) 
            where T:IComparable<T>
        {
            var winners = new List<int>(N);
            for (int i = 0; i < items.Count; i++)
            {
                // If the list is empty OR this is the largest number we've seen, 
                // then add this item to the winners. (i.e. insert at position 0).
                if (winners.Count == 0 || items[i].CompareTo(items[winners[0]]) > 0)
                {
                    winners.Insert(0, i);
                }
                // If this neighbor is larger than our smallest winner, ...
                else if (winners.Count < N || items[i].CompareTo(items[winners[winners.Count - 1]]) > 0)
                {
                    int index = SpecialBinarySearch_Recurse<T>(winners, items[i], 0, winners.Count - 1, items);
                    if (index <= N)
                    {
                        winners.Insert(index, i);
                        Debug.Assert(index == 0 || items[winners[index - 1]].CompareTo(items[winners[index]]) >= 0);
                        Debug.Assert(winners[index] == i);
                        Debug.Assert(index == winners.Count - 1 || items[winners[index]].CompareTo(items[winners[index + 1]]) >= 0);
                    }
                }
                else
                {
                    continue;
                }
                // If we have too many items, then remove the furthest one.
                if (winners.Count > N)
                {
                    winners.RemoveAt(winners.Count - 1);
                }
                Debug.Assert(winners.Count <= N);
            }
            return winners;
        }

        private static int SpecialBinarySearch_Recurse<T>(IList<int> winners, T input, int startIndex, int endIndex, IList<T> items)
            where T : IComparable<T>
        {
            if (startIndex > endIndex)
                return startIndex;

            // Assert that the input is sorted.
            var isSorted = items[winners[startIndex]].CompareTo(items[winners[endIndex]]) >= 0;
            if (!isSorted)
                Debug.Assert(false);

            int midpoint = startIndex + ((endIndex - startIndex) / 2);
            if (input.CompareTo(items[winners[midpoint]]) < 0)
                return SpecialBinarySearch_Recurse(winners, input, midpoint + 1, endIndex, items);
            else
                return SpecialBinarySearch_Recurse(winners, input, startIndex, midpoint - 1, items);
        }
    }
}
