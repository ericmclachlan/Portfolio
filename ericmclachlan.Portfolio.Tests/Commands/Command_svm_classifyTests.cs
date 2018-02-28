using ericmclachlan.Portfolio.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace ericmclachlan.Portfolio.Tests
{
    [TestClass()]
    public class Command_svm_classifyTests
    {
        [TestMethod()]
        public void ExecuteCommand_svm_classify_Test()
        {
            try
            {
                File.WriteAllText("libSVM_model", Resources.libSVM_model);
                File.WriteAllText("libSVM_test", Resources.libSVM_test);
                File.WriteAllText("libSVM_train", Resources.libSVM_train);

                string command;
                string[] args;

                // Evaluate Training:
                command = @"svm_classify libSVM_train libSVM_model sys_output_train";
                args = TextHelper.SplitOnWhitespace(command);
                CommandPlatform.Execute(args);

                // Evaluate Testing:
                command = @"svm_classify libSVM_test libSVM_model sys_output_test";
                args = TextHelper.SplitOnWhitespace(command);
                CommandPlatform.Execute(args);
            }
            finally
            {
                // Cleanup
                if (File.Exists("libSVM_model"))
                    File.Delete("libSVM_model");
                if (File.Exists("libSVM_test"))
                    File.Delete("libSVM_test");
                if (File.Exists("libSVM_train"))
                    File.Delete("libSVM_train");
            }
        }
    }
}