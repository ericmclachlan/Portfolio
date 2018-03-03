using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ericmclachlan.Portfolio
{
    public class MaxEntClassifier : Classifier
    {
        public readonly double[] _lambda_c;

        // Construction

        protected MaxEntClassifier(
            List<FeatureVector> trainingVectors
            , int noOfClasses
            , double[] lambda_c)
            : base(trainingVectors, noOfClasses)
        {
            // Nothing more needs to be done.
            _lambda_c = lambda_c;
        }


        // Methods

        protected override void Train()
        {
            // Nothing special needs to be done.
        }

        protected override int Test(FeatureVector vector, out double[] details)
        {
            double[] distribution = new double[NoOfClasses];

            // Do two things:
            // 1) Calculate the probability of a document belonging to class c_i, given document <c>vector</c>, and
            // 2) Calculate Z, which will be used to normalize the distribution shortly.
            for (int c_i = 0; c_i < NoOfClasses; c_i++)
            {
                double prob = _lambda_c[c_i];
                for (int u_i = 0; u_i < vector.UsedFeatures.Length; u_i++)
                {
                    int f_i = vector.UsedFeatures[u_i];
                    prob += TrainingVectors[c_i].Features[f_i];
                }
                distribution[c_i] = Math.Pow(Math.E, prob);
            }
            StatisticsHelper.Normalize(distribution);

            details = distribution;
            return StatisticsHelper.ArgMax(details);
        }


        // Static Methods

        public static MaxEntClassifier LoadModel(string model_file, TextIdMapper classToClassId, TextIdMapper featureToFeatureId)
        {
            string text = File.ReadAllText(model_file);
            List<double> lambda_c;
            List<FeatureVector> vectors;
            LoadModel(text, classToClassId, featureToFeatureId, out lambda_c, out vectors);

            return new MaxEntClassifier(vectors, classToClassId.Count, lambda_c.ToArray());
        }

        protected static void LoadModel(string text, TextIdMapper classToClassId, TextIdMapper featureToFeatureId, out List<double> lambda_c, out List<FeatureVector> vectors)
        {
            var probability_c_uf = new Dictionary<int, Dictionary<int, double>>();
            lambda_c = new List<double>();
            int classId = -1;
            string className = null;
            Regex classNamePattern = new Regex(@"FEATURES FOR CLASS (?<className>.+)");
            Regex featurePattern = new Regex(@"(?<feature>\S+)\s+(?<probability>.+)");
            int lineNo = 0;

            foreach (var line in TextHelper.SplitOnNewline(text))
            {
                lineNo++;
                Match match = classNamePattern.Match(line);
                // Branch A: Update the class name.
                if (match.Groups.Count > 1)
                {
                    className = match.Groups["className"].Value;
                    int newClassId = classToClassId[className];

                    // If the class changes, make sure that the dictionary for it exists.
                    if (newClassId != classId)
                    {
                        if (probability_c_uf.ContainsKey(newClassId))
                            Console.Error.WriteLine("Line {0}:\t Category {1} might be listed twice.", lineNo, className);
                        else
                            probability_c_uf[newClassId] = new Dictionary<int, double>();
                    }
                    classId = newClassId;
                }
                // Branch B: Add a new feature.
                else
                {
                    Debug.Assert(classId != -1);
                    Match featureMatch = featurePattern.Match(line);
                    if (featureMatch.Groups.Count > 2)
                    {
                        string featureName = featureMatch.Groups["feature"].Value;
                        double probability = double.Parse(featureMatch.Groups["probability"].Value);

                        // Treat the default values slightly differently.
                        if (featureName == "<default>")
                        {
                            Debug.Assert(classId == lambda_c.Count);
                            lambda_c.Add(probability);
                        }
                        else
                        {
                            int featureId = featureToFeatureId[featureName];
                            // Check that the inner dictionary exists.
                            if (probability_c_uf[classId].ContainsKey(featureId))
                            {
                                Console.Error.WriteLine("Line {0}:\tFeature: {1} appears twice in category {2}.", lineNo, featureName, className);
                            }
                            probability_c_uf[classId][featureId] = probability;
                        }
                    }
                }
            }

            // Create feature vectors based on the information we've extracted.
            vectors = new List<FeatureVector>();
            foreach (int c_i in probability_c_uf.Keys)
            {
                ValueCollection features = new ValueCollection(featureToFeatureId.Count);
                foreach (int usedFeatureId in probability_c_uf[c_i].Keys)
                {
                    features[usedFeatureId] = probability_c_uf[c_i][usedFeatureId];
                }
                FeatureVector vector = new FeatureVector(new int[] { c_i }, features, probability_c_uf[c_i].Keys.ToArray(), false);
                vectors.Add(vector);
            }
        }
    }
}
