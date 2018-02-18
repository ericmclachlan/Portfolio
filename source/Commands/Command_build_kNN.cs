using System;
using System.IO;
using System.Text;

namespace ericmclachlan.Portfolio
{
    /// <summary>Provides a commandline interface to the kNN Classifier.</summary>
    internal class Command_build_kNN : ICommand
    {
        // Public Members

        public string CommandName { get { return "build_kNN"; } }

        internal ValueIdMapper<string> classToclassId = new ValueIdMapper<string>();
        internal ValueIdMapper<string> featureToFeatureId = new ValueIdMapper<string>();


        // Other Members


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
            Func<int, int> transformationF = (i) => { return i; };
            var trainingVectors = FeatureVector.LoadFromSVMLight(File.ReadAllText(training_data_file), featureToFeatureId, classToclassId, transformationF);
            var testVectors = FeatureVector.LoadFromSVMLight(File.ReadAllText(test_data_file), featureToFeatureId, classToclassId, transformationF);
            Classifier classifier = new kNNClassifier(k_val, (SimilarityFunction)similarity_func, trainingVectors, classToclassId.Count);
            ProgramOutput.ReportAccuracy("Training", classifier.GetConfusionMatrix(classifier.TrainingVectors), classToclassId);
            StringBuilder sb = new StringBuilder();
            ProgramOutput.GenerateSysOutputForVectors("training data", classifier, classifier.TrainingVectors, classToclassId, sb);
            ProgramOutput.GenerateSysOutputForVectors("test data", classifier, testVectors, classToclassId, sb);
            File.WriteAllText(sys_output, sb.ToString());
            ProgramOutput.ReportAccuracy("Test", classifier.GetConfusionMatrix(testVectors), classToclassId);
        }
    }
}
