using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ericmclachlan.Portfolio
{
    public abstract class Classifier
    {
        public List<FeatureVector> TrainingVectors { get; private set; }

        protected Classifier(List<FeatureVector> trainingVectors, int noOfClasses)
        {
            #region Input Validation
            if (trainingVectors == null)
                throw new ArgumentNullException("vectors");
            if (trainingVectors.Count <= 0)
                throw new ArgumentOutOfRangeException("vectors");
            if (trainingVectors[0].AllFeatures.Length <= 0)
                throw new ArgumentOutOfRangeException("vectors");
            if (noOfClasses <= 0)
                throw new ArgumentOutOfRangeException("noOfClasses");
            #endregion

            TrainingVectors = trainingVectors;
            NoOfFeatures = trainingVectors[0].AllFeatures.Length;
            NoOfClasses = noOfClasses;
        }

        // Properties

        public readonly int NoOfClasses;
        public readonly int NoOfFeatures;


        // Abstract Methods

        /// <summary><para>Indicates whether training has been performed or not.</para>
        /// <para>To manually trigger training, call PerformTraining().</para></summary>
        protected bool HasTrained { get; private set; }
        /// <summary>Trains the classifier on the <c>TrainingVectors</c>.</summary>
        protected abstract void Train();
        /// <summary>Manually invokes training; otherwise, it will only be performed when required.</summary>
        protected void PerformTraining()
        {
            Train();
            HasTrained = true;
        }

        Dictionary<string, double[]> _cache = new Dictionary<string, double[]>();

        /// <summary>Classifies the specified vector and returns a distribution of the probabilities.</summary>
        public double[] Classify(FeatureVector testVector)
        {
            // TODO: Have the classifier return the non-normalized values.
            // This gives consumers of this method more freedom to do post-processing.

            // If training hasn't been done yet, then perform the training now.
            if (!HasTrained)
                PerformTraining();

            double[] distribution;
            // Check the cache.
            string cacheKey = testVector.ToString();
            if (_cache.TryGetValue(cacheKey, out distribution))
                return distribution;

            // Calculate the distribution
            distribution = Test(testVector);

            // Cache the distribution for later use.
            _cache[cacheKey] = distribution;
            return distribution;
        }

        /// <summary>Classifies the specified vector and returns a distribution of the probabilities.</summary>
        protected abstract double[] Test(FeatureVector vector);


        // Methods

        /// <summary>Returns a confusion matrix for the given set of vectors.</summary>
        public ConfusionMatrix GetConfusionMatrix(List<FeatureVector> vectors)
        {
            ConfusionMatrix confusionMatrix = new ConfusionMatrix(NoOfClasses);
            for (int v_i = 0; v_i < vectors.Count; v_i++)
            {
                int systemClass = StatisticsHelper.ArgMax(Classify(vectors[v_i]));
                confusionMatrix[vectors[v_i].GoldClass, systemClass]++;
            }
            return confusionMatrix;
        }
    }
}
