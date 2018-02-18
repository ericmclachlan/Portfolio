using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ericmclachlan.Portfolio
{
    internal class Command_build_NB2 : ICommand
    {
        // Public Members

        public string CommandName { get { return "build_NB2"; } }

        // Other Members

        private ValueIdMapper<string> classToclassId = new ValueIdMapper<string>();
        private ValueIdMapper<string> featureToFeatureId = new ValueIdMapper<string>();

        private List<FeatureVector> testVectors = null;

        [CommandParameter(Index = 0, Type = CommandParameterType.InputFile, Description = "Vector file in text format(cf.train.vectors.txt).")]
        public string training_data_file { get; set; }

        [CommandParameter(Index = 1, Type = CommandParameterType.InputFile, Description = "Vector file in text format (cf. test.vectors.txt).")]
        public string test_data_file { get; set; }

        [CommandParameter(Index = 2, Description = "The δ used in add-δ smoothing when calculating the class prior P(c).")]
        public double class_prior_delta { get; set; }

        [CommandParameter(Index = 3, Description = "The δ used in add-δ smoothing when calculating the conditional probability P(f|c).")]
        public double cond_prob_delta { get; set; }

        [CommandParameter(Index = 4, Description = "Stores the values of P(c) and P(f|c) (cf. model1).")]
        public string model_file { get; set; }

        [CommandParameter(Index = 5, Description = "The classification result on the training and test data (cf. sys1).")]
        public string sys_output { get; set; }


        // Methods

        public void ExecuteCommand()
        {
            Func<int, int> transformationF = (i) => { return i; };
            var trainingVectors = FeatureVector.LoadFromSVMLight(File.ReadAllText(training_data_file), featureToFeatureId, classToclassId, transformationF);
            testVectors = FeatureVector.LoadFromSVMLight(File.ReadAllText(test_data_file), featureToFeatureId, classToclassId, transformationF);
            Classifier classifier = new NaiveBayesClassifier_Multinomial(class_prior_delta, cond_prob_delta, trainingVectors, classToclassId.Count);
            ProgramOutput.ReportAccuracy("Training", classifier.GetConfusionMatrix(classifier.TrainingVectors), classToclassId);
            StringBuilder sb = new StringBuilder();
            ProgramOutput.GenerateSysOutputForVectors("training data", classifier, classifier.TrainingVectors, classToclassId, sb);
            ProgramOutput.GenerateSysOutputForVectors("test data", classifier, testVectors, classToclassId, sb);
            File.WriteAllText(sys_output, sb.ToString());
            ProgramOutput.ReportAccuracy("Test", classifier.GetConfusionMatrix(testVectors), classToclassId);
        }
    }
}
