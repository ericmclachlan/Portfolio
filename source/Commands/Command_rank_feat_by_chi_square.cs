using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ericmclachlan.Portfolio
{
    internal class Command_rank_feat_by_chi_square : ICommand
    {
        // Public Members

        public string CommandName { get { return "rank_feat_by_chi_square"; } }


        // Private Members

        private ValueIdMapper<string> classToClassId = new ValueIdMapper<string>();
        private ValueIdMapper<string> featureToFeatureId = new ValueIdMapper<string>();


        // Methods

        public void ExecuteCommand()
        {
            var svmLight_data = Console.In.ReadToEnd();

            Console.Error.WriteLine("{0} characters of input received.", svmLight_data.Length);

            Func<int, int> transformationF = (i) => { return i > 0 ? 1 : 0; };
            string tempFile = Path.GetTempFileName();
            List<FeatureVector> vectors;
            try
            {
                File.WriteAllText(tempFile, svmLight_data);
                vectors = FeatureVector.LoadFromSVMLight(tempFile, featureToFeatureId, classToClassId, transformationF);
            }
            finally
            {
                File.Delete(tempFile);
            }
            Debug.Assert(vectors.Count > 0);

            IdValuePair<double>[] chiSquare = new IdValuePair<double>[featureToFeatureId.Count];
            //TODO: Make the implementation less binary dependent (i.e. the hardcoded 2 below).
            double[][,] contingencyTable_f = new double[featureToFeatureId.Count][,];
            for (int f_i = 0; f_i < featureToFeatureId.Count; f_i++)
            {
                // Create a contingency table for this vector.
                contingencyTable_f[f_i] = new double[classToClassId.Count, 2];
                for (int v_i = 0; v_i < vectors.Count; v_i++)
                {
                    FeatureVector v = vectors[v_i];
                    contingencyTable_f[f_i][v.GoldClass, (int)v.AllFeatures[f_i]]++;
                }
                chiSquare[f_i] = new IdValuePair<double>(f_i, StatisticsHelper.CalculateChiSquare(contingencyTable_f[f_i]));
            }
            ReportChiSquareResults(contingencyTable_f, chiSquare);
        }


        // Private Methods

        /// <summary>Reports the chi-square results for the each feature in <c>contingencyTable_f</c>.</summary>
        internal void ReportChiSquareResults(double[][,] contingencyTable_f, IdValuePair<double>[] chiSquare)
        {
            // Sort the results according to decreasing chi-square value.
            var results = from val in chiSquare
                          orderby val.Value descending
                          select val;
            // Output each the chi-square value for each feature.
            foreach (var result in results)
            {
                // Count the number of documents (input instances) that reference that feature (in all categories).
                double docFreq = 0;
                for (int c_i = 0; c_i < classToClassId.Count; c_i++)
                {
                    // The "1" below indicates where the feature (result.Id) is used by a particular class (c_i).
                    docFreq += contingencyTable_f[result.Id][c_i, 1];
                }
                Console.WriteLine("{0}\t{1:0.00000}\t{2:0.00000}", featureToFeatureId[result.Id], chiSquare[result.Id].Value, docFreq);
            }
        }
    }
}
