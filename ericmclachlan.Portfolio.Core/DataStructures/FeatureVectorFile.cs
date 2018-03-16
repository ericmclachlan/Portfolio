using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ericmclachlan.Portfolio.Core
{
    public class FeatureVectorFile
    {
        // Properties

        public string Path { get; private set; }

        public int NoOfHeaderRows { get; set; } = 0;
        public int NoOfHeaderColumns { get; private set; }

        public char FeatureDelimiter { get; private set; }

        public int[][] Headers { get; private set; }
        public bool IsSortRequired { get; }

        public string[] HeaderRows { get; private set; }


        // Construction

        public FeatureVectorFile(string path, int noOfHeaderColumns, char featureDelimiter, bool isSortRequired)
        {
            Path = path;
            NoOfHeaderColumns = noOfHeaderColumns;
            FeatureDelimiter = featureDelimiter;
            Headers = new int[noOfHeaderColumns][];
            IsSortRequired = isSortRequired;
        }

        /// <summary>Loads and returns a collection of FeatureVectors from the specified <c>uri</c>.</summary>
        /// <param name="uri">A file, storing the features in SVM format.</param>
        /// <param name="featureToFeatureId">
        /// A mapping between the feature's text values and internal numeric identifiers that represents these value.
        /// </param>
        /// <param name="classToClassId">
        /// A mapping between class's names and internal numeric identifiers that represents these class names.
        /// </param>
        /// <param name="transformationCount"></param>
        /// <returns></returns>
        public List<FeatureVector> LoadFromSVMLight(
            TextIdMapper featureToFeatureId
            , TextIdMapper[] headerToHeaderIds
            , FeatureType featureType)
        {
            Debug.Assert(headerToHeaderIds != null && headerToHeaderIds.Length == this.NoOfHeaderColumns);

            // Step 1: Read the data file:
            string[] lines = File.ReadAllLines(this.Path);
            
            var wordBags_i = new List<Dictionary<int, int>>();

            // Now that we know the number of lines, we can create the arrays for storing the header columns.
            for (int j = 0; j < Headers.Length; j++)
            {
                Headers[j] = new int[lines.Length];
                Debug.Assert(headerToHeaderIds[j] != null);
            }

            // Store the header rows:
            HeaderRows = new string[NoOfHeaderRows];
            for (int i = 0; i < NoOfHeaderRows; i++)
            {
                HeaderRows[i] = lines[i];
            }

            // Parse 1: Iterate over each of the rows:
            for (int i = NoOfHeaderRows; i < lines.Length; i++)
            {
                string line = lines[i];
                var chunks = TextHelper.SplitOnWhitespaceOr(line, FeatureDelimiter);

                // The first chunk contains the class:
                int j = 0;
                for (; j < Headers.Length; j++)
                {
                    Headers[j][i- NoOfHeaderRows] = headerToHeaderIds[j][chunks[j]];
                }

                // For each of the words in the document, ...
                var wordToWordCount = new Dictionary<int, int>();
                for (; j < chunks.Length; j += 2)
                {
                    int count = Int32.Parse(chunks[j + 1]);
                    var featureId = featureToFeatureId[chunks[j]];
                    // Add this count to the existing sum:
                    int sum;
                    if (!wordToWordCount.TryGetValue(featureId, out sum))
                    {
                        sum = 0;
                    }
                    wordToWordCount[featureId] = sum + count;
                }
                wordBags_i.Add(wordToWordCount);
            }

            // Parse 2: 
            // This array is a matrix where each row represents a class and each column represents a word in our dictionary
            // (where the dictionary itself is a dictionary of ALL words in ALL classes).
            var vectors = new List<FeatureVector>();
            for (int i = NoOfHeaderRows; i < lines.Length; i++)
            {
                var wordCounts = wordBags_i[i - NoOfHeaderRows];
                var allFeatures = new ValueCollection(featureToFeatureId.Count);
                var usedFeatures = new int[wordCounts.Keys.Count];
                int[] headers_j = new int[NoOfHeaderColumns];
                for (int j = 0; j < NoOfHeaderColumns; j++)
                {
                    headers_j[j] = Headers[j][i - NoOfHeaderRows];
                }
                int w_i = 0;
                foreach (int f_i in wordCounts.Keys)
                {
                    allFeatures[f_i] = GetFeatureValue(featureType, wordCounts[f_i]);
                    usedFeatures[w_i++] = f_i;
                }
                vectors.Add(new FeatureVector(headers_j, allFeatures, usedFeatures, IsSortRequired));
            }
            return vectors;
        }


        // Private Methods

        /// <summary>Stores the feature value according to the specified <c>featureType</c>.</summary>
        private static int GetFeatureValue(FeatureType featureType, int value)
        {
            // TODO: Consider making this method abstract and creating two subtypes of FeatureVector such that
            // ContinuousFeatureVector and BinaryFeatureVector have different implementations for this method.
            switch (featureType)
            {
                case FeatureType.Continuous:
                    return value;
                case FeatureType.Binary:
                    return value == 0 ? 0 : 1;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
