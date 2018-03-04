using System;
using System.Collections.Generic;

namespace ericmclachlan.Portfolio
{
    public abstract class NaiveBayesClassifier: Classifier
    {
        // Properties

        /// <summary>The δ used in add-δ smoothing when calculating the P(c) = prob_c.</summary>
        public double ClassPriorDelta { get; set; }

        /// <summary>The δ used in add-δ smoothing when calculating the conditional probability P(f|c) = prob_f_c.</summary>
        public double ConditionalProbabilityDelta { get; set; }

        protected double[] count_c;
        protected double[] prob_c;
        protected double[] logProb_c;

        protected double[,] count_f_c;
        protected double[,] prob_f_c;
        protected double[,] logProb_f_c;

        /// <summary>The number of training instances.</summary>
        protected long count_t;

        protected readonly int Gold_i;


        // Construction

        public NaiveBayesClassifier(double class_prior_delta, double cond_prob_delta, List<FeatureVector> trainingVectors, int noOfClasses, int gold_i)
            : base(trainingVectors, noOfClasses)
        {
            ClassPriorDelta = class_prior_delta;
            ConditionalProbabilityDelta = cond_prob_delta;
            Gold_i = gold_i;
        }


        // Abstract Methods

        protected abstract double Calculate_Prob_f_c(int c_i, int f_i);
        protected abstract double Calculate_Prob_c(int c_i);
        protected abstract double CalculateLogProb_v_c(FeatureVector vector, int c_i);


        // Public Methods

        protected override void Train()
        {
            count_f_c = new double[NoOfFeatures, NoOfClasses];
            count_c = new double[NoOfClasses];
            prob_f_c = new double[NoOfFeatures, NoOfClasses];
            logProb_f_c = new double[NoOfFeatures, NoOfClasses];
            prob_c = new double[NoOfClasses];
            logProb_c = new double[NoOfClasses];

            count_t = TrainingVectors.Count;
            // Iterate over each of the vectors (i.e. documents), ...
            foreach (FeatureVector v in TrainingVectors)
            {
                // Iterate over the used features (i.e. words), ...
                for (int w_i = 0; w_i < v.UsedFeatures.Length; w_i++)
                {
                    int f_i = v.UsedFeatures[w_i];
                    //Debug.Assert(v.AllFeatures[f_i] != 0);
                    count_f_c[f_i, v.Headers[Gold_i]] += v.Features[f_i];
                }
                count_c[v.Headers[Gold_i]]++;
            }

            // Summarize the data to make calculations super-fast later.
            for (int c_i = 0; c_i < NoOfClasses; c_i++)
            {
                for (int f_i = 0; f_i < NoOfFeatures; f_i++)
                {
                    prob_f_c[f_i, c_i] = Calculate_Prob_f_c(c_i, f_i);
                    logProb_f_c[f_i, c_i] = Math.Log10(prob_f_c[f_i, c_i]);
                }
                prob_c[c_i] = Calculate_Prob_c(c_i);
                logProb_c[c_i] = Math.Log10(prob_c[c_i]);
            }
        }

        protected override int Test(FeatureVector vector, out double[] details)
        {
            // classify(v) = argmax(P(c)P(v|c)) = argmax_c(prob_c * prob_v_c)
            double[] v_logProb_c = new double[NoOfClasses];
            for (int c_i = 0; c_i < NoOfClasses; c_i++)
            {
                v_logProb_c[c_i] = CalculateLogProb_v_c(vector, c_i);
            }
            // Convert back from log to decimal format.
            NormalizationHelper.NormalizeLogs(v_logProb_c, 10);
            details = v_logProb_c;
            return StatisticsHelper.ArgMax(details);
        }
    }
}
