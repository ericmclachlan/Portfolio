using System.Collections.Generic;
using System.IO;
using System.Text;

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
            // Phase 1: Load the training data

            ValueIdMapper<string> featureToFeatureId = new ValueIdMapper<string>();
            ValueIdMapper<string> classToClassId = new ValueIdMapper<string>();

            string text = File.ReadAllText(training_data_file);
            List<FeatureVector> vectors = FeatureVector.LoadFromSVMLight(text, featureToFeatureId, classToClassId, (count) => { return 1; });

            // Create the Decision Tree
            DecisionTreeClassifier classifier = new DecisionTreeClassifier(vectors, classToClassId.Count, max_depth, min_gain);
            text = classifier.Root.GetModelAsText(classToClassId, featureToFeatureId);
            File.WriteAllText(model_file, text);

            // Report accuracy on training data.
            var confusionMatrix = classifier.GetConfusionMatrix(vectors);
            ProgramOutput.ReportAccuracy("training", confusionMatrix, classToClassId);

            text = File.ReadAllText(test_data_file);
            List<FeatureVector> testVectors = FeatureVector.LoadFromSVMLight(text, featureToFeatureId, classToClassId, (count) => { return 1; });

            StringBuilder sb = new StringBuilder();
            ProgramOutput.GenerateSysOutputForVectors("test", classifier, vectors, classToClassId, sb);
            File.WriteAllText(sys_output, sb.ToString());

            // Report accuracy on testing data.
            confusionMatrix = classifier.GetConfusionMatrix(testVectors);
            ProgramOutput.ReportAccuracy("test", confusionMatrix, classToClassId);
        }
    }
}
