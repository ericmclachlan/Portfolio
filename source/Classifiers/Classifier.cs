using System;
using System.Collections.Generic;

namespace ericmclachlan.Portfolio
{
    public abstract class Classifier
    {
        public List<FeatureVector> TrainingVectors { get; private set; }

        protected Classifier(List<FeatureVector> trainingVectors, int noOfClasses)
        {
            #region Input Validation
            if (noOfClasses <= 0)
                throw new ArgumentOutOfRangeException("noOfClasses");
            #endregion

            TrainingVectors = trainingVectors;

            NoOfClasses = noOfClasses;

            if (trainingVectors == null)
                NoOfFeatures = 0;
            else
                NoOfFeatures = trainingVectors[0].AllFeatures.Length;
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
        public double[] GetDistribution(FeatureVector testVector)
        {
            // TODO: Have the classifier return the non-normalized values.
            // This gives consumers of this method more freedom to do post-processing.

            // If training hasn't been done yet, then perform the training now.
            if (TrainingVectors != null && !HasTrained)
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

        /// <summary>Returns the class of each of the <c>vectors</c>, as estimated by the system.</summary>
        public int[] Classify(IList<FeatureVector> vectors)
        {
            int[] systemClasses = new int[vectors.Count];
            for (int v_i = 0; v_i < vectors.Count; v_i++)
            {
                double[] distribution = GetDistribution(vectors[v_i]);
                systemClasses[v_i] = StatisticsHelper.ArgMax(distribution);
            }
            return systemClasses;
        }

        /// <summary>Classifies the specified vector and returns a distribution of the probabilities.</summary>
        protected abstract double[] Test(FeatureVector vector);
    }
}
