using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ericmclachlan.Portfolio.Tests
{
    [TestClass()]
    public class SortHelper_Tests
    {
        [TestMethod()]
        public void QuickSort_Integers_Test()
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
        public void QuickSort_Strings_Test()
        {
            // Define the alphabet:
            string[] alphabet = new string[26];
            for (int i = 0; i < alphabet.Length; i++)
            {
                alphabet[i] = ('a' + i).ToString();
            }

            // Get every permutation consisting of 3 letters:
            var permutations = PermutationHelper.GeneratePermutation(alphabet, 4);
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

        [TestMethod()]
        public void QuickSort_IComparable_Test()
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