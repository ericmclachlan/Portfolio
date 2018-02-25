using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ericmclachlan.Portfolio.Tests
{
    [TestClass()]
    public class SearchHelperTests
    {
        [TestMethod()]
        public void BinarySearchTest_QuickSortIntegers()
        {
            var permutations = PermutationHelper.GeneratePermutation(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 5);
            foreach (var permutation in permutations)
            {
                SortHelper.QuickSort(permutation);
                for (int i = 0; i < permutation.Length; i++)
                {
                    Assert.IsTrue(i == 0 || permutation[i].CompareTo(permutation[i - 1]) >= 0);
                }
            }
        }

        [TestMethod()]
        public void BinarySearchTest_QuickSortStrings()
        {
            // Define the alphabet:
            string[] alphabet = new string[26];
            for (int i = 0; i < alphabet.Length; i++)
            {
                alphabet[i] = ('a' + i).ToString();
            }

            // Get every permutation consisting of 3 letters:
            var permutations = PermutationHelper.GeneratePermutation(alphabet, 5);
            foreach (var permutation in permutations)
            {
                // Sort each permutation:
                SortHelper.QuickSort(permutation);
                // Evaluate whether or not the sort was successful.
                for (int i = 0; i < permutation.Length; i++)
                {
                    Assert.IsTrue(i == 0 || permutation[i].CompareTo(permutation[i - 1]) >= 0);
                }
            }
        }

        public class TestComparable : IComparable
        {
            private int id;
            public TestComparable(int i)
            {
                this.id = i;
            }

            public int CompareTo(object that)
            {
                return this.id.CompareTo(((TestComparable)that).id);
            }
        }

        [TestMethod()]
        public void BinarySearchTest_QuickSortIComparable()
        {
            // Calculate all possible permutations of some arbitrary input (in this case integers):
            var permutations = PermutationHelper.GeneratePermutation(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 5);
            // Test each permutation:
            foreach (var permutation in permutations)
            {
                // 
                TestComparable[] objects = new TestComparable[permutation.Length];
                for (int i = 0; i < permutation.Length; i++)
                {
                    objects[i] = new TestComparable(permutation[i]);
                }
                SortHelper.QuickSort(objects);
                for (int i = 0; i < permutation.Length; i++)
                {
                    Assert.IsTrue(i == 0 || objects[i].CompareTo(objects[i - 1]) >= 0);
                }
            }
        }
    }
}