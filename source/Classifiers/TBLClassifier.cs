using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ericmclachlan.Portfolio
{
    public class TBLClassifier : Classifier
    {
        // Properties

        /// <summary>The TBL Classifier will stop when the net_gain of the best transformation for the current iteration is less than min_gain.</summary>
        public readonly int MinGain;
        private readonly int Gold_i;

        public readonly List<Transformation> _transformations;
        public IList<Transformation> Transformations { get { return _transformations; } }

        public readonly int _defaultClass;
        public int DefaultClass { get { return _defaultClass; } }


        // Construction

        public TBLClassifier(List<FeatureVector> trainingVectors, int noOfClasses, int minGain, int gold_i) 
            : base(trainingVectors, noOfClasses)
        {
            // Nothing else needs to be done for now.
            _transformations = new List<Transformation>();
            _defaultClass = 0;
            MinGain = minGain;
            Gold_i = gold_i;
        }

        public TBLClassifier(List<Transformation> transformations, int noOfClasses, int defaultClass, int gold_i)
            : base(null, noOfClasses)
        {
            _transformations = transformations;
            _defaultClass = defaultClass;
            Gold_i = gold_i;
        }


        // Methods

        protected override void Train()
        {
            int[] sysClasses = ApplyInitialCategorization(TrainingVectors.ToArray());
            // Keep improving with each iteration; until the improvement is less than the MinGain threshold.
            while (true)
            {
                var possibleTs = GetPossibleTransformations(sysClasses);
                Transformation bestT = FindBestTransformation(possibleTs);
                // Stop improving until the improvement is less than the MinGain threshold.
                if (bestT.NetGain < MinGain)
                    break;
                _transformations.Add(bestT);
                Transform(sysClasses, bestT);
            }
        }

        public void Transform(int[] sysClasses, Transformation transformation)
        {
            for (int v_i = 0; v_i < TrainingVectors.Count; v_i++)
            {
                sysClasses[v_i] = Transform(sysClasses[v_i], transformation, TrainingVectors[v_i]);
            }
        }

        public int Transform(int currentClass, Transformation transformation, FeatureVector vector)
        {
            if (currentClass == transformation.FromClass
                && vector.UsedFeatures.Contains(transformation.FeatureId))
            {
                currentClass = transformation.ToClass;
            }
            return currentClass;
        }

        private Dictionary<Transformation, Transformation> GetPossibleTransformations(int[] sysClasses)
        {
            var possibleTs = new Dictionary<Transformation, Transformation>();
            // The idea here is to simultaneously calculate the effect of all relevant transformations on the training vectors.
            // If there is a net gain resulting from a transformation, then we will consider keeping it.
            // In fact, the transformation that causes the largest net gain will be added to our model.

            // More specifically, this algorithm proposes a change, e.g. f1: c1 -> c2.
            // This change is applied to all vectors with feature f1.
            // If the class WAS c1 (correct) and it is changed to c2 (incorrect), we need to decreate the net gain.
            // If the class WAS c2 (incorrecy) and it is changed to c1 (correct), we need to increase the net gain.

            for (int v_i = 0; v_i < TrainingVectors.Count; v_i++)
            {
                int fromClass = sysClasses[v_i];
                foreach (int f_i in TrainingVectors[v_i].UsedFeatures)
                {
                    for (int toClass = 0; toClass < NoOfClasses; toClass++)
                    {
                        // If the change will not affect the classification, then we can ignore it.
                        if (fromClass == toClass)
                            continue;

                        Transformation t = new Transformation(f_i, fromClass, toClass, 0);
                        if (!possibleTs.ContainsKey(t))
                            possibleTs[t] = t;

                        if (toClass == TrainingVectors[v_i].Headers[Gold_i])
                            possibleTs[t].NetGain++;
                        if (fromClass == TrainingVectors[v_i].Headers[Gold_i])
                            possibleTs[t].NetGain--;
                    }
                }
            }
            return possibleTs;
        }

        private Transformation FindBestTransformation(Dictionary<Transformation, Transformation> possibleTs)
        {
            // Evaluate each combination of (feature, from_class, to_class) to find the combination with the highest net gain.
            Transformation bestT = possibleTs.Keys.First();
            Debug.Assert(bestT != null);
            foreach (Transformation currentT in possibleTs.Keys)
            {
                if (possibleTs[currentT].NetGain > bestT.NetGain)
                {
                    bestT = currentT;
                }
            }
            return bestT;
        }

        protected override double[] Test(FeatureVector vector)
        {
            int sysClass = _defaultClass;
            foreach (Transformation t in _transformations)
            {
                sysClass = Transform(sysClass, t, vector);
            }
            double[] results = new double[NoOfClasses];
            results[sysClass] = 1D;
            return results;
        }

        private int[] ApplyInitialCategorization(params FeatureVector[] vectors)
        {
            // Dumb baseline: Defaults the classification for all vectors to the first class (classId = 0).
            int[] sysClasses = new int[vectors.Length];
            for (int i = 0; i < sysClasses.Length; i++)
            {
                sysClasses[i] = _defaultClass;
            }
            return sysClasses;
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
        public static TBLClassifier LoadModel(string model_file, ValueIdMapper<string> classToClassId, ValueIdMapper<string> featureToFeatureId, int N, int gold_id)
        {
            int defaultClass;
            List<Transformation> transformations = new List<Transformation>();
            using (StreamReader sr = File.OpenText(model_file))
            {
                // Read the default class, which is presented in the first line:
                string line = sr.ReadLine();
                defaultClass = classToClassId[line.Trim()];
                // Read each of the transitions stored in the model file:
                Regex parser = new Regex(@"(?<featName>[^\s]+)\s+(?<from_class>[^\s]+)\s+(?<to_class>[^\s]+)\s+(?<net_gain>[^\s]+)");
                while (!sr.EndOfStream && transformations.Count < N)
                {
                    line = sr.ReadLine();
                    var match = parser.Match(line);

                    int feat_id = featureToFeatureId[match.Groups["featName"].Value.Trim()];
                    int from_class_id = classToClassId[match.Groups["from_class"].Value.Trim()];
                    int to_class_id = classToClassId[match.Groups["to_class"].Value.Trim()];
                    int net_gain = int.Parse(match.Groups["net_gain"].Value.Trim());
                    transformations.Add(new Transformation(feat_id, from_class_id, to_class_id, net_gain));
                }
            }
            // Set the minimum gain to -1 (an invalid value) to indicate that the model has been loaded.
            return new TBLClassifier(transformations, classToClassId.Count, defaultClass, gold_i);
        }

        /// <summary>
        /// This struct represents a single transformation for a particular feature (<c>FeatureId</c>), 
        /// from a particular class (<c>FromClass</c>) to a particular class (<c>ToClass</c>).
        /// </summary>
        public class Transformation
        {
            // Properties

            public readonly int FeatureId;

            public readonly int FromClass;

            public readonly int ToClass;

            public int NetGain { get; set; } = 0;

            private readonly string _text;
            private readonly int _hashcode;

            // Construction

            public Transformation(int featureId, int fromClass, int toClass, int net_gain)
            {
                FeatureId = featureId;
                FromClass = fromClass;
                ToClass = toClass;
                NetGain = net_gain;

                _text = string.Format($"{FeatureId}:{FromClass}:{ToClass}");
                _hashcode = _text.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                Transformation that;
                if (obj == null || (that = (obj as Transformation)) == null)
                    return false;
                if (ReferenceEquals(this, that))
                    return true;
                return this.FeatureId == that.FeatureId 
                    && this.FromClass == that.FromClass 
                    && this.ToClass == that.ToClass;
            }

            public override string ToString()
            {
                return _text;
            }

            public override int GetHashCode()
            {
                return _hashcode;
            }
        }
    }
}
