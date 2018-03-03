using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ericmclachlan.Portfolio
{
    internal class Command_calc_emp_exp : Command<bool>
    {
        // Members

        public override string CommandName { get { return "calc_emp_exp"; } }

        private TextIdMapper classToClassId = new TextIdMapper();
        private TextIdMapper featureToFeatureId = new TextIdMapper();
        int gold_i = 0;

        #region Parameters

        [CommandParameter(Index = 0, Type = CommandParameterType.InputFile)]
        public string vector_file { get; set; }

        #endregion


        // Methods

        public override bool ExecuteCommand()
        {
            FeatureVectorFile vectorFile = new FeatureVectorFile(path: vector_file, noOfHeaderColumns: 1, featureDelimiter: ' ', isSortRequired: false);

            // Load the training file.
            int gold_i = 0;
            TextIdMapper featureToFeatureId = new TextIdMapper();
            TextIdMapper classToClassId = new TextIdMapper();
            TextIdMapper[] headerToHeaderIds = new TextIdMapper[] { classToClassId };

            var trainingVectors = vectorFile.LoadFromSVMLight(featureToFeatureId, headerToHeaderIds, FeatureType.Binary);
            var goldClasses = vectorFile.Headers[gold_i];

            double[,] observation, expectation;
            CalculateObservationAndEmpiricalExpectation(trainingVectors, out observation, out expectation);

            OutputEmpiricalCount(observation, expectation, requiresSort:true);
            return true;
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
