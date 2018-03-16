using ericmclachlan.Portfolio.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ericmclachlan.Portfolio.ConsoleApp
{
    public enum FileCreationMode
    {
        CreateNew,
        Append,
    }

    /// <summary>This class encapsulates logic related to the generating output specific to this program.</summary>
    internal class ProgramOutput
    {
        // Methods

        /// <summary>Reports the accuracy of the classifier, based on the specified <c>confusionMatrix</c>.</summary>
        /// <param name="confusionMatrix">The confusion matrix to report.</param>
        /// <param name="classToclassId">A lookup that maps class identifiers to their text representations.</param>
        /// <param name="reportTitle">A text description of the set of data being classfied.</param>
        private static double ReportAccuracy(ConfusionMatrix confusionMatrix, TextIdMapper classToclassId, string reportTitle)
        {
            // Write column headers:
            Console.WriteLine("Confusion matrix for '{0}':", reportTitle);
            Console.WriteLine("row is the truth, column is the system output");
            Console.WriteLine();
            Console.Write("            ");
            for (int i = 0; i < confusionMatrix.NoOfDimensions; i++)
            {
                Console.Write(" {0}", classToclassId[i]);
            }
            Console.WriteLine();
            // Write rows.
            for (int i = 0; i < confusionMatrix.NoOfDimensions; i++)
            {
                // Write row header:
                Console.Write("{0}", classToclassId[i]);
                // Write cells:
                for (int j = 0; j < confusionMatrix.NoOfDimensions; j++)
                {
                    Console.Write("\t{0}", confusionMatrix[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            double accuracy = confusionMatrix.CalculateAccuracy();
            Console.WriteLine($"  {reportTitle} accuracy={accuracy:0.00000}");
            Console.WriteLine();
            return accuracy;
        }



        // Private Methods

        /// <summary>
        /// Outputs the classification result as follows: {instanceName} {gold_class_label} {sys_class_label} {probability}.
        /// </summary>
        /// <param name="output_file">The location of the sys_output file.</param>
        /// <param name="vectors">A collection of vectors to classify.</param>
        /// <param name="classToclassId">A class for providing human-readable class labels.</param>
        /// <param name="heading">Usually, "Training" or "Test".</param>
        internal static double GenerateSysOutput(
            string output_file
            , FileCreationMode fileCreationMode
            , List<FeatureVector> vectors
            , TextIdMapper classToclassId
            , int[] goldClasses
            , int[] systemClasses
            , string[] details
            , string heading
            )
        {
            Debug.Assert(vectors != null && vectors.Count > 0);
            Debug.Assert(systemClasses != null && systemClasses.Length == vectors.Count);

            StreamWriter writer = null;
            try
            {
                switch (fileCreationMode)
                {
                    case FileCreationMode.CreateNew: writer = File.CreateText(output_file); break;
                    case FileCreationMode.Append:    writer = File.AppendText(output_file); break;
                    default: throw new Exception($"Internal error: ProgramOutput.FileCreationMode with value '{fileCreationMode}' is not supported by this version of the application.");
                }

                writer.Write($"%%%%% {heading}:{Environment.NewLine}");

                // For each of the vectors, ...
                var confusionMatrix = new ConfusionMatrix(classToclassId.Count);
                for (int v_i = 0; v_i < vectors.Count; v_i++)
                {
                    string trueLabel = classToclassId[goldClasses[v_i]];
                    string sysLabel = classToclassId[systemClasses[v_i]];

                    // Output the {true_class_label} {details}
                    writer.WriteLine($"{trueLabel}\t{sysLabel}\t{details[v_i]}");
                    confusionMatrix[goldClasses[v_i], systemClasses[v_i]]++;
                }
                writer.WriteLine();
                double accuracy = ReportAccuracy(confusionMatrix, classToclassId, heading);
                return accuracy;
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        public static string[] GetDistributionDetails(Classifier classifier, List<FeatureVector> vectors, TextIdMapper classToClassId)
        {
            string[] details = new string[vectors.Count];
            for (int v_i = 0; v_i < vectors.Count; v_i++)
            {
                double[] distribution;
                //int sysClass = 
                    classifier.Classify(vectors[v_i], out distribution);
                var distribution_sorted = SearchHelper.GetMaxNItems(distribution.Length, distribution);

                // Output the results:
                StringBuilder sb = new StringBuilder();
                foreach (var classId in distribution_sorted)
                {
                    // Output the results:
                    sb.AppendFormat("\t{0}\t{1:0.00000}", classToClassId[classId], distribution[classId]);
                }
                details[v_i] = sb.ToString();
            }
            return details;
        }
    }
}
