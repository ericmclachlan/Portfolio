using ericmclachlan.Portfolio.Core;

namespace ericmclachlan.Portfolio.ConsoleApp
{
    internal class Command_build_NB1 : Command<bool>
    {
        // Members

        public override string CommandName { get { return "build_NB1"; } }

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

        public override bool ExecuteCommand()
        {
            FeatureVectorFile vectorFile_train = new FeatureVectorFile(path: training_data_file, noOfHeaderColumns: 1, featureDelimiter: ' ', isSortRequired: false);
            FeatureVectorFile vectorFile_test = new FeatureVectorFile(path: test_data_file, noOfHeaderColumns: 1, featureDelimiter: ' ', isSortRequired: false);
            Program.ReportOnTrainingAndTesting(vectorFile_train, vectorFile_test, sys_output
                    , classifierFactory: (trainingVectors, gold_i, noOfClasses) =>
                    {
                        return new NaiveBayesClassifier_MultivariateBernoulli(
                            class_prior_delta
                            , cond_prob_delta
                            , trainingVectors
                            , noOfClasses
                            , gold_i);
                    }
                    , getDetailsFunc: (classifier, vectors, classToClassId) =>
                    {
                        return ProgramOutput.GetDistributionDetails(classifier, vectors, classToClassId);
                    }
                );
            return true;
        }
    }
}
