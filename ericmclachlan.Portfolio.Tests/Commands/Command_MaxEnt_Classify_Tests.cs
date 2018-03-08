using ericmclachlan.Portfolio.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ericmclachlan.Portfolio.Tests
{
    [TestClass()]
    public class Command_MaxEnt_Classify_Tests
    {
        [TestMethod()]
        public void ExecuteCommand_MaxEnt_Classify_Test()
        {
            // Set up the parameters:
            using (var vectors_inFile = new TempFile(Resources.SVMLight_TestVectors))
            using (var model_inFile = new TempFile(Resources.Model_MaxEnt_Mallet))
            using (var sys_outFile = new TempFile())
            {
                // Initialize the commmand parameters:
                string command = $"maxent_classify {vectors_inFile.Location} {model_inFile.Location} {sys_outFile.Location}";
                string[] args = TextHelper.SplitOnWhitespace(command);

                // Execute the command:
                double accuracy = (double)CommandPlatform.Execute(args);

                // Confirm the result:
                Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(accuracy, 0.82667D, 5));
            }
        }
    }
}