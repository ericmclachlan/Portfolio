using System.Collections.Generic;
using System.Text;

namespace ericmclachlan.Portfolio
{
    /// <summary>
    /// <para>
    /// This command will classify the specified <c>vector_file</c> using Transition Based Learning (TBL).
    /// </para>
    /// <para>
    /// This command will be called like this: TBL_classify.sh <c>vector_file</c> <c>model_file</c> <c>sys_output</c> <c>N</c>
    /// </para>
    /// </summary>
    public class Command_TBL_classify : ICommand
    {
        // Properties

        public string CommandName { get { return "TBL_classify"; } }

        /// <summary>Vector file in text format. (You could run the classifier against either a test or training file.)</summary>
        [CommandParameter(Index = 0, Type = CommandParameterType.InputFile, Description = "Vector file in text format. (You could run the classifier against either a test or training file.)")]
        public string vector_file { get; set; }

        /// <summary>This input file contains a serialization of the TBL model.</summary>
        [CommandParameter(Index = 1, Type = CommandParameterType.InputFile, Description = "This input file contains a serialization of the TBL model.")]
        public string model_file { get; set; }

        /// <summary>This output file contains the classification results for the test_data.</summary>
        [CommandParameter(Index = 2, Description = "This output file contains the classification results for the test_data.")]
        public string sys_output { get; set; }

        /// <summary>N is the number of transformations (specified in the model_file) that will be performed during classification.</summary>
        [CommandParameter(Index = 3, Type = CommandParameterType.PositiveInteger, Description = "N is the number of transformations (specified in the model_file) that will be performed during classification.")]
        public int N { get; set; }


        // Methods

        public void ExecuteCommand()
        {
            int gold_i = 0;

            FeatureVectorFile vectorFile = new FeatureVectorFile(path: vector_file, noOfHeaderColumns: 1, featureDelimiter: ' ', isSortRequired: false);
            
            Program.ReportOnModel(vectorFile, sys_output
                , classifierFactory: (classToClassId, featureToFeatureId) =>
                {
                    return TBLClassifier.LoadModel(model_file, classToClassId, featureToFeatureId, N, gold_i);
                }
                , getDetailsFunc: GetDetails
            );
        }

        private static string[] GetDetails(Classifier classifier, List<FeatureVector> vectors, TextIdMapper classToClassId, TextIdMapper featureToFeatureId)
        {
            TBLClassifier tblClassifier = (TBLClassifier)classifier;
            var systemClasses = new int[vectors.Count];
            var details = new string[vectors.Count];
            for (int v_i = 0; v_i < vectors.Count; v_i++)
            {
                StringBuilder sb = new StringBuilder();
                int currentClass = tblClassifier.DefaultClass;
                foreach (TBLClassifier.Transformation t in tblClassifier.Transformations)
                {
                    int newClass = tblClassifier.Transform(currentClass, t, vectors[v_i]);
                    if (newClass == currentClass)
                        continue;

                    string featName = featureToFeatureId[t.FeatureId];
                    string from_class = classToClassId[t.FromClass];
                    string to_class = classToClassId[t.ToClass];
                    sb.AppendFormat($" {featName} {from_class} {to_class}");
                    currentClass = newClass;
                }
                systemClasses[v_i] = currentClass;
                details[v_i] = sb.ToString();
            }
            return details;
        }
    }
}
