namespace ericmclachlan.Portfolio
{
    /// <summary>
    /// <para>
    /// Decodes a <c>vector_file</c> using a MaxEnt decoder.
    /// </para>
    /// <para>
    /// This script will be used as follows: maxent_classify.sh test_data model_file sys_output > acc_file
    /// </para>
    /// </summary>
    internal class Command_maxent_classify : Command<double>
    {
        // Members

        public override string CommandName { get { return "maxent_classify"; } }

        /// <summary>Vector file in SVMLight format. (You could run the classifier against either a test or training file.)</summary>
        [CommandParameter(Index = 0, Type = CommandParameterType.InputFile, Description = "Vector file in text format. (You could run the classifier against either a test or training file.)")]
        public string vector_file { get; set; }

        /// <summary>This input file contains a serialization of the TBL model.</summary>
        [CommandParameter(Index = 1, Type = CommandParameterType.InputFile, Description = "This input file contains a serialization of the TBL model.")]
        public string model_file { get; set; }

        /// <summary>This output file contains the classification results for the test_data.</summary>
        [CommandParameter(Index = 2, Description = "This output file contains the classification results for the vector_file.")]
        public string sys_output { get; set; }


        // Methods

        public override double ExecuteCommand()
        {
            FeatureVectorFile vectorFile_train = new FeatureVectorFile(path: vector_file, noOfHeaderColumns: 1, featureDelimiter: ':', isSortRequired: false);
            double accuracy = Program.ReportOnModel(vectorFile_train, sys_output
                , classifierFactory: (classToClassId, featureToFeatureId) =>
                {
                    return MaxEntClassifier.LoadModel(model_file, classToClassId, featureToFeatureId);
                }
                , getDetailsFunc: (classifier, vectors, classToClassId, featureToFeatureId) =>
                {
                    return ProgramOutput.GetDistributionDetails(classifier, vectors, classToClassId);
                }
            );
            return accuracy;
        }
    }
}
