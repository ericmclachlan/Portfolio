using System;
using System.Collections.Generic;

namespace ericmclachlan.Portfolio
{
    /// <summary>
    /// The ClassificationDetails class stores the results of classification. Both the class itself and details provided by the model.
    /// </summary>
    public class ClassificationDetails
    {
        public int Class { get; set; }
        public double[] Details { get; set; }
    }

    public abstract class Classifier
    {
        protected List<FeatureVector> TrainingVectors { get; private set; }

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
                NoOfFeatures = trainingVectors[0].Features.Length;
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

        Dictionary<string, ClassificationDetails> _cache = new Dictionary<string, ClassificationDetails>();

        /// <summary>
        /// Returns the system predicted class for the specified <c>vector</c> and any details received from the classifier.
        /// </summary>
        public int Classify(FeatureVector vector, out double[] details)
        {
            // If training hasn't been done yet, then perform the training now.
            if (!HasTrained)
                PerformTraining();

            // Check the cache.
            ClassificationDetails classDetails;
            string cacheKey = vector.ToString();
            if (_cache.TryGetValue(cacheKey, out classDetails))
            {
                details = classDetails.Details;
                return classDetails.Class;
            }

            // Calculate the distribution
            classDetails = new ClassificationDetails();
            classDetails.Class = Test(vector, out details);
            classDetails.Details = details;

            // Cache the distribution for later use.
            _cache[cacheKey] = classDetails;
            return classDetails.Class;
        }

        /// <summary>Returns the system predicted class for the specified <c>vector</c>.</summary>
        public int Classify(FeatureVector vector)
        {
            double[] details;
            return Classify(vector, out details);
        }

        /// <summary>Returns the class of each of the <c>vectors</c>, as predicted by the system.</summary>
        public int[] Classify(IList<FeatureVector> vectors)
        {
            int[] systemClasses = new int[vectors.Count];
            for (int v_i = 0; v_i < vectors.Count; v_i++)
            {
                systemClasses[v_i] = Classify(vectors[v_i]);
            }
            return systemClasses;
        }

        /// <summary>Classifies the specified vector and returns a distribution of the probabilities.</summary>
        protected abstract int Test(FeatureVector vector, out double[] details);
    }
}
