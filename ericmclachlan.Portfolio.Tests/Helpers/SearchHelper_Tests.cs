using ericmclachlan.Portfolio.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ericmclachlan.Portfolio.Tests
{
    [TestClass()]
    public class SearchHelper_Tests
    {
        [TestMethod()]
        public void SearchHelper_FindInsertIndex_Char_Test()
        {
            // Make sure the algorithm works for collections of a variety of sizes.
            for (int size = 0; size < 11; size++)
            {
                List<char> alphabet = new List<char>();
                // Create a sorted list:
                for (int i = 0; i < size; i++)
                {
                    alphabet.Add((char)('a' + (i % 26)));
                }
                for (int original_i = 0; original_i < alphabet.Count; original_i++)
                {
                    // Remove an element from the sorted list. 
                    var item = alphabet[original_i];
                    alphabet.RemoveAt(original_i);

                    // Test: Find the index at which to insert the removed item. (It should be the original index);
                    int insert_i = SearchHelper.FindInsertIndex(alphabet, item);

                    Assert.AreEqual(insert_i, original_i);

                    //Re-insert the item into the original position: 
                    alphabet.Insert(original_i, item);
                }
            }
        }

        [TestMethod()]
        public void SearchHelper_FindInsertIndex_Int32_Test()
        {
            // Make sure the algorithm works for collections of a variety of sizes.
            for (int size = 0; size < 11; size++)
            {
                var inputSet = new List<int>();
                // Create a sorted list:
                for (int i = 0; i < size; i++)
                {
                    inputSet.Add(i);
                }
                for (int original_i = 0; original_i < inputSet.Count; original_i++)
                {
                    // Remove an element from the sorted list. 
                    var item = inputSet[original_i];
                    inputSet.RemoveAt(original_i);

                    // Test: Find the index at which to insert the removed item. (It should be the original index);
                    int insert_i = SearchHelper.FindInsertIndex(inputSet, item);

                    Assert.AreEqual(insert_i, original_i);

                    //Re-insert the item into the original position: 
                    inputSet.Insert(original_i, item);
                }
            }
        }

        [TestMethod()]
        public void SearchHelper_FindInsertIndex_Comparable_Test()
        {
            // Make sure the algorithm works for collections of a variety of sizes.
            for (int size = 0; size < 11; size++)
            {
                var inputSet = new List<TestComparable>();
                // Create a sorted list:
                for (int i = 0; i < size; i++)
                {
                    inputSet.Add(new TestComparable(i));
                }
                for (int original_i = 0; original_i < inputSet.Count; original_i++)
                {
                    // Remove an element from the sorted list. 
                    var item = inputSet[original_i];
                    inputSet.RemoveAt(original_i);

                    // Test: Find the index at which to insert the removed item. (It should be the original index);
                    int insert_i = SearchHelper.FindInsertIndex(inputSet, item);

                    Assert.AreEqual(insert_i, original_i);

                    //Re-insert the item into the original position: 
                    inputSet.Insert(original_i, item);
                }
            }
        }
    }
}