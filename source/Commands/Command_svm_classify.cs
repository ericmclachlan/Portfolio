namespace ericmclachlan.Portfolio
{
    /// <summary>
    /// <para>
    /// Decodes a <c>test_data</c> file using a SVM decoder.
    /// </para><para>
    /// Example: svm_classify test_data model_file sys_output
    /// </para>
    /// </summary>
    public class Command_svm_classify : ICommand
    {
        public string CommandName { get { return "svm_classify"; } }

        #region Parameters

        /// <summary>Vector file in libSVM format.</summary>
        [CommandParameter(Index = 0, Type = CommandParameterType.InputFile, Description = "Vector file in libSVM format.")]
        public string test_data { get; set; }

        [CommandParameter(Index = 1, Type = CommandParameterType.InputFile, Description = "The SVM model in libSVM model format.")]
        /// <summary>The SVM model in libSVM model format. The model_file stores α_iy_i for each support vector and ρ.</summary>
        public string model_file { get; set; }

        /// <summary>This output file contains the classification results for the test_data.</summary>
        [CommandParameter(Index = 2, Description = "This output file contains the classification results for test_data.")]
        public string sys_output { get; set; }

        #endregion


        public void ExecuteCommand()
        {
            int noOfHeadersColumns = 1;
            int gold_i = 0;
            ValueIdMapper<string> featureToFeatureId;
            ValueIdMapper<string>[] headerToHeaderIds;
            ValueIdMapper<string> classToClassId;
            Program.CreateValueIdMappers(noOfHeadersColumns, gold_i, out featureToFeatureId, out headerToHeaderIds, out classToClassId);

            int[][] headers;
            var vectors = FeatureVector.LoadFromSVMLight(test_data, featureToFeatureId, headerToHeaderIds, noOfHeadersColumns, out headers, FeatureType.Binary, featureDelimiter: ':', isSortRequiredForFeatures: false);
            var goldClasses = headers[gold_i];
            
            Classifier classifier = SVMClassifier.LoadModel(model_file, classToClassId, featureToFeatureId);

            ConfusionMatrix confusionMatrix;
            ProgramOutput.GenerateSysOutputForVectors(sys_output, FileCreationMode.CreateNew, "test data", classifier, vectors, classToClassId, out confusionMatrix, gold_i);
            ProgramOutput.ReportAccuracy(confusionMatrix, classToClassId, "Test");
        }
    }
}
