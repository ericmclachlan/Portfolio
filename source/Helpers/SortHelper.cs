using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ericmclachlan.Portfolio
{
    public static class SortHelper
    {
        /// <summary>Performs an in-place sort of the specified <c>items</c>.</summary>
        public static void QuickSort<T>(IList<T> items)
            where T : IComparable
        {
            if (items == null || items.Count <= 1)
                return;

            Queue<int[]> stack = new Queue<int[]>();
            int startIndex = 0;
            int endIndex = items.Count - 1;
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

        public static void RadixSort(IList<int> collection, int noOfBaskets)
        {
            // Create all the queues:
            var queues = new Queue<string>[noOfBaskets];
            for (int i = 0; i < noOfBaskets; i++)
                queues[i] = new Queue<string>();

            int maxValue = -1;
            // Add all the items in the collection to a queue: (The 1st queue has been chosen arbitrarilly.)
            // Perform validation of input at the same time.
            for (int i = 0; i < collection.Count; i++)
            {
                //if (collection[i] < 0)
                //    throw new NotImplementedException("Negative numbers are not supported by this Radix sort implementation.");
                string text = collection[i].ToString();
                if (text.Length > maxValue)
                    maxValue = text.Length;
                queues[0].Enqueue(text);
            }

            // Remember that all the items are in the first queue.
            int[] itemsInQueue_old = new int[noOfBaskets];
            itemsInQueue_old[0] = collection.Count;

            // Start the iterative part, ...
            for (int iteration = 0; iteration < maxValue; iteration++)
            {
                int[] itemsInQueue_new = new int[noOfBaskets];
                // Go through all the queues:
                for (int i = 0; i < queues.Length; i++)
                {
                    // Process all the items in this queue:
                    // (Bear in mind that additional items may be placed in this queue as it is recycled for this iteration.)
                    for (int j = 0; j < itemsInQueue_old[i]; j++)
                    {
                        string text = queues[i].Dequeue();
                        int basket;
                        if (text.Length <= iteration)
                        {
                            basket = 0;
                        }
                        else
                        {
                            char c = text[text.Length - 1 - iteration];
                            basket = c - '0';
                        }
                        queues[basket].Enqueue(text);
                        itemsInQueue_new[basket]++;
                    }
                }
                itemsInQueue_old = itemsInQueue_new;
            }

            // Return the result:
            int r_i = 0;
            // For each queue in the collection of queues, ...
            for (int i = 0; i < queues.Length; i++)
            {
                // Add the items to the final results.
                while (queues[i].Count > 0)
                {
                    collection[r_i++] = int.Parse(queues[i].Dequeue());
                }
            }
        }

        private static void Swap(IList<int> items, int i, int j)
        {
            int mask = items[i] ^ items[j];
            items[i] ^= mask;
            items[j] ^= mask;
        }

        private static void Swap<T>(IList<T> items, int i, int j)
        {
            T temp = items[i];
            items[i] = items[j];
            items[j] = temp;
        }
    }
}
