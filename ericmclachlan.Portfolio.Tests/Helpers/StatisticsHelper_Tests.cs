using ericmclachlan.Portfolio.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ericmclachlan.Portfolio.Tests
{
    [TestClass()]
    public class StatisticsHelper_Tests
    {
        [TestMethod()]
        public void IsApproximatelyEqual_Test_NonFractions()
        {
            // Positive Tests
            Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(0, 0, decimals: 10));
            Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(0D, 0D, decimals: 10));

            // Negative Tests
            Assert.IsFalse(StatisticsHelper.IsApproximatelyEqual(0D, 1D, decimals: 1));
            Assert.IsFalse(StatisticsHelper.IsApproximatelyEqual(0, 1, decimals: 10));
            Assert.IsFalse(StatisticsHelper.IsApproximatelyEqual(0D, 1D, decimals: 10));
            Assert.IsFalse(StatisticsHelper.IsApproximatelyEqual(500D, 2D, decimals: 15));
        }

        [TestMethod()]
        public void IsApproximatelyEqual_Test_Fractions()
        {
            // Positive Tests
            Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(0.10D, 0.12D, decimals: 1));
            Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(0.01D, 0.00D, decimals: 1));
            Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(0.01D, 0.009D, decimals: 2));
            Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(0.01D, 0.009D, decimals: 1));

            // Negative Tests
            Assert.IsFalse(StatisticsHelper.IsApproximatelyEqual(0.01D, 0.00D, decimals: 2));
        }
    }
}