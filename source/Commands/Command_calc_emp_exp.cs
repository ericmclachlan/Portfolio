using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ericmclachlan.Portfolio
{
    internal class Command_calc_emp_exp : ICommand
    {
        // Members

        public string CommandName { get { return "calc_emp_exp"; } }

        private ValueIdMapper<string> classToClassId = new ValueIdMapper<string>();
        private ValueIdMapper<string> featureToFeatureId = new ValueIdMapper<string>();
        int gold_i = 0;

        [CommandParameter(Index = 0, Type = CommandParameterType.InputFile)]
        public string training_data_file { get; set; }



        // Methods

        public void ExecuteCommand()
        {
            // Load the training file.
            int noOfHeadersColumns = 1;
            ValueIdMapper<string>[] headerToHeaderIds;
            Program.CreateValueIdMappers(noOfHeadersColumns, gold_i, out featureToFeatureId, out headerToHeaderIds, out classToClassId);

            int[][] headers;
            var trainingVectors = FeatureVector.LoadFromSVMLight(training_data_file, featureToFeatureId, headerToHeaderIds, noOfHeadersColumns, out headers, FeatureType.Binary, featureDelimiter: ' ', isSortRequiredForFeatures: false);
            var goldClasses = headers[gold_i];

            double[,] observation, expectation;
            CalculateObservationAndEmpiricalExpectation(trainingVectors, out observation, out expectation);

            OutputEmpiricalCount(observation, expectation, requiresSort:true);
        }

        private void OutputEmpiricalCount(double[,] observation, double[,] expectation, bool requiresSort)
        {
            for (int c_i = 0; c_i < classToClassId.Count; c_i++)
            {
                List<string> output = new List<string>();
                for (int f_i = 0; f_i < featureToFeatureId.Count; f_i++)
                {
                    output.Add(string.Format("{0}\t{1}\t{2:0.00000}\t{3}", classToClassId[c_i], featureToFeatureId[f_i], expectation[f_i, c_i], observation[f_i, c_i]));
                }
                if (requiresSort)
                    output.Sort();
                foreach (var item in output)
                {
                    Console.WriteLine(item);
                }
            }
        }

        private void CalculateObservationAndEmpiricalExpectation(List<FeatureVector> trainingVectors, out double[,] observation, out double[,] expectation)
        {
            observation = new double[featureToFeatureId.Count, classToClassId.Count];
            expectation = new double[observation.GetLength(0), observation.GetLength(1)];
            double[] sum_byClass = new double[classToClassId.Count];
            double[] sum_byFeature = new double[featureToFeatureId.Count];
            double count = 0;
            double fraction = 1D / trainingVectors.Count;
            for (int v_i = 0; v_i < trainingVectors.Count; v_i++)
            {
                for (int u_i = 0; u_i < trainingVectors[v_i].UsedFeatures.Length; u_i++)
                {
                    int c_i = trainingVectors[v_i].Headers[gold_i];
                    int f_i = trainingVectors[v_i].UsedFeatures[u_i];
                    Debug.Assert(trainingVectors[v_i].AllFeatures[f_i] == 1);
                    double featureValue = trainingVectors[v_i].AllFeatures[f_i];
                    observation[f_i, c_i] += featureValue;
                    sum_byClass[c_i] += featureValue;
                    sum_byFeature[f_i] += featureValue;
                    count += featureValue;
                    // Update the expectation:
                    expectation[f_i, c_i] += fraction;
                }
            }
        }
    }
}
