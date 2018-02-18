using System;
using System.IO;
using System.Text;

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

            string text = File.ReadAllText(model_file);
            Classifier classifier = MaxEntClassifier.LoadFromModel(text, out classToclassId, out featureToFeatureId);

            Func<int, int> transformationF = (i) => { return 1; };
            text = File.ReadAllText(test_data_file);
            var testVectors = FeatureVector.LoadFromSVMLight(text, featureToFeatureId, classToclassId, transformationF);

            StringBuilder sb = new StringBuilder();
            ProgramOutput.GenerateSysOutputForVectors("test data", classifier, testVectors, classToclassId, sb);
            File.WriteAllText(sys_output, sb.ToString());
            ProgramOutput.ReportAccuracy("Test", classifier.GetConfusionMatrix(testVectors), classToclassId);
        }
    }
}
