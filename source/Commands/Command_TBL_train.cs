using System;

namespace ericmclachlan.Portfolio
{
    /// <summary>
    /// <para>This command will train a TBL classifier.</para>
    /// <para>The command line is: TBL_train.sh train_data model_file min_gain.</para>
    /// </summary>
    public class Command_TBL_train : ICommand
    {
        // Properties

        public string CommandName { get { return "TBL_train"; } }

        /// <summary>Training vector file in text format.</summary>
        [CommandParameter(Index = 0, Type = CommandParameterType.InputFile, Description = "Training vector file in text format.")]
        public string train_data { get; set; }

        /// <summary>This output file contains a serialization of the TBL classifier's model.</summary>
        [CommandParameter(Index = 1, Description = "This output file contains a serialization of the TBL classifier's model.")]
        public string model_file { get; set; }

        /// <summary>The TBL Classifier will stop when the net_gain of the best transformation for the current iteration is less than min_gain.</summary>
        [CommandParameter(Index =2, Type =CommandParameterType.PositiveInteger, Description = "The TBL Classifier will stop when the net_gain of the best transformation for the current iteration is less than min_gain.")]
        public int min_gain { get; set; }


        // Methods
        
        public void ExecuteCommand()
        {
            int noOfHeadersColumns = 1;
            int gold_i = 0;
            ValueIdMapper<string> featureToFeatureId;
            ValueIdMapper<string>[] headerToHeaderIds;
            ValueIdMapper<string> classToClassId;
            Program.CreateValueIdMappers(noOfHeadersColumns, gold_i, out featureToFeatureId, out headerToHeaderIds, out classToClassId);

            int[][] headers;
            var trainingVectors = FeatureVector.LoadFromSVMLight(train_data, featureToFeatureId, headerToHeaderIds, noOfHeadersColumns, out headers, FeatureType.Binary, featureDelimiter: ' ', isSortRequiredForFeatures: false);
            var goldClasses = headers[gold_i];

            // Create a TBL classifier:
            var classifier = new TBLClassifier(trainingVectors, classToClassId.Count, min_gain, gold_i);

            // Save the TBL classifier model to the model_file:
            classifier.SaveModel(model_file, classToClassId, featureToFeatureId);
        }
    }
}
