using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ericmclachlan.Portfolio
{
    internal class Command_calc_model_exp : ICommand
    {
        // Members

        public string CommandName { get { return "calc_model_exp"; } }

        private TextIdMapper classToClassId = new TextIdMapper();
        private TextIdMapper featureToFeatureId = new TextIdMapper();

        #region Parameters

        [CommandParameter(Index = 0, Type = CommandParameterType.InputFile)]
        public string training_data_file { get; set; }

        [CommandParameter(Index = 1, Type = CommandParameterType.InputFile, IsRequired = false)]
        public string model_file { get; set; }

        #endregion


        // Methods

        public void ExecuteCommand()
        {
            FeatureVectorFile vectorFile_train = new FeatureVectorFile(path: training_data_file, noOfHeaderColumns: 1, featureDelimiter: ' ', isSortRequired: false);
            
            int gold_i = 0;
            TextIdMapper featureToFeatureId = new TextIdMapper();
            TextIdMapper classToClassId = new TextIdMapper();
            TextIdMapper[] headerToHeaderIds = new TextIdMapper[] { classToClassId };

            var trainingVectors = vectorFile_train.LoadFromSVMLight(featureToFeatureId, headerToHeaderIds, FeatureType.Binary);
            var goldClasses_train = vectorFile_train.Headers[gold_i];

            // model_file is optional. 
            Func<int, FeatureVector, double> calculate_Prob_c_v;
            // If it is not given, p(v|c_i) = 1/|C|, where |C| is the number of class_labels.
            if (string.IsNullOrWhiteSpace(model_file))
            {
                double kProbability = 1D / classToClassId.Count;
                calculate_Prob_c_v = (v, c_i) => { return kProbability; };
            }
            // If it is given, it is used to calculate p(y|xi). 
            else
            {
                MaxEntClassifier classifier = MaxEntClassifier.LoadModel(model_file, classToClassId, featureToFeatureId);
                calculate_Prob_c_v =
                    (c_i, v) => 
                    {
                        double[] details;
                        int sysClass = classifier.Classify(v, out details);
                        return details[c_i];
                    };
            }

            double[,] expectation = CalculateModelExpectation(trainingVectors, calculate_Prob_c_v);

            OutputEmpiricalCount(expectation, trainingVectors.Count, requiresSort:true);
        }

        private double[,] CalculateModelExpectation(List<FeatureVector> trainingVectors, Func<int, FeatureVector,  double> calculate_Prob_c_v)
        {
            double[,] expectation = new double[featureToFeatureId.Count, classToClassId.Count];
            double fraction = 1D / trainingVectors.Count;
            for (int v_i = 0; v_i < trainingVectors.Count; v_i++)
            {
                for (int u_i = 0; u_i < trainingVectors[v_i].UsedFeatures.Length; u_i++)
                {
                    for (int c_i = 0; c_i < classToClassId.Count; c_i++)
                    {
                        int f_i = trainingVectors[v_i].UsedFeatures[u_i];
                        Debug.Assert(trainingVectors[v_i].AllFeatures[f_i] == 1);
                        // Update the expectation:
                        expectation[f_i, c_i] += fraction * calculate_Prob_c_v(c_i, trainingVectors[v_i]);
                    }
                }
            }
            return expectation;
        }

        private void OutputEmpiricalCount(double[,] expectation, int noOfTrainingInstances, bool requiresSort)
        {
            for (int c_i = 0; c_i < classToClassId.Count; c_i++)
            {
                List<string> output = new List<string>();
                for (int f_i = 0; f_i < featureToFeatureId.Count; f_i++)
                {
                    // count = expectation multiplied by the number of training instances.
                    double count = expectation[f_i, c_i] * noOfTrainingInstances;
                    output.Add(string.Format("{0}\t{1}\t{2:0.00000}\t{3:0.00000}", classToClassId[c_i], featureToFeatureId[f_i], expectation[f_i, c_i], count));
                }
                if (requiresSort)
                    output.Sort();
                foreach (var item in output)
                {
                    Console.WriteLine(item);
                }
            }
        }
    }
}
