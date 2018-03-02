namespace ericmclachlan.Portfolio
{
    /// <summary>Provides a commandline interface to the kNN Classifier.</summary>
    internal class Command_build_kNN : ICommand
    {
        // Members

        public string CommandName { get { return "build_kNN"; } }

        [CommandParameter(Index = 0, Type = CommandParameterType.InputFile, Description = "Vector file in text format(cf.train.vectors.txt).")]
        public string training_data_file { get; set; }

        [CommandParameter(Index = 1, Type = CommandParameterType.InputFile, Description = "Vector file in text format (cf. test.vectors.txt).")]
        public string test_data_file { get; set; }

        [CommandParameter(Index = 2, Type = CommandParameterType.NonNegativeInteger, Description = "The value of k; i.e., the number of nearest neighbors chosen for classification.")]
        public int k_val { get; set; }

        [CommandParameter(Index = 3, Description = "The id of the similarity function. { 1=EuclideanDistance, 2=: CosineFunction}")]
        public int similarity_func { get; set; }

        [CommandParameter(Index = 4, Description = "The classification result on the training and test data (cf. sys1).")]
        public string sys_output { get; set; }


        // Public Methods

        public void ExecuteCommand()
        {
            FeatureVectorFile vectorFile_train = new FeatureVectorFile(path: training_data_file, noOfHeaderColumns: 1, featureDelimiter: ' ', isSortRequired: true);
            FeatureVectorFile vectorFile_test = new FeatureVectorFile(path: test_data_file, noOfHeaderColumns: 1, featureDelimiter: ' ', isSortRequired: true);
            
            int gold_i = 0;
            TextIdMapper featureToFeatureId = new TextIdMapper();
            TextIdMapper classToClassId = new TextIdMapper();
            TextIdMapper[] headerToHeaderIds = new TextIdMapper[] { classToClassId };

            var trainingVectors = vectorFile_train.LoadFromSVMLight(featureToFeatureId, headerToHeaderIds, FeatureType.Binary);
            var goldClasses_train = vectorFile_train.Headers[gold_i];

            var testVectors = vectorFile_test.LoadFromSVMLight(featureToFeatureId, headerToHeaderIds, FeatureType.Binary);
            var goldClasses_test = vectorFile_test.Headers[gold_i];

            var classifier = new kNNClassifier(k_val, (SimilarityFunction)similarity_func, trainingVectors, classToClassId.Count, gold_i);

            var systemClasses_train = classifier.Classify(trainingVectors);
            var systemClasses_test = classifier.Classify(testVectors);

            var details_train = ProgramOutput.GetDistributionDetails(classifier, trainingVectors, classToClassId);
            var details_test = ProgramOutput.GetDistributionDetails(classifier, testVectors, classToClassId);

            ProgramOutput.GenerateSysOutput(sys_output, FileCreationMode.CreateNew, trainingVectors, classToClassId, goldClasses_train, systemClasses_train, details_train, "training data");
            ProgramOutput.GenerateSysOutput(sys_output, FileCreationMode.Append, testVectors, classToClassId, goldClasses_test, systemClasses_test, details_test, "test data");
        }
    }
}
