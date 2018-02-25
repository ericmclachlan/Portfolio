using System;

namespace ericmclachlan.Portfolio
{
    /// <summary>
    /// <para>
    /// Decodes a <c>test_data</c> file using a SVM decoder.
    /// </para><para>
    /// Example: svm_classify test_data model_file sys_output
    /// </para>
    /// </summary>
    class Command_svm_classify : ICommand
    {
        public string CommandName { get { return "svm_classify"; } }

        #region Parameters

        /// <summary>Vector file in libSVM format.</summary>
        [CommandParameter(Index = 0, Type = CommandParameterType.InputFile, Description = "Vector file in libSVM format.")]
        public string test_data { get; set; }

        [CommandParameter(Index = 1, Type = CommandParameterType.InputFile, Description = "The SVM model in libSVM model format.")]
        /// <summary>The SVM model in libSVM model format. The model_file stores α_iy_i for each support vector and ρ</summary>
        public string model_file { get; set; }

        /// <summary>This output file contains the classification results for the test_data.</summary>
        [CommandParameter(Index = 2, Description = "This output file contains the classification results for test_data.")]
        public string sys_output { get; set; }

        #endregion


        public void ExecuteCommand()
        {
            ValueIdMapper<string> classToclassId;
            ValueIdMapper<string> featureToFeatureId;

            Classifier classifier = SVMClassifier.LoadModel(model_file, out classToclassId, out featureToFeatureId);

            Func<int, int> transformationF = (i) => { return 1; };
            var testVectors = FeatureVector.LoadFromSVMLight(test_data, featureToFeatureId, classToclassId, transformationF);

            ConfusionMatrix confusionMatrix;
            ProgramOutput.GenerateSysOutputForVectors(sys_output, FileCreationMode.CreateNew, "test data", classifier, testVectors, classToclassId, out confusionMatrix);
            ProgramOutput.ReportAccuracy(confusionMatrix, classToclassId, "Test");
        }
    }
}
