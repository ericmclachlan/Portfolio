using ericmclachlan.Portfolio.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace ericmclachlan.Portfolio.Tests
{
    [TestClass()]
    public class Command_TBL_Test
    {
        [TestMethod()]
        public void Command_TBL_Test1()
        {
            using (var modelFile = new TempFile())
            {
                using (var trainFile = new TempFile())
                {
                    File.WriteAllText(trainFile.Path, Resources.FeatureVectors_train);

                    // Initialize the commmand parameters:
                    int min_gain = 30;
                    object result = CommandPlatform.Execute("TBL_train", trainFile.Path, modelFile.Path, min_gain.ToString());
                    Assert.AreEqual(result, 10);
                }                
            }
        }

        [TestMethod()]
        public void Command_TBL_Classify()
        {
            using (var modelFile = new TempFile())
            {
                File.WriteAllText(modelFile.Path, Resources.TBLModel);

                using (var testFile = new TempFile())
                {
                    File.WriteAllText(testFile.Path, Resources.FeatureVectors_test);
                    using (var sysOutputFile = new TempFile())
                    {
                        // Set up the output parameters:
                        string sys_output = "sys_output";
                        try
                        {
                            double accuracy;
                            accuracy = (double)CommandPlatform.Execute("TBL_classify", testFile.Path, modelFile.Path, sys_output, "1");
                            Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(accuracy, 0.41667, 4));

                            accuracy = (double)CommandPlatform.Execute("TBL_classify", testFile.Path, modelFile.Path, sys_output, "5");
                            Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(accuracy, 0.63667, 4));

                            accuracy = (double)CommandPlatform.Execute("TBL_classify", testFile.Path, modelFile.Path, sys_output, "10");
                            Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(accuracy, 0.69667, 4));

                            accuracy = (double)CommandPlatform.Execute("TBL_classify", testFile.Path, modelFile.Path, sys_output, "20");
                            Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(accuracy, 0.74000, 4));

                            accuracy = (double)CommandPlatform.Execute("TBL_classify", testFile.Path, modelFile.Path, sys_output, "50");
                            Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(accuracy, 0.76333, 4));

                            accuracy = (double)CommandPlatform.Execute("TBL_classify", testFile.Path, modelFile.Path, sys_output, "100");
                            Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(accuracy, 0.78667, 4));

                            accuracy = (double)CommandPlatform.Execute("TBL_classify", testFile.Path, modelFile.Path, sys_output, "200");
                            Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(accuracy, 0.77333, 4));
                        }
                        finally
                        {
                            // Cleanup the output file.
                            if (File.Exists(sys_output))
                                File.Delete(sys_output);
                        }
                    }
                }
            }
        }
    }
}