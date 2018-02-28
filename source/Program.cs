using System;

namespace ericmclachlan.Portfolio
{
    /// <summary>This class contains the entry-point to this application.</summary>
    public class Program
    {
        // Public Methods

        /// <summary>This method is the entry-point to this application.</summary>
        public static void Main(string[] args)
        {
            try
            {
                PerformanceHelper.Measure("\tTotal Execution Time", () =>
                {
                    CommandPlatform.Execute(args);
                });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: {0}", ex.Message);
            }
        }

        /// <summary>Creates a set of ValueIdMapper for use with classifiers.</summary>
        internal static void CreateValueIdMappers(int noOfHeadersColumns, int gold_i, out ValueIdMapper<string> featureToFeatureId, out ValueIdMapper<string>[] headerToHeaderIds, out ValueIdMapper<string> classToClassId)
        {
            featureToFeatureId = new ValueIdMapper<string>();
            headerToHeaderIds = new ValueIdMapper<string>[noOfHeadersColumns];
            for (int i = 0; i < noOfHeadersColumns; i++)
            {
                headerToHeaderIds[i] = new ValueIdMapper<string>();
            }
            classToClassId = headerToHeaderIds[gold_i];
        }
    }
}
