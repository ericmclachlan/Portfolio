using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ericmclachlan.Portfolio
{
    /// <summary>
    /// <para>
    /// Decodes a <c>test_data</c> file using a SVM decoder.
    /// </para><para>
    /// This command will be called as follows: svm_classify test_data model_file sys_output
    /// </para>
    /// </summary>
    internal class Command_svm_classify : Command<double>
    {
        public override string CommandName { get { return "svm_classify"; } }

        #region Parameters

        /// <summary>Vector file in libSVM format.</summary>
        [CommandParameter(Index = 0, Type = CommandParameterType.InputFile, Description = "Vector file in libSVM format.")]
        public string vector_file { get; set; }

        [CommandParameter(Index = 1, Type = CommandParameterType.InputFile, Description = "The SVM model in libSVM model format.")]
        /// <summary>The SVM model in libSVM model format. The model_file stores α_iy_i for each support vector and ρ.</summary>
        public string model_file { get; set; }

        /// <summary>This output file contains the classification results for the test_data.</summary>
        [CommandParameter(Index = 2, Description = "This output file contains the classification results for test_data.")]
        public string sys_output { get; set; }

        #endregion


        public override double ExecuteCommand()
        {
            FeatureVectorFile vectorFile = new FeatureVectorFile(path: vector_file, noOfHeaderColumns: 1, featureDelimiter: ':', isSortRequired: true);
            FeatureVectorFile modelFile = new FeatureVectorFile(path: model_file, noOfHeaderColumns: 1, featureDelimiter: ':', isSortRequired: true);

            int alphaColumn_i = 0;
            TextIdMapper[] headerToHeaderIds_model = new TextIdMapper[modelFile.NoOfHeaderColumns];
            headerToHeaderIds_model[alphaColumn_i] = new TextIdMapper();

            var accuracy = Program.ReportOnModel(vectorFile, sys_output
                , classifierFactory: (classToClassId, featureToFeatureId) =>
                {
                    return SVMClassifier.LoadModel(modelFile, classToClassId, featureToFeatureId, alphaColumn_i, headerToHeaderIds_model);
                }
                , getDetailsFunc: GetDetails
            );
            return accuracy;
        }

        private static string[] GetDetails(Classifier classifier, List<FeatureVector> vectors, TextIdMapper classToClassId, TextIdMapper featureToFeatureId)
        {
            var detailsAsText = new string[vectors.Count];
            for (int v_i = 0; v_i < vectors.Count; v_i++)
            {
                double[] details;
                //int sysClass = 
                    classifier.Classify(vectors[v_i], out details);
                Debug.Assert(details.Length == 1);
                detailsAsText[v_i] = string.Format($"{details[0]:0.00000}");
            }
            return detailsAsText;
        }
    }
}
