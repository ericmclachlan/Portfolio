using ericmclachlan.Portfolio.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace ericmclachlan.Portfolio.Tests
{
    [TestClass()]
    public class Command_SVM_classify_Tests
    {
        [TestMethod()]
        public void ExecuteCommand_svm_classify_Test()
        {
            // Set up the parameters:
            string model_file = "libSVM_model";
            string test_vectors = "libSVM_test_vectors";
            string sys_output = "libSVM_sys_output";

            try
            {
                // Create the input files:
                File.WriteAllText(model_file, Resources.libSVM_model);
                File.WriteAllText(test_vectors, Resources.libSVM_test);

                // Initialize the commmand parameters:
                string command = $"svm_classify {test_vectors} {model_file} {sys_output}";
                string[] args = TextHelper.SplitOnWhitespace(command);
                
                // Execture the command:
                var result = CommandPlatform.Execute(args);

                // Confirm the result:
                Assert.AreEqual(result, 0.95D);
            }
            finally
            {
                // Cleanup any files created during execution
                if (File.Exists(model_file))
                    File.Delete(model_file);
                if (File.Exists(test_vectors))
                    File.Delete(test_vectors);
                if (File.Exists(sys_output))
                    File.Delete(sys_output);
            }
        }
    }
}