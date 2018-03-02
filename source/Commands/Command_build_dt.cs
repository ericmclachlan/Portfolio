namespace ericmclachlan.Portfolio
{
    public class Command_build_dt : ICommand
    {
        // Properties

        public string CommandName { get { return "build_dt"; } }


        // Members
        
        [CommandParameter(Index = 0, Type = CommandParameterType.InputFile)]
        public string training_data_file { get; set; }

        [CommandParameter(Index = 1, Type = CommandParameterType.InputFile)]
        public string test_data_file { get; set; }

        [CommandParameter(Index = 2)]
        public int max_depth { get; set; }

        [CommandParameter(Index = 3)]
        public double min_gain { get; set; }

        [CommandParameter(Index = 4)]
        public string model_file { get; set; }

        [CommandParameter(Index = 5)]
        public string sys_output { get; set; }


        // Methods

        public void ExecuteCommand()
        {
            FeatureVectorFile vectorFile_train = new FeatureVectorFile(path: training_data_file, noOfHeaderColumns: 1, featureDelimiter: ' ', isSortRequired: false);
            FeatureVectorFile vectorFile_test = new FeatureVectorFile(path: test_data_file, noOfHeaderColumns: 1, featureDelimiter: ' ', isSortRequired: false);
            Program.ReportOnTrainingAndTesting(vectorFile_train, vectorFile_test, sys_output
                    , classifierFactory: (trainingVectors, gold_i, noOfClasses) =>
                    {
                        return new DecisionTreeClassifier(trainingVectors, gold_i, noOfClasses, max_depth, min_gain);
                    }
                    , getDetailsFunc: (classifier, vectors, classToClassId) =>
                    {
                        return ProgramOutput.GetDistributionDetails(classifier, vectors, classToClassId);
                    }
                );
        }
    }
}
