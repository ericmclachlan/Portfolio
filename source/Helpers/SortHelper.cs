using System;
using System.Collections.Generic;

namespace ericmclachlan.Portfolio
{
    public static class SortHelper
    {
        /// <summary>Performs an in-place sort of the specified <c>items</c>.</summary>
        public static void QuickSort<T>(T[] items)
            where T : IComparable
        {
            if (items == null || items.Length <= 1)
                return;

            Queue<int[]> stack = new Queue<int[]>();
            int startIndex = 0;
            int endIndex = items.Length - 1;
            stack.Enqueue(new int[] { startIndex, endIndex });

            while (stack.Count > 0)
            {
                int[] parameters = stack.Dequeue();
                startIndex = parameters[0];
                endIndex = parameters[1];

                if (endIndex < startIndex)
                    continue;

                // The pivot element is selected to be the last element in the range.
                int pivotIndex = endIndex;
                int i = startIndex;

                //Debug.Assert(i >= 0);
                //Debug.Assert(endIndex < items.Length);
                //Debug.Assert(startIndex != endIndex);

                while (pivotIndex >= startIndex && i <= pivotIndex && endIndex - i >= 1)
                {
                    if (items[i].CompareTo(items[pivotIndex]) <= 0)
                    {
                        i++;
                        continue;
                    }
                    // Example: 5 (index: i) is bigger than 4 (the pivot (index: p)).
                    // e.g. | i |   | p | <- Indices
                    //      | 5 | ? |*4*| <- Values
                    // Temporarilly swap the pivot with the larger item (value: 5).
                    Swap(items, i, pivotIndex);
                    // e.g. | i |   | p | <- Indices
                    //      |*4*| ? | 5 | <- Values
                    // Now, swap the pivot item with the item before the original pivot index.
                    // e.g. | i |   | p | <- Indices
                    //      | ? |*4*| 5 | <- Values
                    Swap(items, i, --pivotIndex);
                }
                // We can assume that the items before the pivot are less than the pivot and
                // the items after the pivot are greater than the pivot.
                if (pivotIndex - startIndex > 1)
                {
                    stack.Enqueue(new int[] { startIndex, pivotIndex - 1 });
                }
                if (endIndex - pivotIndex > 1)
                {
                    stack.Enqueue(new int[] { pivotIndex + 1, endIndex });
                }
            }
        }

        private static void Swap(int[] items, int i, int j)
        {
            int mask = items[i] ^ items[j];
            items[i] ^= mask;
            items[j] ^= mask;
        }

        private static void Swap<T>(T[] items, int i, int j)
        {
            T temp = items[i];
            items[i] = items[j];
            items[j] = temp;
        }
    }
}
