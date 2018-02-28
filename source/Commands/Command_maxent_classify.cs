namespace ericmclachlan.Portfolio
{
    internal class Command_maxent_classify : ICommand
    {
        // Members

        public string CommandName { get { return "maxent_classify"; } }

        [CommandParameter(Index = 0, Type = CommandParameterType.InputFile)]
        public string data_file { get; set; }

        [CommandParameter(Index = 1, Type = CommandParameterType.InputFile)]
        public string model_file { get; set; }

        [CommandParameter(Index = 2)]
        public string sys_output { get; set; }


        // Methods

        public void ExecuteCommand()
        {
            int noOfHeadersColumns = 1;
            int gold_i = 0;
            ValueIdMapper<string> featureToFeatureId;
            ValueIdMapper<string>[] headerToHeaderIds;
            ValueIdMapper<string> classToClassId;
            Program.CreateValueIdMappers(noOfHeadersColumns, gold_i, out featureToFeatureId, out headerToHeaderIds, out classToClassId);

            Classifier classifier = MaxEntClassifier.LoadModel(model_file, out classToClassId, out featureToFeatureId);

            int[][] headers;
            var vectors = FeatureVector.LoadFromSVMLight(data_file, featureToFeatureId, headerToHeaderIds, noOfHeadersColumns, out headers, FeatureType.Binary, featureDelimiter: ' ', isSortRequiredForFeatures: false);
            var goldClasses_train = headers[gold_i];

            ConfusionMatrix confusionMatrix;
            ProgramOutput.GenerateSysOutputForVectors(sys_output, FileCreationMode.CreateNew, "test data", classifier, vectors, classToClassId, out confusionMatrix, gold_i);
            ProgramOutput.ReportAccuracy(confusionMatrix, classToClassId, "Test");
        }
    }
}
