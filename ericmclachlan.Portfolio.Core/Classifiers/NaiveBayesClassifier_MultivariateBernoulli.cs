using System;
using System.Collections.Generic;

namespace ericmclachlan.Portfolio.Core
{
    public class NaiveBayesClassifier_MultivariateBernoulli : NaiveBayesClassifier
    {
        // Members

        private double[,] logNegProb_f_c;
        private double[] logNegativeVocabulary;
        private double[] logProbOOV_c;


        // Construction

        public NaiveBayesClassifier_MultivariateBernoulli(double class_prior_delta, double cond_prob_delta, List<FeatureVector> trainingVectors, int noOfClasses, int gold_i)
            : base(class_prior_delta, cond_prob_delta, trainingVectors, noOfClasses, gold_i)
        {
            // Nothing else needs to be done.
        }


        // Methods

        protected override void Train()
        {
            base.Train();

            logNegativeVocabulary = new double[NoOfClasses];
            logProbOOV_c = new double[NoOfClasses];
            logNegProb_f_c = new double[NoOfFeatures, NoOfClasses];

            for (int c_i = 0; c_i < NoOfClasses; c_i++)
            {
                for (int f_i = 0; f_i < NoOfFeatures; f_i++)
                {
                    logNegProb_f_c[f_i, c_i] = Math.Log10(1 - prob_f_c[f_i, c_i]);
                    logNegativeVocabulary[c_i] += logNegProb_f_c[f_i, c_i];
                }
                logProbOOV_c[c_i] = Math.Log10(SmoothingHelper.GetAddDeltaProbability(0, count_c[c_i], ConditionalProbabilityDelta, 2));
            }
        }

        protected override double Calculate_Prob_f_c(int c_i, int f_i)
        {
            // The 2 below represents 2 distributions:
            // 1. Probability of getting word f_i.
            // 2. Probability of NOT getting word f_i.
            return SmoothingHelper.GetAddDeltaProbability(count_f_c[f_i, c_i], count_c[c_i], ConditionalProbabilityDelta, 2);
        }

        protected override double Calculate_Prob_c(int c_i)
        {
            return SmoothingHelper.GetAddDeltaProbability(count_c[c_i], count_t, ClassPriorDelta, NoOfClasses);
        }

        protected override double CalculateLogProb_v_c(FeatureVector vector, int c_i)
        {
            double logProb = logNegativeVocabulary[c_i];
            for (int w_i = 0; w_i < vector.UsedFeatures.Length; w_i++)
            {
                int f_i = vector.UsedFeatures[w_i];
                if (f_i < logProb_f_c.GetLength(0))
                    logProb += logProb_f_c[f_i, c_i] - logNegProb_f_c[f_i, c_i];
                else
                    logProb += logProbOOV_c[c_i];
            }
            // Add logProb_c
            logProb += logProb_c[c_i];
            return logProb;
        }
    }
}
