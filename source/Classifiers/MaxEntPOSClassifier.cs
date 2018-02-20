using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ericmclachlan.Portfolio
{
    public class MaxEntPOSClassifier : MaxEntClassifier
    {
        // Construction

        protected MaxEntPOSClassifier(
            List<FeatureVector> trainingVectors
            , int noOfClasses
            , double[] lambda_c)
            : base(trainingVectors, noOfClasses, lambda_c)
        {
            // Nothing more needs to be done.
        }


        // Methods

        protected override void Train()
        {
            // Nothing special needs to be done.
        }

        protected override double[] Test(FeatureVector vector)
        {
            double[] distribution = new double[NoOfClasses];

            // Do two things:
            // 1) Calculate the probability of a document belonging to class c_i, given document <c>vector</c>, and
            // 2) Calculate Z, which will be used to normalize the distribution shortly.
            for (int c_i = 0; c_i < NoOfClasses; c_i++)
            {
                double logProb = _lambda_c[c_i];
                for (int u_i = 0; u_i < vector.UsedFeatures.Length; u_i++)
                {
                    int f_i = vector.UsedFeatures[u_i];
                    logProb += CalculateLogProb_c_f(c_i, f_i);
                }
                distribution[c_i] = logProb;
            }
            StatisticsHelper.NormalizeLogs(distribution, Math.E);
            return distribution;
        }

        public double CalculateLogProb_c_f(int c_i, int f_i)
        {
            // Handle words in our vocabulary:
            if (f_i < TrainingVectors[c_i].AllFeatures.Length)
                return TrainingVectors[c_i].AllFeatures[f_i];
            // Handle words Out of Our Vocabulary (OOV):
            else
                return 0;   // TODO: Use smoothing to handle OOVs.
        }


        // Static Methods

        public new static MaxEntPOSClassifier LoadModel(string text, out ValueIdMapper<string> classToClassId, out ValueIdMapper<string> featureToFeatureId)
        {
            List<double> lambda_c;
            List<FeatureVector> vectors;
            LoadModel(text, out classToClassId, out featureToFeatureId, out lambda_c, out vectors);

            return new MaxEntPOSClassifier(vectors, classToClassId.Count, lambda_c.ToArray());
        }
    }
}
