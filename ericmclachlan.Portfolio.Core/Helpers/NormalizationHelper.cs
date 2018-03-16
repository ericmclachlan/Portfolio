using System;
using System.Collections.Generic;

namespace ericmclachlan.Portfolio.Core
{
    public static class NormalizationHelper
    {
        /// <summary>Performs an in-place normalizes of the specified <c>distribution</c>.</summary>
        public static void Normalize(IList<double> values)
        {
            // TODO: Find a more discriptive name for this kind of normalization.
            _Normalize(values, values);
        }

        /// <summary>Returns a new array containing a normalized distribution based on the specified <c>input</c>.</summary>
        internal static double[] CreateNormalizedDistribution(IList<double> input)
        {
            // TODO: Find a more discriptive name for this kind of normalization.
            double[] output = new double[input.Count];
            _Normalize(input, output);
            return output;
        }

        /// <summary>Uses a normalized exponential function to normalize the specified <c>values</c>.</summary>
        public static void Softmax(double[] values)
        {
            _Softmax(values, values);
        }

        /// <summary>Uses a normalized exponential function to create a normal distribution from the specified <c>values</c>.</summary>
        public static double[] CreateSoftmaxNormalization(IList<double> input)
        {
            double[] output = new double[input.Count];
            _Softmax(input, output);
            return output;
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


        // Private Methods

        private static void _Normalize(IList<double> input, IList<double> output)
        {
            // Calculate the normalization denominator:
            double denominator = 0;
            for (int i = 0; i < input.Count; i++)
            {
                denominator += input[i];
            }

            // Normalize the distribution.
            for (int i = 0; i < input.Count; i++)
            {
                output[i] = input[i] / denominator;
            }
        }

        private static void _Softmax(IList<double> input, IList<double> output)
        {
            // Calculate the normalization denominator:
            double denominator = 0;
            for (int i = 0; i < input.Count; i++)
            {
                denominator += Math.Pow(Math.E, input[i]);
            }

            // Normalize the distribution.
            for (int i = 0; i < input.Count; i++)
            {
                output[i] = Math.Pow(Math.E, input[i]) / denominator;
            }
        }
    }
}
