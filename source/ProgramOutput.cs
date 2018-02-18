using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ericmclachlan.Portfolio
{
    /// <summary>This class encapsulates logic related to the generating output specific to this program.</summary>
    internal class ProgramOutput
    {
        // Internal Methods

        /// <summary>Reports the accuracy of the classifier.</summary>
        /// <param name="reportTitle">A text description of the set of data being classfied.</param>
        /// <param name="classToclassId">A lookup that maps class identifiers to their text representations.</param>
        internal static double ReportAccuracy(string reportTitle, int[,] confusionMatrix, ValueIdMapper<string> classToclassId)
        {
            // Write column headers:
            Console.WriteLine("Confusion matrix for the {0} data:", reportTitle.ToLower());
            Console.WriteLine("row is the truth, column is the system output");
            Console.WriteLine();
            Console.Write("            ");
            for (int i = 0; i < confusionMatrix.GetLength(0); i++)
            {
                Console.Write(" {0}", classToclassId[i]);
            }
            Console.WriteLine();
            // Write rows.
            for (int i = 0; i < confusionMatrix.GetLength(0); i++)
            {
                // Write row header:
                Console.Write("{0}", classToclassId[i]);
                // Write cells:
                for (int j = 0; j < confusionMatrix.GetLength(1); j++)
                {
                    Console.Write("\t{0}", confusionMatrix[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            double accuracy = StatisticsHelper.CalculateAccuracy(confusionMatrix);
            Console.WriteLine("  {0} accuracy={1:0.00000}", reportTitle, accuracy);
            Console.WriteLine();
            return accuracy;
        }


        // Private Methods

        internal static void GenerateSysOutputForVectors(string heading, Classifier classifier, List<FeatureVector> vectors, ValueIdMapper<string> classToclassId, StringBuilder sb)
        {
            sb.AppendFormat("%%%%% {0}:{1}", heading, Environment.NewLine);
            // For each of the vectors, ...
            for (int v_i = 0; v_i < vectors.Count; v_i++)
            {
                // Output the {instanceName} {true_class_label} ...
                sb.AppendFormat("array:{0}\t{1}", v_i, classToclassId[vectors[v_i].ClassId]);
                double[] distribution = classifier.Classify(vectors[v_i]);
                //Debug.Assert(distribution.Length == classToclassId.Count);
                int classId = 0;

                // Add an explicit class indicator "ClassId".
                var probability_class = (from d in distribution
                             select new { ClassId = classId++, Value = d });
                // Sort the probabilities in descending order of Value.
                var sortedByProbability = (from d in probability_class
                              orderby d.Value descending
                              select d);

                // Output the results:
                foreach (var result in sortedByProbability)
                {
                    sb.AppendFormat("\t{0}\t{1:0.00000}", classToclassId[result.ClassId], result.Value);
                }
                sb.AppendLine();
            }
            sb.AppendLine();
            sb.AppendLine();
        }
    }
}
