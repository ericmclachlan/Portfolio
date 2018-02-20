using System;
using System.IO;

namespace ericmclachlan.Portfolio
{
    internal class Command_maxent_classify : ICommand
    {
        // Members

        public string CommandName { get { return "maxent_classify"; } }

        [CommandParameter(Index = 0, Type = CommandParameterType.InputFile)]
        public string test_data_file { get; set; }

        [CommandParameter(Index = 1, Type = CommandParameterType.InputFile)]
        public string model_file { get; set; }

        [CommandParameter(Index = 2)]
        public string sys_output { get; set; }


        // Methods

        public void ExecuteCommand()
        {
            ValueIdMapper<string> classToclassId;
            ValueIdMapper<string> featureToFeatureId;

            Classifier classifier = MaxEntClassifier.LoadModel(model_file, out classToclassId, out featureToFeatureId);

            Func<int, int> transformationF = (i) => { return 1; };
            var testVectors = FeatureVector.LoadFromSVMLight(test_data_file, featureToFeatureId, classToclassId, transformationF);

            ConfusionMatrix confusionMatrix;
            ProgramOutput.GenerateSysOutputForVectors(sys_output, FileCreationMode.CreateNew, "test data", classifier, testVectors, classToclassId, out confusionMatrix);
            ProgramOutput.ReportAccuracy(confusionMatrix, classToclassId, "Test");
        }
    }
}
