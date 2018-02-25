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
            
            List<FeatureVector> trainingVectors = FeatureVector.LoadFromSVMLight(training_data_file, featureToFeatureId, classToClassId, FeatureType.Binary);

            // Create the Decision Tree
            DecisionTreeClassifier classifier = new DecisionTreeClassifier(trainingVectors, classToClassId.Count, max_depth, min_gain);
            string text = classifier.Root.GetModelAsText(classToClassId, featureToFeatureId);
            File.WriteAllText(model_file, text);

            // Report accuracy on training data.
            ConfusionMatrix confusionMatrix = classifier.GetConfusionMatrix(trainingVectors);
            ProgramOutput.ReportAccuracy(confusionMatrix, classToClassId, "training");
            
            List<FeatureVector> testVectors = FeatureVector.LoadFromSVMLight(test_data_file, featureToFeatureId, classToClassId, FeatureType.Binary);
            
            ProgramOutput.GenerateSysOutputForVectors(sys_output, FileCreationMode.CreateNew, "test", classifier, trainingVectors, classToClassId, out confusionMatrix);

            // Report accuracy on testing data.
            ProgramOutput.ReportAccuracy(confusionMatrix, classToClassId, "test");
        }
    }
}
