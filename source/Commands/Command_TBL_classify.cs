namespace ericmclachlan.Portfolio
{
    /// <summary>
    /// <para>
    /// This command will invoke Transition Based Learning *TBL( classification.
    /// </para>
    /// <para>
    /// This command will be called like this: TBL_classify.sh test_data model_file sys_output N
    /// </para>
    /// </summary>
    public class Command_TBL_classify : ICommand
    {
        // Properties

        public string CommandName { get { return "TBL_classify"; } }

        /// <summary>Test vector file in text format.</summary>
        [CommandParameter(Index = 0, Type = CommandParameterType.InputFile, Description = "Test vector file in text format.")]
        public string test_file { get; set; }

        /// <summary>This input file contains a serialization of the TBL model.</summary>
        [CommandParameter(Index = 1, Type = CommandParameterType.InputFile, Description = "This input file contains a serialization of the TBL model.")]
        public string model_file { get; set; }

        /// <summary>This output file contains the classification results for the test_data.</summary>
        [CommandParameter(Index = 2, Description = "This output file contains the classification results for the test_data.")]
        public string sys_output { get; set; }

        /// <summary>N is the number of transformations (specified in the model_file) that will be performed during classification.</summary>
        [CommandParameter(Index = 3, Description = "N is the number of transformations (specified in the model_file) that will be performed during classification.")]
        public string N { get; set; }


        // Methods

        public void ExecuteCommand()
        {
            int gold_i = 0;

            // Load the TBL classifier:
            var classifier = TBLClassifier.LoadModel(model_file, gold_i);

            int noOfHeadersColumns = 1;
            ValueIdMapper<string> featureToFeatureId;
            ValueIdMapper<string>[] headerToHeaderIds;
            ValueIdMapper<string> classToClassId;
            Program.CreateValueIdMappers(noOfHeadersColumns, gold_i, out featureToFeatureId, out headerToHeaderIds, out classToClassId);

            int[][] headers;
            var trainingVectors = FeatureVector.LoadFromSVMLight(test_file, featureToFeatureId, headerToHeaderIds, noOfHeadersColumns, out headers, FeatureType.Binary, featureDelimiter: ' ', isSortRequiredForFeatures: false);
            var goldClasses = headers[gold_i];
            
            // TODO: Where did this implementation go?

        }
    }
}
