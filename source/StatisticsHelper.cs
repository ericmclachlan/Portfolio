using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ericmclachlan.Portfolio
{
    public static class StatisticsHelper
    {
        public static double Significance = Math.Pow(10, -10);

        // Public Methods

        /// <summary>Returns the index of the maximum value in the set.</summary>
        public static int ArgMax(IList<double> collection)
        {
            List<int> possibleMaxes = new List<int>();
            // Assume the first class is the best.
            int indexOfMax = 0;
            possibleMaxes.Add(0);
            // Find the best class.
            for (int i = 1; i < collection.Count; i++)
            {
                if (collection[i] == collection[indexOfMax])
                {
                    // Potential ambiguous result detected.
                    possibleMaxes.Add(i);
                }
                else if (collection[i] > collection[indexOfMax])
                {
                    // New max detected.
                    indexOfMax = i;
                    possibleMaxes.Clear();
                    possibleMaxes.Add(i);
                }
            }
            // TODO: To minimize bias, it would be best to randomly choose from the possibleMaxes, rather than systematically chosing the first category.
            //if (possibleMaxes.Count > 1)
            //{
            //    Console.Error.Write(".");
            //    Debug.Assert(indexOfMax == possibleMaxes[0]);
            //}

            return indexOfMax;
        }

        /// <summary>Calculates Shannon's information entropy; based on the distribution of the inputs.</summary>
        public static double CalculateEntropy(IEnumerable<int> values)
        {
            int denominator = 0;
            foreach (int val in values)
            {
                denominator += val;
            }
            // Avoid the division-by-zero.
            if (denominator == 0)
                return 0;

            double entropy = 0;
            foreach (int val in values)
            {
                entropy += CalculateEntropys_Subset(((double)val) / denominator);
            }
            return entropy;
        }

        /// <summary>Calculates Shannon's information entropy; based on the distribution of the inputs.</summary>
        public static double CalculateEntropy(params int[] values)
        {
            IEnumerable<int> enumerable = values as IEnumerable<int>;
            //Debug.Assert(enumerable != null);
            return CalculateEntropy(enumerable);
        }

        /// <summary>Calculates the information gain by splitting the data according to the specified distribution.</summary>
        public static double CalculateInformationGain(params int[][] features_byCategory)
        {
            // TODO: Make this less binary dependent.
            int[] totalRecords_byFeatureValue = new int[2];
            int[] denominators_byCategory = new int[features_byCategory.Length];
            int total_denominator = 0;
            for (int i = 0; i < features_byCategory.Length; i++)
            {
                int[] item = features_byCategory[i];
                for (int j = 0; j < item.Length; j++)
                {
                    totalRecords_byFeatureValue[j] += item[j];
                    denominators_byCategory[i] += item[j];
                    total_denominator += item[j];
                }
            }

            double informationGain = CalculateEntropy(totalRecords_byFeatureValue);

            for (int i = 0; i < features_byCategory.Length; i++)
            {
                int[] item = features_byCategory[i];
                double entropy = CalculateEntropy(item);
                double term = ((((double)denominators_byCategory[i]) / total_denominator) * entropy);
                informationGain -= term;
            }
            return informationGain;
        }

        /// <summary>Performs an in-place normalizes of the specified <c>distribution</c>.</summary>
        public static void Normalize(double[] distribution)
        {
            // Calculate the normalization denominator:
            double denominator = 0;
            for (int i = 0; i < distribution.Length; i++)
            {
                denominator += distribution[i];
            }

            // Normalize the distribution.
            double sanityCheck = 0;
            for (int i = 0; i < distribution.Length; i++)
            {
                distribution[i] = distribution[i] / denominator;
                sanityCheck += distribution[i];
            }
            Debug.Assert(IsApproximatelyEqual(sanityCheck, 1));
        }

        /// <summary>Normalizes (in-place) a collection of log values.</summary>
        public static void NormalizeLogs(double[] logs, double logBase)
        {
            double max = double.MinValue;
            // Find the maximum log value.
            for (int i = 0; i < logs.Length; i++)
            {
                if (logs[i] > max)
                    max = logs[i];
            }
            // Calculate the total mass:
            double totalWeight = 0;
            double[] weights = new double[logs.Length];
            for (int i = 0; i < logs.Length; i++)
            {
                logs[i] -= max;
                weights[i] = Math.Pow(logBase, logs[i]);
                totalWeight += weights[i];
            }
            // Normalize the distribution based on their relative mass.
            for (int i = 0; i < logs.Length; i++)
            {
                logs[i] = weights[i] / totalWeight;
            }
        }

        /// <summary>Converts the specified values into a distribution.</summary>
        internal static double[] ConvertToDistribution(double[] values)
        {
            double sum = 0;
            for (int i = 0; i < values.Length; i++)
            {
                sum += values[i];
            }
            double[] distribution = new double[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                distribution[i] += values[i] / sum;
            }
            return distribution;
        }

        /// <summary>Returns the chi-square value for the given observation table.</summary>
        public static double CalculateChiSquare(double[,] observationTable)
        {
            double[] sum_c = new double[observationTable.GetLength(0)];
            double[] sum_v = new double[observationTable.GetLength(1)];
            double sum = 0;

            // Parse 1: Sum the values.
            for (int r = 0; r < sum_c.Length; r++)
            {
                for (int c = 0; c < sum_v.Length; c++)
                {
                    sum_v[c] += observationTable[r, c];
                    sum_c[r] += observationTable[r, c];
                    sum += observationTable[r, c];
                }
            }

            // Parse 3: Calculate the expectation table:
            double[,] expectation = CalculateExpectation(observationTable, sum, sum_c, sum_v);

            // Parse 3: Calculate the chi-square values:
            //double[,] expectationTable = new double[observationTable.GetLength(0), observationTable.GetLength(1)];
            double chiSquare = 0;
            for (int r = 0; r < sum_c.Length; r++)
            {
                for (int c = 0; c < sum_v.Length; c++)
                {
                    double diff = observationTable[r, c] - expectation[r,c];
                    chiSquare += ((diff * diff) / expectation[r, c]);
                }
            }
            return chiSquare;
        }

        /// <summary>Calculates the expectation table for the given observation table.</summary>
        public static double[,] CalculateExpectation(double[,] observationTable, double sum, double[] totals_d1, double[] totals_d2)
        {
            int length_d1 = observationTable.GetLength(0);
            int length_d2 = observationTable.GetLength(1);

            double[,] expectationTable = new double[length_d1, length_d2];
            for (int d1_i = 0; d1_i < length_d1; d1_i++)
            {
                for (int d2_i = 0; d2_i < length_d2; d2_i++)
                {
                    // Define the expectation value:
                    //expectationTable[c_i, v_i] = sum_v[v_i] * sum_c[c_i] / sum;
                    //double diff = observationTable[c_i, v_i] - expectationTable[c_i, v_i];
                    //term = (diff * diff) / expectationTable[c_i, v_i];
                    expectationTable[d1_i, d2_i] = totals_d1[d1_i] * totals_d2[d2_i] / sum;
                }
            }
            return expectationTable;
        }

        /// <summary>Returns the Degrees of Freedom for the specified observation table.</summary>
        public static int CalculateChiDegreesOfFreedom(double[,] observationTable)
        {
            double[] sum_c = new double[observationTable.GetLength(0)];
            double[] sum_v = new double[observationTable.GetLength(1)];
            return (sum_c.Length - 1) * (sum_v.Length - 1);
        }


        // Private Methods

        private static double CalculateEntropys_Subset(double probability)
        {
            // If probability is zero, define the entropy as zero.
            if (IsApproximatelyEqual(probability, 0))
                return 0;

            // TODO: The algorithm can be optimized by removing the negation and working with negative entropy.
            return -1 * probability * Math.Log(probability, 2);
        }

        /// <summary>Returns true if x and y are virually equal.</summary>
        internal static bool IsApproximatelyEqual(double x, double y)
        {
            return x - Significance <= y && y <= x + Significance;
        }
    }
}
