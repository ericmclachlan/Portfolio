using System;
using System.Collections.Generic;
using System.IO;

namespace ericmclachlan.Portfolio
{
    /// <summary>This class contains the entry-point to this application.</summary>
    public class Program
    {
        // Public Methods

        /// <summary>This method is the entry-point to this application.</summary>
        public static void Main(string[] args)
        {
            try
            {
                PerformanceHelper.Measure("\tTotal Execution Time", () =>
                {
                    CommandPlatform.Execute(args);
                });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Trains the classifier provided by the <c>classifierFactory</c> on the <c>training_data</c>.
        /// Then, the classifier is used to evaluate the accuracy of both the <c>training_data</c> and <c>test_data</c>.
        /// A report on the classification details is printed to the <c>output_file</c>.
        /// </summary>
        /// <param name="output_file">A report on the classification details.</param>
        /// <param name="classifierFactory">Provides the necessary classifier.</param>
        internal static void ReportOnTrainingAndTesting(
            FeatureVectorFile vectorFile_train
            , FeatureVectorFile vectorFile_test
            , string output_file
            , Func<List<FeatureVector>, int, int, Classifier> classifierFactory
            , Func<Classifier, List<FeatureVector>, TextIdMapper, string[]> getDetailsFunc
            )
        {
            int gold_i = 0;
            TextIdMapper featureToFeatureId = new TextIdMapper();
            TextIdMapper classToClassId = new TextIdMapper();
            TextIdMapper[] headerToHeaderIds = new TextIdMapper[] { classToClassId };

            var trainingVectors = vectorFile_train.LoadFromSVMLight(featureToFeatureId, headerToHeaderIds, FeatureType.Binary);
            var goldClasses_train = vectorFile_train.Headers[gold_i];

            var testVectors = vectorFile_test.LoadFromSVMLight(featureToFeatureId, headerToHeaderIds, FeatureType.Binary);
            var goldClasses_test = vectorFile_test.Headers[gold_i];

            Classifier classifier = classifierFactory(trainingVectors, classToClassId.Count, gold_i);

            var systemClasses_train = classifier.Classify(trainingVectors);
            var systemClasses_test = classifier.Classify(testVectors);

            var details_train = ProgramOutput.GetDistributionDetails(classifier, trainingVectors, classToClassId);
            var details_test = ProgramOutput.GetDistributionDetails(classifier, testVectors, classToClassId);

            ProgramOutput.GenerateSysOutput(output_file, FileCreationMode.CreateNew, trainingVectors, classToClassId, goldClasses_train, systemClasses_train, details_train, "training data");
            ProgramOutput.GenerateSysOutput(output_file, FileCreationMode.Append, testVectors, classToClassId, goldClasses_test, systemClasses_test, details_test, "test data");
        }


        /// <summary>
        /// Loads the classifier provided by the <c>classifierFactory</c> which is modelled using the specified <c>model_file</c>.
        /// Then, the classifier is used to evaluate the accuracy of the <c>vector_data</c>.
        /// A report on the classification details is printed to the <c>output_file</c>.
        /// </summary>
        /// <param name="model_file">A file containing a serialization of the classifier model.</param>
        /// <param name="sys_output">A report on the classification details.</param>
        /// <param name="classifierFactory">Provides the necessary classifier.</param>
        internal static void ReportOnModel(
            FeatureVectorFile vectorFile
            , string sys_output
            , Func<TextIdMapper, TextIdMapper, Classifier> classifierFactory
            , Func<Classifier, List<FeatureVector>, TextIdMapper, TextIdMapper, string[]> getDetailsFunc
            )
        {
            int gold_i = 0;
            TextIdMapper featureToFeatureId = new TextIdMapper();
            TextIdMapper classToClassId = new TextIdMapper();
            TextIdMapper[] headerToHeaderIds = new TextIdMapper[] { classToClassId };

            Classifier classifier = classifierFactory(classToClassId, featureToFeatureId);
            
            var vectors = vectorFile.LoadFromSVMLight(featureToFeatureId, headerToHeaderIds, FeatureType.Binary);
            var goldClasses = vectorFile.Headers[gold_i];

            var systemClasses = classifier.Classify(vectors);

            string[] details = getDetailsFunc(classifier, vectors, classToClassId, featureToFeatureId);

            ProgramOutput.GenerateSysOutput(sys_output, FileCreationMode.CreateNew, vectors, classToClassId, goldClasses, systemClasses, details, heading: Path.GetFileName(vectorFile.Path));
        }


        /// <summary>
        /// Trains a classifier on the specified <c>train_data</c>.
        /// Output the model to the specified <c>model_file</c>.
        /// </summary>
        /// <param name="model_file">A file containing a serialization of the classifier model.</param>
        /// <param name="classifierFactory">Provides the necessary classifier, which must implement ISaveModel.</param>
        internal static void TrainModel<T>(
            FeatureVectorFile vector_file
            , string model_file
            , char featureDelimiter
            , bool isSortRequiredForFeatures
            , Func<List<FeatureVector>, TextIdMapper, TextIdMapper, T> classifierFactory
            )
            where T: Classifier, ISaveModel
        {
            TextIdMapper featureToFeatureId = new TextIdMapper();
            TextIdMapper classToClassId = new TextIdMapper();
            TextIdMapper[] headerToHeaderIds = new TextIdMapper[] { classToClassId };

            var vectors = vector_file.LoadFromSVMLight(featureToFeatureId, headerToHeaderIds, FeatureType.Binary);

            T classifier = classifierFactory(vectors, classToClassId, featureToFeatureId);

            //var systemClasses = 
                classifier.Classify(vectors);

            classifier.SaveModel(model_file, classToClassId, featureToFeatureId);
        }
    }
}
