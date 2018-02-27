using System;

namespace ericmclachlan.Portfolio
{
    internal class Command_build_NB2 : ICommand
    {
        // Members

        public string CommandName { get { return "build_NB2"; } }
        
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

        private ValueIdMapper<string> classToclassId = new ValueIdMapper<string>();
        private ValueIdMapper<string> featureToFeatureId = new ValueIdMapper<string>();


        // Methods

        public void ExecuteCommand()
        {
            var trainingVectors = FeatureVector.LoadFromSVMLight(training_data_file, featureToFeatureId, classToclassId, FeatureType.Continuous);
            var testVectors = FeatureVector.LoadFromSVMLight(test_data_file, featureToFeatureId, classToclassId, FeatureType.Continuous);

            Classifier classifier = new NaiveBayesClassifier_Multinomial(class_prior_delta, cond_prob_delta, trainingVectors, classToclassId.Count);

            ConfusionMatrix confusionMatrix;
            ProgramOutput.GenerateSysOutputForVectors(sys_output, FileCreationMode.CreateNew, "training data", classifier, trainingVectors, classToclassId, out confusionMatrix);
            ProgramOutput.ReportAccuracy(confusionMatrix, classToclassId, "Training");
            ProgramOutput.GenerateSysOutputForVectors(sys_output, FileCreationMode.Append, "test data", classifier, testVectors, classToclassId, out confusionMatrix);
            ProgramOutput.ReportAccuracy(confusionMatrix, classToclassId, "Test");
        }
    }
}
