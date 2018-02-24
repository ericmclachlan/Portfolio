using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ericmclachlan.Portfolio
{
    public class TBLClassifier : Classifier
    {
        // Properties

        public readonly int MinGain;

        public readonly List<Transformation> _transformations;


        // Construction

        public TBLClassifier(List<FeatureVector> trainingVectors, int noOfClasses, int minGain) 
            : base(trainingVectors, noOfClasses)
        {
            // Nothing else needs to be done for now.
            _transformations = new List<Transformation>();
            MinGain = minGain;
        }

        // Methods

        protected override void Train()
        {
            int[] current_class = ApplyInitialCategorization(TrainingVectors.ToArray());
            // Keep improving with each iteration; until the improvement is less than the MinGain threshold.
            while (true)
            {
                int[,,] netGains = CalculateNetGains(current_class);
                var transformation = FindMaxNetGain(netGains);
                // Stop improving until the improvement is less than the MinGain threshold.
                if (transformation.NetGain < MinGain)
                    break;
                _transformations.Add(transformation);
                Transform(current_class, transformation);
            }
        }

        internal void Transform(int[] classificationResults, Transformation transformation)
        {
            for (int v_i = 0; v_i < TrainingVectors.Count; v_i++)
            {
                if (classificationResults[v_i] == transformation.FromClass
                    && TrainingVectors[v_i].UsedFeatures.Contains(transformation.FeatureId))
                {
                    classificationResults[v_i] = transformation.ToClass;
                }
            }
        }

        private int[,,] CalculateNetGains(int[] current_class)
        {
            // The idea here is to simultaneously calculate the effect of all relevant transformations on the training vectors.
            // If there is a net gain resulting from the change, then we will consider keeping it.
            // In fact, the change that causes the greatest net gain will be added to our list of transformations.
            int[,,] netGains = new int[NoOfFeatures, NoOfClasses, NoOfClasses];
            for (int v_i = 0; v_i < TrainingVectors.Count; v_i++)
            {
                int fromClass = current_class[v_i];
                int goldClass = TrainingVectors[v_i].GoldClass;
                for (int u_i = 0; u_i < TrainingVectors[v_i].UsedFeatures.Length; u_i++)
                {
                    int f_i = TrainingVectors[v_i].UsedFeatures[u_i];
                    for (int toClass = 0; toClass < NoOfClasses; toClass++)
                    {
                        // If the change will not affect the classification, then we can ignore it.
                        if (fromClass == toClass)
                            continue;
                        else if (toClass == goldClass)
                            netGains[f_i, fromClass, toClass]++;
                        else if (fromClass == goldClass)
                            netGains[f_i, fromClass, toClass]--;
                    }
                }
            }
            return netGains;
        }

        private Transformation FindMaxNetGain(int[,,] netGains)
        {
            // Initialization:
            int best_f = -1;
            int best_from = -1;
            int best_t = -1;
            int maxNetGain = 0;
            for (int f = 0; f < TrainingVectors[0].UsedFeatures.Length; f++)
            {
                for (int from = 0; from < NoOfClasses; from++)
                {
                    for (int to = 0; to < NoOfClasses; to++)
                    {
                        if (netGains[f, from, to] > maxNetGain)
                        {
                            maxNetGain = netGains[f, from, to];
                            best_f = f;
                            best_from = from;
                            best_t = to;
                        }
                    }
                }
            }
            return new Transformation(best_f, best_from, best_t, maxNetGain);
        }

        protected override double[] Test(FeatureVector vector)
        {
            int[] classes = ApplyInitialCategorization(vector);
            return new double[NoOfClasses];
        }

        private int[] ApplyInitialCategorization(params FeatureVector[] vectors)
        {
            // Dumb baseline: Defaults the classification for all vectors to the first class (classId = 0).
            return new int[vectors.Length];
        }

        /// <summary>Saves this model to the specifiedl location.</summary>
        public void SaveModel(string model_file, ValueIdMapper<string> classToClassId, ValueIdMapper<string> featureToFeatureId)
        {
            // Make sure that training has been performed.
            if (!HasTrained)
                PerformTraining();

            using (StreamWriter sw = File.CreateText(model_file))
            {
                // The first line contains the default classname (i.e., the first class in the training data),
                string defaultClassName = classToClassId[0];
                sw.WriteLine($"{defaultClassName} ");
                // Then, write the list of transformation (1 x transformation per line),
                foreach (var transformation in _transformations)
                {
                    //   Each transformation line:   format: featName from_class to_class net_gain
                    string featName = featureToFeatureId[transformation.FeatureId];
                    string to_class = classToClassId[transformation.ToClass];
                    string from_class = classToClassId[transformation.FromClass];
                    int net_gain = transformation.NetGain;
                    sw.WriteLine($"{featName} {from_class} {to_class} {net_gain}");
                }
            }
        }

        /// <summary>Loads a TBL classidier from the model_file at the specifiedl location.</summary>
        public static TBLClassifier LoadModel(string model_file)
        {
            int noOfClasses = 3;
            // Set the minimum gain to -1 (an invalid value) to indicate that the model has been loaded.
            return new TBLClassifier(null, noOfClasses, -1);
        }

        /// <summary>
        /// This struct represents a single transformation for a particular feature (<c>FeatureId</c>), 
        /// from a particular class (<c>FromClass</c>) to a particular class (<c>ToClass</c>).
        /// </summary>
        public struct Transformation
        {
            // Properties

            public readonly int FeatureId;

            public readonly int FromClass;

            public readonly int ToClass;

            public readonly int NetGain;
            

            // Construction

            public Transformation(int featureId, int fromClass, int toClass, int netGain)
            {
                FeatureId = featureId;
                FromClass = fromClass;
                ToClass = toClass;
                NetGain = netGain;
            }
        }
    }
}
