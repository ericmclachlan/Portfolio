using ericmclachlan.Portfolio.ConsoleApp;
using ericmclachlan.Portfolio.Core;
using ericmclachlan.Portfolio.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ericmclachlan.Portfolio.Tests
{
    [TestClass()]
    public class Command_BeamSearch_MaxEnt_Tests
    {
        [TestMethod()]
        public void ExecuteCommand_BeamSearch_MaxEnt_Test1()
        {
            // Initialize Parameters:
            using (var vectors_inFile = new TempFile(Resources.POS_Vectors))
            using (var boundary_inFile = new TempFile(Resources.POS_Vectors_Boundaries))
            using (var model_inFile = new TempFile(Resources.MaxEnt_Model))
            using (var sys_outFile = new TempFile())
            {
                int beamSize = 0;
                int topN = 1;
                int topK = 1;

                // Execute the command:
                double accuracy = (double)CommandPlatform.Execute(
                    $"beamsearch_maxent"
                    , vectors_inFile.Location
                    , boundary_inFile.Location
                    , model_inFile.Location
                    , sys_outFile.Location
                    , beamSize.ToString()
                    , topN.ToString()
                    , topK.ToString()
                );

                // Confirm the result:
                Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(accuracy, 0.9375D, 5));
            }
        }

        [TestMethod()]
        public void ExecuteCommand_BeamSearch_MaxEnt_Test2()
        {
            // Initialize Parameters:
            using (var vectors_inFile = new TempFile(Resources.POS_Vectors))
            using (var boundary_inFile = new TempFile(Resources.POS_Vectors_Boundaries))
            using (var model_inFile = new TempFile(Resources.MaxEnt_Model))
            using (var sys_outFile = new TempFile())
            {
                int beamSize = 1;
                int topN = 3;
                int topK = 5;

                // Execute the command:
                double accuracy = (double)CommandPlatform.Execute(
                    $"beamsearch_maxent"
                    , vectors_inFile.Location
                    , boundary_inFile.Location
                    , model_inFile.Location
                    , sys_outFile.Location
                    , beamSize.ToString()
                    , topN.ToString()
                    , topK.ToString()
                );

                // Confirm the result:
                Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(accuracy, 0.9375D, 5));
            }
        }

        [TestMethod()]
        public void ExecuteCommand_BeamSearch_MaxEnt_Test3()
        {
            // Initialize Parameters:
            using (var vectors_inFile = new TempFile(Resources.POS_Vectors))
            using (var boundary_inFile = new TempFile(Resources.POS_Vectors_Boundaries))
            using (var model_inFile = new TempFile(Resources.MaxEnt_Model))
            using (var sys_outFile = new TempFile())
            {
                int beamSize = 2;
                int topN = 5;
                int topK = 10;

                // Execute the command:
                double accuracy = (double)CommandPlatform.Execute(
                    $"beamsearch_maxent"
                    , vectors_inFile.Location
                    , boundary_inFile.Location
                    , model_inFile.Location
                    , sys_outFile.Location
                    , beamSize.ToString()
                    , topN.ToString()
                    , topK.ToString()
                );

                // Confirm the result:
                Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(accuracy, 0.9375D, 5));
            }
        }

        [TestMethod()]
        public void ExecuteCommand_BeamSearch_MaxEnt_Test4()
        {
            // Initialize Parameters:
            using (var vectors_inFile = new TempFile(Resources.POS_Vectors))
            using (var boundary_inFile = new TempFile(Resources.POS_Vectors_Boundaries))
            using (var model_inFile = new TempFile(Resources.MaxEnt_Model))
            {
                var sys_outFile = new TempFile();
                int beamSize = 3;
                int topN = 10;
                int topK = 100;

                // Execute the command:
                double accuracy = (double)CommandPlatform.Execute(
                    $"beamsearch_maxent"
                    , vectors_inFile.Location
                    , boundary_inFile.Location
                    , model_inFile.Location
                    , sys_outFile.Location
                    , beamSize.ToString()
                    , topN.ToString()
                    , topK.ToString()
                );

                // Confirm the result:
                Assert.IsTrue(StatisticsHelper.IsApproximatelyEqual(accuracy, 0.9375D, 5));
            }
        }
    }
}