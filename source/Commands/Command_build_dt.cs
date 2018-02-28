using System.Collections.Generic;
using System.IO;

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
            int noOfHeadersColumns = 1;
            int gold_i = 0;
            ValueIdMapper<string> featureToFeatureId;
            ValueIdMapper<string>[] headerToHeaderIds;
            ValueIdMapper<string> classToClassId;
            Program.CreateValueIdMappers(noOfHeadersColumns, gold_i, out featureToFeatureId, out headerToHeaderIds, out classToClassId);

            // Load the training data:
            int[][] headers;
            List<FeatureVector> trainingVectors = FeatureVector.LoadFromSVMLight(training_data_file, featureToFeatureId, headerToHeaderIds, noOfHeadersColumns, out headers, FeatureType.Binary, featureDelimiter: ' ', isSortRequiredForFeatures: false);
            int[] goldClasses_train = headers[gold_i];

            // Create the Decision Tree
            DecisionTreeClassifier classifier = new DecisionTreeClassifier(trainingVectors, gold_i, classToClassId.Count, max_depth, min_gain);
            string text = classifier.Root.GetModelAsText(classToClassId, featureToFeatureId);
            File.WriteAllText(model_file, text);

            // Report accuracy on training data.
            ConfusionMatrix confusionMatrix = classifier.GetConfusionMatrix(trainingVectors, gold_i);
            ProgramOutput.ReportAccuracy(confusionMatrix, classToClassId, "training");

            // Load the test data:
            List<FeatureVector> testVectors = FeatureVector.LoadFromSVMLight(test_data_file, featureToFeatureId, headerToHeaderIds, noOfHeadersColumns, out headers, FeatureType.Binary, featureDelimiter: ' ', isSortRequiredForFeatures: false);
            int[] goldClasses_test = headers[gold_i];

            ProgramOutput.GenerateSysOutputForVectors(sys_output, FileCreationMode.CreateNew, "test", classifier, trainingVectors, classToClassId, out confusionMatrix, gold_i);
            ProgramOutput.ReportAccuracy(confusionMatrix, classToClassId, "test");
        }
    }
}
