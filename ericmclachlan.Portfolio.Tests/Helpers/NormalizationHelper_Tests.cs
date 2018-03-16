using ericmclachlan.Portfolio.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ericmclachlan.Portfolio.Tests
{
    [TestClass()]
    public class NormalizationHelper_Tests
    {
        ReadOnlyCollection<double> givenInput = new ReadOnlyCollection<double>(new double[] { 1.0, 2.0, 3.0, 4.0, 1.0, 2.0, 3.0 });
        ReadOnlyCollection<double> expectedOutput = new ReadOnlyCollection<double>(new double[] { 0.0236405, 0.0642617, 0.174681, 0.474833, 0.0236405, 0.0642617, 0.174681 });

        [TestMethod()]
        public void Softmax_InPlace_Test()
        {
            var values = givenInput.ToArray();
            Assert.IsTrue(!object.ReferenceEquals(values, givenInput));
            NormalizationHelper.Softmax(values);
            double sum = 0;
            for (int i = 0; i < values.Length; i++)
            {
                double diff = Math.Abs(expectedOutput[i] - values[i]);
                Assert.IsTrue(diff < Math.Pow(10, -6));
                sum += values[i];
            }
            Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(sum, 1D));
        }

        [TestMethod()]
        public void Softmax_Create_Test()
        {
            var output = NormalizationHelper.CreateSoftmaxNormalization(givenInput);
            Assert.IsTrue(!object.ReferenceEquals(output, givenInput));
            double sum = 0;
            for (int i = 0; i < givenInput.Count; i++)
            {
                double diff = Math.Abs(expectedOutput[i] - output[i]);
                Assert.IsTrue(diff < Math.Pow(10, -6));
                sum += output[i];
            }
            Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(sum, 1D));
        }
    }
}