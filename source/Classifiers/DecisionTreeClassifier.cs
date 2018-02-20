using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ericmclachlan.Portfolio
{
    public class DecisionTreeClassifier : Classifier
    {
        // Members

        internal DecisionTreeNode Root { get; private set; }

        private int _maxDepth;
        private double _minGain;


        // Construction

        public DecisionTreeClassifier(List<FeatureVector> trainingVectors, int noOfClasses, int maxDepth, double minGain)
            : base(trainingVectors, noOfClasses)
        {
            _maxDepth = maxDepth;
            _minGain = minGain;
        }


        // Public Methods

        protected override void Train()
        {
            Root = BuildDecisionTree_Recursive(NoOfClasses, TrainingVectors, 0, _maxDepth, _minGain);
        }

        protected override double[] Test(FeatureVector vector)
        {
            var leaf = Root.FindLeaf(vector);
            return leaf.GetDistributionByClass();
        }

        private static DecisionTreeNode BuildDecisionTree_Recursive(int noOfCategories, List<FeatureVector> vectors, int depth, int maxDepth, double minGain)
        {
            // Initialize our list of training instances. This collection will be split at each node.
            int bestFeature;
            double informationGain;
            List<FeatureVector>[] vectors_splitByFeature;
            FindFeatureWithMaxInformationGain(vectors, noOfCategories, out bestFeature, out informationGain, out vectors_splitByFeature);
            DecisionTreeNode node = new DecisionTreeNode(bestFeature, vectors, noOfCategories);
            //Debug.Assert(vectors_splitByFeature.Length == 2);
            if ((minGain != 0 && informationGain < minGain) || Math.Abs(informationGain) < StatisticsHelper.Significance)
                return node;

            // TODO: Make this less binary dependent.
            if (depth < maxDepth)
            {
                if (vectors_splitByFeature[0].Count > 0)
                {
                    node.FalseBranch = BuildDecisionTree_Recursive(noOfCategories, vectors_splitByFeature[0], depth + 1, maxDepth, minGain);
                }
                if (vectors_splitByFeature[1].Count > 0)
                {
                    node.TrueBranch = BuildDecisionTree_Recursive(noOfCategories, vectors_splitByFeature[1], depth + 1, maxDepth, minGain);
                }
            }
            return node;
        }

        private static void FindFeatureWithMaxInformationGain(
            List<FeatureVector> vectors
            , int noOfCategories
            , out int bestFeature
            , out double maxInformationGain
            , out List<FeatureVector>[] bestSplit)
        {
            // Input Validation:
            if (vectors.Count <= 0)
            {
                throw new ArgumentOutOfRangeException("vector", "Parameter is expected to have at least one training vector.");
            }

            int noOfFeatures = vectors[0].AllFeatures.Length;
            //Debug.Assert(noOfFeatures >= 0);

            // Initialize Output:
            bestFeature = -1;
            maxInformationGain = 0;
            bestSplit = null;

            // Iterate over each of the features, ...
            for (int featureIndex = 0; featureIndex < noOfFeatures; featureIndex++)
            {
                int count_t = 0;
                int count_f = 0;
                foreach (FeatureVector vector in vectors)
                {
                    if (vector.AllFeatures[featureIndex] > 0)
                        count_t++;
                    else
                        count_f++;
                }

                // Group the vectors by class.
                List<FeatureVector>[] vectors_byClass = new List<FeatureVector>[noOfCategories];
                int[][] distribution_byClass = new int[noOfCategories][];
                //TODO: It would be nice to make this less binary-feature dependent.
                List<FeatureVector>[] split = new List<FeatureVector>[2];
                split[0] = new List<FeatureVector>();
                split[1] = new List<FeatureVector>();
                for (int i = 0; i < noOfCategories; i++)
                {
                    vectors_byClass[i] = new List<FeatureVector>();
                    distribution_byClass[i] = new int[2];
                }
                // Iterate over each of the training vectors and add the vector to the relevant group BY CATEGORY
                // AND split the data BY FEATURE.
                foreach (FeatureVector vector in vectors)
                {
                    vectors_byClass[vector.GoldClass].Add(vector);
                    //TODO: It would be nice to make this less binary-feature dependent.
                    if (vector.AllFeatures[featureIndex] > 0)
                    {
                        split[1].Add(vector);
                        distribution_byClass[vector.GoldClass][1]++;
                    }
                    else
                    {
                        split[0].Add(vector);
                        distribution_byClass[vector.GoldClass][0]++;
                    }
                }
                double informationGain = StatisticsHelper.CalculateInformationGain(distribution_byClass);
                if (bestFeature == -1 || informationGain > maxInformationGain)
                {
                    maxInformationGain = informationGain;
                    bestFeature = featureIndex;
                    bestSplit = split;
                }
            }
        }


        // Inner classes

        public class DecisionTreeNode
        {
            public DecisionTreeNode Parent { get; private set; }

            public readonly int f_i;
            public readonly List<FeatureVector> FeatureVectors;
            private readonly int NoOfClasses;

            private DecisionTreeNode _trueBranch;
            public DecisionTreeNode TrueBranch
            {
                get { return _trueBranch; }
                set
                {
                    // The old value's parent no longer this node.
                    if (_trueBranch != null)
                    {
                        //Debug.Assert(_trueBranch.Parent == this);
                        _trueBranch.Parent = null;
                    }
                    // Update the value of this property.
                    _trueBranch = value;
                    // Update the parent of the new node.
                    if (_trueBranch != null)
                    {
                        _trueBranch.Parent = this;
                    }
                }
            }

            private DecisionTreeNode _falseBranch;
            public DecisionTreeNode FalseBranch
            {
                get { return _falseBranch; }
                set
                {
                    // The old value's parent no longer this node.
                    if (_falseBranch != null)
                    {
                        //Debug.Assert(_falseBranch.Parent == this);
                        _falseBranch.Parent = null;
                    }
                    // Update the value of this property.
                    _falseBranch = value;
                    // Update the parent of the new node.
                    if (_falseBranch != null)
                    {
                        _falseBranch.Parent = this;
                    }
                }
            }


            // Construction

            public DecisionTreeNode(int featureId, List<FeatureVector> featureVectors, int noOfClasses)
            {
                f_i = featureId;
                FeatureVectors = featureVectors;
                NoOfClasses = noOfClasses;
            }


            // Methods

            internal string GetModelAsText(ValueIdMapper<string> classToClassId, ValueIdMapper<string> wordToWordId)
            {
                /// TODO: Move a serialization method to the classifier class.
                StringBuilder sb = new StringBuilder();
                Serialize_Recursive(sb, classToClassId, wordToWordId, 0);
                return sb.ToString();
            }

            private void Serialize_Recursive(StringBuilder sb, ValueIdMapper<string> classToClassId, ValueIdMapper<string> wordToWordId, int depth)
            {
                // If is is a leaf node, ...
                if (TrueBranch == null && FalseBranch == null)
                {
                    string path = GetPath(wordToWordId);
                    sb.AppendFormat("{0} {1}", path, FeatureVectors.Count);
                    double[] distribution = GetDistributionByClass();
                    for (int i = 0; i < distribution.Length; i++)
                    {
                        sb.AppendFormat(" {0} {1}", classToClassId[i], distribution[i]);
                    }
                    sb.AppendLine();
                }
                // If it is not a leaf node, ...
                else
                {
                    if (FalseBranch != null)
                    {
                        FalseBranch.Serialize_Recursive(sb, classToClassId, wordToWordId, depth + 1);
                    }
                    if (TrueBranch != null)
                    {
                        TrueBranch.Serialize_Recursive(sb, classToClassId, wordToWordId, depth + 1);
                    }
                }
            }

            internal DecisionTreeNode FindLeaf(FeatureVector vector)
            {
                // TODO: Make this less binary dependent.
                if (vector.AllFeatures[f_i] > 0 && TrueBranch != null)
                    return TrueBranch.FindLeaf(vector);
                else if (FalseBranch != null)
                    return FalseBranch.FindLeaf(vector);
                else
                    return this;
            }

            public double[] GetDistributionByClass()
            {
                var results = from v in FeatureVectors
                              group v by v.GoldClass into g
                              orderby g.Key
                              select new { ClassId = g.Key, Count = g.Count() };
                double[] distribution = new double[NoOfClasses];
                foreach (var result in results)
                {
                    distribution[result.ClassId] = Math.Round(((double)result.Count) / FeatureVectors.Count, 3);
                }

                return distribution;
            }

            public string GetPath(ValueIdMapper<string> wordToWordId)
            {
                if (Parent == null)
                {
                    //Debug.Assert(depth == 0);
                    return string.Empty;
                }
                string featureName = wordToWordId[this.Parent.f_i];
                if (object.ReferenceEquals(this, Parent.FalseBranch))
                    featureName = "!" + featureName;
                string parentFeature = Parent.GetPath(wordToWordId);
                if (parentFeature == string.Empty)
                    return featureName;
                return parentFeature + "&" + featureName;
            }
        }
    }
}
