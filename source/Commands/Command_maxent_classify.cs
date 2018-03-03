namespace ericmclachlan.Portfolio
{
    internal class Command_maxent_classify : Command<bool>
    {
        // Members

        public override string CommandName { get { return "maxent_classify"; } }

        [CommandParameter(Index = 0, Type = CommandParameterType.InputFile)]
        public string vector_file { get; set; }

        [CommandParameter(Index = 1, Type = CommandParameterType.InputFile)]
        public string model_file { get; set; }

        [CommandParameter(Index = 2)]
        public string sys_output { get; set; }


        // Methods

        public override bool ExecuteCommand()
        {
            FeatureVectorFile vectorFile_train = new FeatureVectorFile(path: vector_file, noOfHeaderColumns: 1, featureDelimiter: ' ', isSortRequired: false);
            Program.ReportOnModel(vectorFile_train, sys_output, classifierFactory: (classToClassId, featureToFeatureId) =>
                {
                    return MaxEntClassifier.LoadModel(model_file, classToClassId, featureToFeatureId);
                }
, getDetailsFunc: (classifier, vectors, classToClassId, featureToFeatureId) =>
                {
                    return ProgramOutput.GetDistributionDetails(classifier, vectors, classToClassId);
                }
            );
            return true;
        }
    }
}
