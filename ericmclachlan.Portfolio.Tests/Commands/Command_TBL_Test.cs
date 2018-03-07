using ericmclachlan.Portfolio.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ericmclachlan.Portfolio.Tests
{
    [TestClass()]
    public class Command_TBL_Test
    {
        [TestMethod()]
        public void Command_TBL_Test1()
        {
            using (var train_inFile = new TempFile(Resources.FeatureVectors_train))
            using (var model_outFile = new TempFile())
            {
                int min_gain = 30;

                int noOfTransformations = (int)CommandPlatform.Execute(
                    "TBL_train"
                    , train_inFile.Location
                    , model_outFile.Location
                    , min_gain.ToString());

                Assert.AreEqual(noOfTransformations, 10);
            }
        }

        [TestMethod()]
        public void Command_TBL_Classify()
        {
            using (var modelFile = new TempFile(Resources.TBLModel))
            using (var testFile = new TempFile(Resources.FeatureVectors_test))
            using (var sysOutputFile = new TempFile())
            {
                // Set up the output parameters:
                double accuracy = (double)CommandPlatform.Execute("TBL_classify", testFile.Location, modelFile.Location, sysOutputFile.Location, "1");
                Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(accuracy, 0.41667, 4));

                accuracy = (double)CommandPlatform.Execute("TBL_classify", testFile.Location, modelFile.Location, sysOutputFile.Location, "5");
                Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(accuracy, 0.63667, 4));

                accuracy = (double)CommandPlatform.Execute("TBL_classify", testFile.Location, modelFile.Location, sysOutputFile.Location, "10");
                Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(accuracy, 0.69667, 4));

                accuracy = (double)CommandPlatform.Execute("TBL_classify", testFile.Location, modelFile.Location, sysOutputFile.Location, "20");
                Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(accuracy, 0.74000, 4));

                accuracy = (double)CommandPlatform.Execute("TBL_classify", testFile.Location, modelFile.Location, sysOutputFile.Location, "50");
                Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(accuracy, 0.76333, 4));

                accuracy = (double)CommandPlatform.Execute("TBL_classify", testFile.Location, modelFile.Location, sysOutputFile.Location, "100");
                Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(accuracy, 0.78667, 4));

                accuracy = (double)CommandPlatform.Execute("TBL_classify", testFile.Location, modelFile.Location, sysOutputFile.Location, "200");
                Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(accuracy, 0.77333, 4));
            }
        }
    }
}