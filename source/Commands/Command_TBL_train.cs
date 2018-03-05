namespace ericmclachlan.Portfolio
{
    /// <summary>
    /// <para>This command will train a TBL classifier.</para>
    /// <para>The command line is: TBL_train.sh train_data model_file min_gain.</para>
    /// </summary>
    internal class Command_TBL_train : Command<int>
    {
        // Properties

        public override string CommandName { get { return "TBL_train"; } }

        /// <summary>Training vector file in text format.</summary>
        [CommandParameter(Index = 0, Type = CommandParameterType.InputFile, Description = "Training vector file in text format.")]
        public string vector_data { get; set; }

        /// <summary>This output file contains a serialization of the TBL classifier's model.</summary>
        [CommandParameter(Index = 1, Description = "This output file contains a serialization of the TBL classifier's model.")]
        public string model_file { get; set; }

        /// <summary>The TBL Classifier will stop when the net_gain of the best transformation for the current iteration is less than min_gain.</summary>
        [CommandParameter(Index =2, Type =CommandParameterType.PositiveInteger, Description = "The TBL Classifier will stop when the net_gain of the best transformation for the current iteration is less than min_gain.")]
        public int min_gain { get; set; }


        // Methods
        
        public override int ExecuteCommand()
        {
            FeatureVectorFile vectorFile = new FeatureVectorFile(path: vector_data, noOfHeaderColumns: 1, featureDelimiter: ':', isSortRequired: false);
            int gold_i = 0;

            TBLClassifier classifier = null;
            Program.TrainModel(vectorFile, model_file, classifierFactory: (vectors, classToClassId, featureToFeatureId) =>
                {
                    return classifier = new TBLClassifier(vectors, classToClassId.Count, min_gain, gold_i);
                }
            );

            return classifier.Transformations.Count;
        }
    }
}
