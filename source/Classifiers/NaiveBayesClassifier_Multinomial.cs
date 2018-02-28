using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ericmclachlan.Portfolio
{
    public class NaiveBayesClassifier_Multinomial: NaiveBayesClassifier
    {
        // Members

        private double countW;
        private double[] countW_c;
        private double[] logProbOOV_c;


        // Construction

        public NaiveBayesClassifier_Multinomial(double class_prior_delta, double cond_prob_delta, List<FeatureVector> trainingVectors, int noOfClasses, int gold_i)
            : base(class_prior_delta, cond_prob_delta, trainingVectors, noOfClasses, gold_i)
        {
            // Nothing else needs to be done.
        }


        // Methods

        protected override void Train()
        {
            countW_c = new double[NoOfClasses];
            logProbOOV_c = new double[NoOfClasses];

            for (int v_i = 0; v_i < TrainingVectors.Count; v_i++)
            {
                for (int f_i = 0; f_i < TrainingVectors[v_i].AllFeatures.Length; f_i++)
                {
                    countW_c[TrainingVectors[v_i].Headers[Gold_i]] += TrainingVectors[v_i].AllFeatures[f_i];
                    countW += countW_c[TrainingVectors[v_i].Headers[Gold_i]];
                }
            }

            base.Train();

            for (int c_i = 0; c_i < NoOfClasses; c_i++)
            {
                logProbOOV_c[c_i] = Math.Log10(SmoothingHelper.GetAddDeltaProbability(0, countW_c[c_i], ConditionalProbabilityDelta, NoOfFeatures));
            }
        }

        protected override double Calculate_Prob_f_c(int c_i, int f_i)
        {
            return SmoothingHelper.GetAddDeltaProbability(count_f_c[f_i, c_i], countW_c[c_i], ConditionalProbabilityDelta, NoOfFeatures);
        }

        protected override double Calculate_Prob_c(int c_i)
        {
            return SmoothingHelper.GetAddDeltaProbability(countW_c[c_i], countW, ClassPriorDelta, NoOfClasses);
        }

        protected override double CalculateLogProb_v_c(FeatureVector vector, int c_i)
        {
            double logProb = 0;
            // Add Log(n!)
            for (int w_i = vector.UsedFeatures.Length; w_i > 1; w_i--)
            {
                logProb += Math.Log10(w_i);
            }
            // Add logProb_v_c
            for (int w_i = 0; w_i < vector.UsedFeatures.Length; w_i++)
            {
                int f_i = vector.UsedFeatures[w_i];
                if (f_i < logProb_f_c.GetLength(0))
                    logProb += (vector.AllFeatures[f_i] * logProb_f_c[f_i, c_i]);
                else
                    logProb += logProbOOV_c[c_i];
            }
            // Add logProb_c
            logProb += logProb_c[c_i];
            return logProb;
        }
    }
}
