using System.Collections.Generic;
using System.IO;
using System.Text;

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

        /// <summary>Vector file in text format. (You could run the classifier against either a test or training file.)</summary>
        [CommandParameter(Index = 0, Type = CommandParameterType.InputFile, Description = "Vector file in text format. (You could run the classifier against either a test or training file.)")]
        public string test_file { get; set; }

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

            int noOfHeadersColumns = 1;
            ValueIdMapper<string> featureToFeatureId;
            ValueIdMapper<string>[] headerToHeaderIds;
            ValueIdMapper<string> classToClassId;
            Program.CreateValueIdMappers(noOfHeadersColumns, gold_i, out featureToFeatureId, out headerToHeaderIds, out classToClassId);

            // Load the TBL classifier:
            var classifier = TBLClassifier.LoadModel(model_file, classToClassId, featureToFeatureId, N, gold_i);

            int[][] headers;
            var vectors = FeatureVector.LoadFromSVMLight(test_file, featureToFeatureId, headerToHeaderIds, noOfHeadersColumns, out headers, FeatureType.Binary, featureDelimiter: ' ', isSortRequiredForFeatures: false);
            var goldClasses = headers[gold_i];

            // Output the result of the classification:
            int[] systemClasses;
            string[] details;
            GetSystemClassesAndTransitionDetails(vectors, classifier, classToClassId, featureToFeatureId, out systemClasses, out details);
            ProgramOutput.GenerateSysOutput(sys_output, FileCreationMode.CreateNew, vectors, classToClassId, systemClasses, details, Path.GetFileName(test_file));
        }

        private static void GetSystemClassesAndTransitionDetails(List<FeatureVector> vectors, TBLClassifier classifier, ValueIdMapper<string> classToClassId, ValueIdMapper<string> featureToFeatureId, out int[] systemClasses, out string[] details)
        {
            systemClasses = new int[vectors.Count];
            details = new string[vectors.Count];
            for (int v_i = 0; v_i < vectors.Count; v_i++)
            {
                StringBuilder sb = new StringBuilder();
                int currentClass = classifier.DefaultClass;
                foreach (TBLClassifier.Transformation t in classifier.Transformations)
                {
                    int newClass = classifier.Transform(currentClass, t, vectors[v_i]);
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
        }
    }
}
