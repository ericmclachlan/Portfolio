using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ericmclachlan.Portfolio.Tests
{
    [TestClass()]
    public class CombinatoricsHelper_Tests
    {
        [TestMethod()]
        public void CombinatoricsHelper_GenerateNTuples_Test()
        {
            var elements = new List<int>();
            for (int noOfElements = 0; noOfElements < 5; noOfElements++)
            {
                elements.Add(noOfElements);
                for (int choose = 0; choose < 5; choose++)
                {
                    var results = CombinatoricsHelper.GenerateNTuples(elements, choose);
                    Assert.AreEqual(results.Count, Math.Pow(elements.Count, choose));
                }
            }
        }

        [TestMethod()]
        public void CombinatoricsHelper_GeneratePermutation_Test()
        {
            var elements = new List<int>();
            for (int noOfElements = 0; noOfElements < 5; noOfElements++)
            {
                elements.Add(noOfElements);
                for (int choose = 0; choose < noOfElements; choose++)
                {
                    var results = CombinatoricsHelper.GeneratePermutations(elements);
                    Assert.AreEqual(results.Count, CombinatoricsHelper.Factorial(elements.Count));
                }
            }
        }

        [TestMethod()]
        public void CombinatoricsHelper_Factorial_Test()
        {
            Assert.AreEqual(CombinatoricsHelper.Factorial(0), 1);
            Assert.AreEqual(CombinatoricsHelper.Factorial(1), 1);
            Assert.AreEqual(CombinatoricsHelper.Factorial(2), 2);
            Assert.AreEqual(CombinatoricsHelper.Factorial(3), 6);
            Assert.AreEqual(CombinatoricsHelper.Factorial(4), 24);
            Assert.AreEqual(CombinatoricsHelper.Factorial(5), 120);
            for (int i = 1; i < 21; i++)
            {
                Assert.AreEqual(CombinatoricsHelper.Factorial(i), i * CombinatoricsHelper.Factorial(i - 1));
            }
        }
    }
}