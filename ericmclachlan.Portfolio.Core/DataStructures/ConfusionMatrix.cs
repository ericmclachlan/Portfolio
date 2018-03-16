namespace ericmclachlan.Portfolio.Core
{
    public class ConfusionMatrix
    {
        // Members

        /// <summary>Values in the confusion matrix are accessible through this indexer.</summary>
        /// <param name="gold">The gold (true or given) value.</param>
        /// <param name="system">The value predicted by the system.</param>
        public int this[int gold, int system]
        {
            get { return confusionMatrix[gold, system]; }
            set { confusionMatrix[gold,system] = value; }
        }

        /// <summary>The number of dimensions for this confusion matrix.</summary>
        public readonly int NoOfDimensions;

        private readonly int[,] confusionMatrix;


        // Construction

        public ConfusionMatrix(int noOfDimensions)
        {
            NoOfDimensions = noOfDimensions;
            confusionMatrix = new int[noOfDimensions, noOfDimensions];
        }


        // Methods

        /// <summary>Returns the accuracy for the specified confusion matrix.</summary>
        public double CalculateAccuracy()
        {
            return CalculateAccuracy(this);
        }


        /// <summary>Returns the accuracy for the specified confusion matrix.</summary>
        private static double CalculateAccuracy(ConfusionMatrix confusionMatrix)
        {
            int correct_sum = 0;
            int all_sum = 0;
            for (int i = 0; i < confusionMatrix.NoOfDimensions; i++)
            {
                for (int j = 0; j < confusionMatrix.NoOfDimensions; j++)
                {
                    if (i == j)
                        correct_sum += confusionMatrix[i, j];
                    all_sum += confusionMatrix[i, j];
                }
            }
            return ((double)correct_sum) / all_sum;
        }
    }
}
