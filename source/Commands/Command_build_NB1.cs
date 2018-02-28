using System;
using System.Collections.Generic;
using System.IO;

namespace ericmclachlan.Portfolio
{
    internal class Command_build_NB1 : ICommand
    {
        // Members

        public string CommandName { get { return "build_NB1"; } }

        [CommandParameter(Index = 0, Type = CommandParameterType.InputFile, Description = "Vector file in text format(cf.train.vectors.txt).")]
        public string training_data_file { get; set; }

        [CommandParameter(Index = 1, Type = CommandParameterType.InputFile, Description = "Vector file in text format (cf. test.vectors.txt).")]
        public string test_data_file { get; set; }

        [CommandParameter(Index = 2, Description = "The δ used in add-δ smoothing when calculating the class prior P(c).")]
        public double class_prior_delta { get; set; }

        [CommandParameter(Index = 3, Description = "The δ used in add-δ smoothing when calculating the conditional probability P(f|c).")]
        public double cond_prob_delta { get; set; }

        [CommandParameter(Index = 4, Description = "Stores the values of P(c) and P(f|c) (cf. model1).")]
        public string model_file { get; set; }

        [CommandParameter(Index = 5, Description = "The classification result on the training and test data (cf. sys1).")]
        public string sys_output { get; set; }
        

        // Methods

        public void ExecuteCommand()
        {
            int noOfHeadersColumns = 1;
            int gold_i = 0;
            ValueIdMapper<string> featureToFeatureId;
            ValueIdMapper<string>[] headerToHeaderIds;
            ValueIdMapper<string> classToClassId;
            Program.CreateValueIdMappers(noOfHeadersColumns, gold_i, out featureToFeatureId, out headerToHeaderIds, out classToClassId);

            int[][] headers;
            var trainingVectors = FeatureVector.LoadFromSVMLight(training_data_file, featureToFeatureId, headerToHeaderIds, noOfHeadersColumns, out headers, FeatureType.Binary, featureDelimiter:' ' , isSortRequiredForFeatures: false);
            var goldClasses_train = headers[gold_i];

            var testVectors = FeatureVector.LoadFromSVMLight(test_data_file, featureToFeatureId, headerToHeaderIds, noOfHeadersColumns, out headers, FeatureType.Binary, featureDelimiter:' ' , isSortRequiredForFeatures: false);
            var goldClasses_test = headers[gold_i];

            Classifier classifier = new NaiveBayesClassifier_MultivariateBernoulli(class_prior_delta, cond_prob_delta, trainingVectors, classToClassId.Count, gold_i);

            ConfusionMatrix confusionMatrix_training;
            ProgramOutput.GenerateSysOutputForVectors(sys_output, FileCreationMode.CreateNew, "training data", classifier, trainingVectors, classToClassId, out confusionMatrix_training, gold_i);
            ProgramOutput.ReportAccuracy(confusionMatrix_training, classToClassId, sys_output);
            ConfusionMatrix confusionMatrix_test;
            ProgramOutput.GenerateSysOutputForVectors(sys_output, FileCreationMode.Append, "test data", classifier, testVectors, classToClassId, out confusionMatrix_test, gold_i);
            ProgramOutput.ReportAccuracy(confusionMatrix_test, classToClassId, "Test");
        }
    }
}
