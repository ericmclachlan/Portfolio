using System;
using System.IO;

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
            int noOfHeadersColumns = 1;
            int gold_i = 0;
            ValueIdMapper<string> featureToFeatureId;
            ValueIdMapper<string>[] headerToHeaderIds;
            ValueIdMapper<string> classToClassId;
            Program.CreateValueIdMappers(noOfHeadersColumns, gold_i, out featureToFeatureId, out headerToHeaderIds, out classToClassId);

            int[][] headers;
            var trainingVectors = FeatureVector.LoadFromSVMLight(training_data_file, featureToFeatureId, headerToHeaderIds, noOfHeadersColumns, out headers, FeatureType.Binary, featureDelimiter: ' ', isSortRequiredForFeatures: true);
            var goldClasses_train = headers[gold_i];

            var testVectors = FeatureVector.LoadFromSVMLight(test_data_file, featureToFeatureId, headerToHeaderIds, noOfHeadersColumns, out headers, FeatureType.Binary, featureDelimiter: ' ', isSortRequiredForFeatures: true);
            var goldClasses_test = headers[gold_i];

            Classifier classifier = new kNNClassifier(k_val, (SimilarityFunction)similarity_func, trainingVectors, classToClassId.Count, gold_i);

            ConfusionMatrix confusionMatrix;
            ProgramOutput.GenerateSysOutputForVectors(sys_output, FileCreationMode.CreateNew, "training data", classifier, trainingVectors, classToClassId, out confusionMatrix, gold_i);
            ProgramOutput.ReportAccuracy(confusionMatrix, classToClassId, "Training");
            ProgramOutput.GenerateSysOutputForVectors(sys_output, FileCreationMode.Append, "test data", classifier, testVectors, classToClassId, out confusionMatrix, gold_i);
            ProgramOutput.ReportAccuracy(confusionMatrix, classToClassId, "Test");
        }
    }
}
