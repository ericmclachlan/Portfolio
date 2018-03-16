using ericmclachlan.Portfolio.ConsoleApp;
using ericmclachlan.Portfolio.Core;
using ericmclachlan.Portfolio.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ericmclachlan.Portfolio.Tests
{
    [TestClass()]
    public class Command_SVM_Classify_Tests
    {
        [TestMethod()]
        public void ExecuteCommand_svm_classify_Test()
        {
            // Set up the parameters:
            using (var vectors_inFile = new TempFile(Resources.libSVM_test))
            using (var model_inFile = new TempFile(Resources.libSVM_model))
            using (var sys_outFile = new TempFile())
            {
                // Initialize the commmand parameters:
                string command = $"svm_classify {vectors_inFile.Location} {model_inFile.Location} {sys_outFile.Location}";
                string[] args = TextHelper.SplitOnWhitespace(command);

                // Execute the command:
                double accuracy = (double)CommandPlatform.Execute(args);

                // Confirm the result:
                Assert.AreEqual(accuracy, 0.95D);
            }
        }
    }
}