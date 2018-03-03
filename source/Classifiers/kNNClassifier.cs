using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ericmclachlan.Portfolio
{
    /// <summary>Indicates the ways in which similarity can be calculated between 2 points in n-dimensional space.</summary>
    public enum SimilarityFunction
    {
        EuclideanDistance = 1,
        CosineFunction = 2,
    }

    /// <summary>A classifier based on democratic voting by the <c>K</c> Nearest Neighbors.</summary>
    public class kNNClassifier : Classifier
    {
        // Members

        private readonly int K;
        private readonly int Gold_i;
        private readonly SimilarityFunction SimilarityFunction;
        private readonly Func<FeatureVector, FeatureVector, double> distanceFunc;


        // Construction

        public kNNClassifier(int k, SimilarityFunction similarityFunction, List<FeatureVector> trainingVectors, int noOfClasses, int gold_i)
            : base(trainingVectors, noOfClasses)
        {
            K = k;
            Gold_i = gold_i;

            SimilarityFunction = similarityFunction;
            switch (SimilarityFunction)
            {
                case SimilarityFunction.EuclideanDistance: distanceFunc = CalculateAbsSquaredEuclideanDistance; break;
                case SimilarityFunction.CosineFunction: distanceFunc = CalculateCosineSimilarity; break;
                default:
                    string errorMessage = string.Format("The specified similarity function is not supported by the {0} class", this.GetType().Name);
                    throw new NotImplementedException(errorMessage);
            }
        }


        // Methods

        protected override void Train()
        {
            // Cache some results.
            for (int v_i = 0; v_i < TrainingVectors.Count; v_i++)
            {
                FeatureVector v = TrainingVectors[v_i];
                v.Tag = SumVectorValuesSquared(v);
            }

            // No other specific training needs to be done as almost everything is calculated during the testing phase.
        }

        protected override int Test(FeatureVector vector, out double[] details)
        {
            double[] votes_c = new double[NoOfClasses];
            var nearestNeighbors = new List<IdValuePair<double>>();
            for (int v_i = 0; v_i < TrainingVectors.Count; v_i++)
            {
                double distance = distanceFunc(TrainingVectors[v_i], vector);
                // If this neighbor is closer than our furthest neighbor, ...

                // If the list of nearest neighbors is empty OR this is the closest distance we'Ve seen, 
                // then add neighbor as the closest neighbor (i.e. insert at position 0).
                if (nearestNeighbors.Count == 0 || distance < nearestNeighbors[0].Value)
                {
                    nearestNeighbors.Insert(0, new IdValuePair<double>(v_i, distance));
                    votes_c[TrainingVectors[v_i].Headers[Gold_i]]++;
                    // If we have too many neighbors, then remove the furthest one.
                    if (nearestNeighbors.Count > K)
                    {
                        votes_c[TrainingVectors[nearestNeighbors[nearestNeighbors.Count - 1].Id].Headers[Gold_i]]--;
                        nearestNeighbors.RemoveAt(nearestNeighbors.Count - 1);
                    }
                }
                else if (nearestNeighbors.Count < K || distance < nearestNeighbors[nearestNeighbors.Count - 1].Value)
                {
                    var newNeighbor = new IdValuePair<double>(v_i, distance);
                    int insert_b = SearchHelper.BinarySearch(nearestNeighbors, newNeighbor);
                    if (insert_b <= K)
                    {
                        nearestNeighbors.Insert(insert_b, newNeighbor);
                        votes_c[TrainingVectors[v_i].Headers[Gold_i]]++;
                    }
                    // If we have too many neighbors, then remove the furthest one.
                    if (nearestNeighbors.Count > K)
                    {
                        votes_c[TrainingVectors[nearestNeighbors[nearestNeighbors.Count - 1].Id].Headers[Gold_i]]--;
                        nearestNeighbors.RemoveAt(nearestNeighbors.Count - 1);
                    }
                }
                Debug.Assert(nearestNeighbors.Count <= K);
            }
            if (nearestNeighbors.Count < K)
                Console.Error.WriteLine("Warning: K nearest neighbors could not be found.");

            details = StatisticsHelper.ConvertToDistribution(votes_c);
            return StatisticsHelper.ArgMax(details);
        }


        // Private Methods

        private static double SumVectorValuesSquared(FeatureVector v)
        {
            double valuesSquared = 0;
            for (int w_i = 0; w_i < v.UsedFeatures.Length; w_i++)
            {
                int f_i = v.UsedFeatures[w_i];
                valuesSquared += v.Features[f_i] * v.Features[f_i];
            }
            return valuesSquared;
        }

        private double CalculateCosineSimilarity(FeatureVector v1, FeatureVector v2)
        {
            double sumOfTheProducts = 0;
            foreach (int f_i in v1.FeatureIntersectionWith(v2))
            {
                // Ignore OOV.
                if (f_i >= NoOfFeatures)
                    continue;
                sumOfTheProducts += (v1.Features[f_i] * v2.Features[f_i]);
            }

            // Get the sum of each vector's values, squared.
            double sumOfV1ValuesSquared;
            if (v1.Tag == null)
                v1.Tag = sumOfV1ValuesSquared = SumVectorValuesSquared(v1);
            else
                sumOfV1ValuesSquared = (double)v1.Tag;
            double sumOfV2ValuesSquared;
            if (v2.Tag == null)
                v2.Tag = sumOfV2ValuesSquared = SumVectorValuesSquared(v2);
            else
                sumOfV2ValuesSquared = (double)v2.Tag;

            // Compute the final result.
            var result = (sumOfTheProducts * sumOfTheProducts) / (sumOfV1ValuesSquared * sumOfV2ValuesSquared);
            return -1 * result;
        }


        /// <summary>Calculates the Squared Euclidean Distance between two vectors.</summary>
        private double CalculateAbsSquaredEuclideanDistance(FeatureVector v1, FeatureVector v2)
        {
            double distance = 0;
            foreach (int f_i in v1.FeatureUnionWith(v2))
            {
                // Ignore OOV.
                if (f_i >= NoOfFeatures)
                    continue;
                double diff = (v2.Features[f_i] - v1.Features[f_i]);
                distance += (diff * diff);
            }
            return distance;
        }
    }
}
