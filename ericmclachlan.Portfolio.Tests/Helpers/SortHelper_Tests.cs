using ericmclachlan.Portfolio.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ericmclachlan.Portfolio.Tests
{
    [TestClass()]
    public class SortHelper_Tests
    {
        [TestMethod()]
        public void SortHelper_IntegersTuples_Odd_QuickSort_Test()
        {
            var tuples = InputProvider.GetIntegerTuples_Odd();
            foreach (var tuple in tuples)
            {
                SortHelper.QuickSort(tuple);
                for (int i = 1; i < tuple.Count; i++)
                {
                    Assert.IsTrue(tuple[i].CompareTo(tuple[i - 1]) >= 0);
                }
            }
        }

        [TestMethod()]
        public void SortHelper_IntegersTuples_Even_QuickSort_Test()
        {
            var tuples = InputProvider.GetIntegerTuples_Even();
            foreach (var tuple in tuples)
            {
                SortHelper.QuickSort(tuple);
                for (int i = 1; i < tuple.Count; i++)
                {
                    Assert.IsTrue(tuple[i].CompareTo(tuple[i - 1]) >= 0);
                }
            }
        }

        [TestMethod()]
        public void SortHelper_QuickSort_Strings_Test()
        {
            // Define the alphabet:
            string[] alphabet = new string[26];
            for (int i = 0; i < alphabet.Length; i++)
            {
                alphabet[i] = ('a' + i).ToString();
            }

            // Get every permutation consisting of 3 letters:
            var permutations = CombinatoricsHelper.GenerateNTuples(alphabet, 4);
            foreach (var permutation in permutations)
            {
                // Sort each permutation:
                SortHelper.QuickSort(permutation);
                // Evaluate whether or not the sort was successful.
                for (int i = 1; i < permutation.Count; i++)
                {
                    Assert.IsTrue(permutation[i].CompareTo(permutation[i - 1]) >= 0);
                }
            }
        }

        [TestMethod()]
        public void SortHelper_IComparableTuples_Odd_QuickSort_Test()
        {
            var tuples = InputProvider.GetIntegerTuples_Odd();
            foreach (var tuple in tuples)
            {
                TestComparable[] objects = new TestComparable[tuple.Count];
                for (int i = 0; i < tuple.Count; i++)
                {
                    objects[i] = new TestComparable(tuple[i]);
                }
                SortHelper.QuickSort(objects);
                for (int i = 1; i < tuple.Count; i++)
                {
                    Assert.IsTrue(objects[i].CompareTo(objects[i - 1]) >= 0);
                }
            }
        }

        [TestMethod()]
        public void SortHelper_IComparableTuples_Even_QuickSort_Test()
        {
            var tuples = InputProvider.GetIntegerTuples_Even();
            foreach (var tuple in tuples)
            {
                TestComparable[] objects = new TestComparable[tuple.Count];
                for (int i = 0; i < tuple.Count; i++)
                {
                    objects[i] = new TestComparable(tuple[i]);
                }
                SortHelper.QuickSort(objects);
                for (int i = 1; i < tuple.Count; i++)
                {
                    Assert.IsTrue(objects[i].CompareTo(objects[i - 1]) >= 0);
                }
            }
        }

        [TestMethod()]
        public void SortHelper_Integers_1Million_QuickSort_Test()
        {
            var nums = InputProvider.GetRandomIntegers_1Million();
            SortHelper.QuickSort(nums);
            for (int i = 1; i < nums.Count; i++)
            {
                Assert.IsTrue(nums[i].CompareTo(nums[i - 1]) >= 0);
            }
        }

        [TestMethod()]
        public void SortHelper_IntegersTuples_Odd_RadixSort_Test()
        {
            var tuples = InputProvider.GetIntegerTuples_Odd();
            foreach (var tuple in tuples)
            {
                SortHelper.RadixSort(tuple, 10);
                for (int i = 1; i < tuple.Count; i++)
                {
                    Assert.IsTrue(tuple[i].CompareTo(tuple[i - 1]) >= 0);
                }
            }
        }

        [TestMethod()]
        public void SortHelper_IntegersTuples_Even_RadixSort_Test()
        {
            var tuples = InputProvider.GetIntegerTuples_Even();
            foreach (var tuple in tuples)
            {
                SortHelper.RadixSort(tuple, 10);
                for (int i = 1; i < tuple.Count; i++)
                {
                    Assert.IsTrue(tuple[i].CompareTo(tuple[i - 1]) >= 0);
                }
            }
        }

        [TestMethod()]
        public void SortHelper_Integers_1Million_RadixSort_Test()
        {
            var nums = InputProvider.GetRandomIntegers_1Million();
            SortHelper.RadixSort(nums, 10);
            for (int i = 1; i < nums.Count; i++)
            {
                Assert.IsTrue(nums[i].CompareTo(nums[i - 1]) >= 0);
            }
        }
    }
}