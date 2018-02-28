using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ericmclachlan.Portfolio
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
        public static double ReportAccuracy(ConfusionMatrix confusionMatrix, ValueIdMapper<string> classToclassId, string reportTitle)
        {
            // Write column headers:
            Console.WriteLine("Confusion matrix for the {0} data:", reportTitle.ToLower());
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
        /// <param name="sys_output">The location of the sys_output file.</param>
        /// <param name="heading">Usually, "Training" or "Test".</param>
        /// <param name="classifier">The classifier used to classify the test vectors.</param>
        /// <param name="vectors">A collection of vectors to classify.</param>
        /// <param name="classToclassId">A class for providing human-readable class labels.</param>
        internal static void GenerateSysOutputForVectors(
            string sys_output
            , FileCreationMode fileCreationMode
            , string heading
            , Classifier classifier
            , List<FeatureVector> vectors
            , ValueIdMapper<string> classToclassId
            , out ConfusionMatrix confusionMatrix
            , int gold_i)
        {
            confusionMatrix = null;
            StreamWriter writer = null;
            try
            {
                switch (fileCreationMode)
                {
                    case FileCreationMode.CreateNew: writer = File.CreateText(sys_output); break;
                    case FileCreationMode.Append:    writer = File.AppendText(sys_output); break;
                    default: throw new Exception($"Internal error: ProgramOutput.FileCreationMode {fileCreationMode} is not supported by this version of the application.");
                }

                writer.Write("%%%%% {0}:{1}", heading, Environment.NewLine);

                // For each of the vectors, ...
                confusionMatrix = new ConfusionMatrix(classToclassId.Count);
                for (int v_i = 0; v_i < vectors.Count; v_i++)
                {
                    // Output the {instanceName} {true_class_label} {details}
                    int sysClass;
                    string detailsAsText = GetDetailsAsText(classifier, vectors[v_i], classToclassId, out sysClass);
                    writer.WriteLine("array:{0}\t{1}", v_i, classToclassId[vectors[v_i].Headers[gold_i]], detailsAsText);
                }
                writer.WriteLine();
                writer.WriteLine();
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        private static string GetDetailsAsText(Classifier classifier, FeatureVector vector, ValueIdMapper<string> classToclassId, out int sysClass)
        {
            double[] distribution = classifier.Classify(vector);
            Debug.Assert(distribution.Length == classToclassId.Count && classToclassId.Count > 0);
            //int classId = 0;

            // Add an explicit class indicator "ClassId".
            //var probability_class = (from d in distribution
            //                         select new { ClassId = classId++, Value = d });

            //// Sort the probabilities in descending order of Value.
            //var sortedByProbability = (from d in probability_class
            //                           orderby d.Value descending
            //                           select d);

            var sortedByProbability = SearchHelper.GetMaxNItems(distribution.Length, distribution);
            sysClass = sortedByProbability[0];

            // Output the results:
            StringBuilder details = new StringBuilder();
            foreach (var result in sortedByProbability)
            {
                details.AppendFormat("\t{0}\t{1:0.00000}", classToclassId[result], distribution[result]);
            }
            string detailsAsText = details.ToString();
            return detailsAsText;
        }
    }
}
