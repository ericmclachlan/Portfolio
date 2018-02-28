using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ericmclachlan.Portfolio
{
    /// <summary>
    /// The values in the a feature vector file may be <c>Binary</c> consisting of {0, 1} or <c>Continuous</c> {-infinity; infinity}.
    /// </summary>
    public enum FeatureType
    {
        Continuous,
        Binary,
    }

    /// <summary>A numbers-only encapsulation of training or testing data; and related metadata.</summary>
    public class FeatureVector
    {
        // Properties

        /// <summary>Each value in this array stores the value of a feature, where the array's index==featureID.</summary>
        // TODO: Rename to Values. Possibly deprecate and modify UsedFeatures to provide direct access.
        public readonly ValueCollection AllFeatures;

        /// <summary>Each value in this array stores the index(==identifier) of a feature used in this document.</summary>
        public readonly int[] UsedFeatures;

        public readonly int[] Headers;


        /// <summary>
        /// Storage for any data a classifier wants to store for a given vector. (e.g. metadata, pre-computed values, etc)
        /// </summary>
        public object Tag { get; set; }


        // Construction

        public FeatureVector(int[] headers, ValueCollection features, int[] usedFeatures, bool sortUsedFeatures)
        {
            Headers = headers;
            AllFeatures = features;
            UsedFeatures = usedFeatures;

            // Sometimes, it is preferable to have the features sorted. In these cases, sort the features.
            if (sortUsedFeatures)
                SortHelper.QuickSort(UsedFeatures);

            // Optimization: The text representation and hash code are cached to speed up dictionary lookups etc.
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            bool isFirst = true;
            for (int w_i = 0; w_i < UsedFeatures.Length; w_i++)
            {
                if (isFirst)
                    isFirst = false;
                else
                    sb.Append(", ");
                int f_i = UsedFeatures[w_i];
                sb.AppendFormat("{0}:{1}", f_i, AllFeatures[f_i]);
            }
            sb.AppendLine("}");
            _text = sb.ToString();
            _hashCode = _text.GetHashCode();
        }


        #region Overrides

        private readonly string _text;
        private readonly int _hashCode;

        public override string ToString()
        {
            return _text;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        #endregion


        // Public Methods

        /// <summary>
        /// Displays the vector 
        /// </summary>
        /// <param name="featureToFeatureId"></param>
        /// <returns></returns>
        public string Display(ValueIdMapper<string> featureToFeatureId)
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;
            foreach (int u_i in UsedFeatures)
            {
                if (isFirst)
                    isFirst = false;
                else
                    sb.AppendFormat(" ");
                sb.AppendFormat("{0}:{1:0.#####}", featureToFeatureId[u_i], AllFeatures[u_i]);
            }
            return sb.ToString();
        }


        // Static Methods

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
        public static List<FeatureVector> LoadFromSVMLight(
            string input_file
            , ValueIdMapper<string> featureToFeatureId
            , ValueIdMapper<string>[] headerToHeaderIds
            , int noOfHeaderColumns
            , out int[][] headers
            , FeatureType featureType
            , char featureDelimiter
            , bool isSortRequiredForFeatures)
        {
            // Step 1: Read the data file:
            string[] lines = File.ReadAllLines(input_file);

            return LoadFromSVMLight(lines, featureToFeatureId, headerToHeaderIds, noOfHeaderColumns, out headers, featureType, featureDelimiter, isSortRequiredForFeatures);
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
        public static List<FeatureVector> LoadFromSVMLight(IList<string> lines, ValueIdMapper<string> featureToFeatureId, ValueIdMapper<string>[] headerToHeaderIds, int noOfHeaderColumns, out int[][] headers, FeatureType featureType, char featureDelimiter, bool isSortRequiredForFeatures)
        {
            Debug.Assert(noOfHeaderColumns > 0);
            headers = new int[noOfHeaderColumns][];

            var wordBags_i = new List<Dictionary<int, int>>();

            // Now that we know the number of lines, we can create the arrays for storing the header columns.
            for (int j = 0; j < headers.Length; j++)
            {
                headers[j] = new int[lines.Count];
                Debug.Assert(headerToHeaderIds[j] != null);
            }

            // Parse 1: Iterate over each of the rows:
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                var chunks = TextHelper.SplitOnWhitespaceOr(line, featureDelimiter);

                // The first chunk contains the class:
                int j = 0;
                for (; j < headers.Length; j++)
                {
                    headers[j][i] = headerToHeaderIds[j][chunks[j]];
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
            for (int i = 0; i < lines.Count; i++)
            {
                var wordCounts = wordBags_i[i];
                var allFeatures = new ValueCollection(featureToFeatureId.Count);
                var usedFeatures = new int[wordCounts.Keys.Count];
                int[] headers_j = new int[noOfHeaderColumns];
                for (int j = 0; j < noOfHeaderColumns; j++)
                {
                    headers_j[j] = headers[j][i];
                }
                int w_i = 0;
                foreach (int f_i in wordCounts.Keys)
                {
                    allFeatures[f_i] = GetFeatureValue(featureType, wordCounts[f_i]);
                    usedFeatures[w_i++] = f_i;
                }
                vectors.Add(new FeatureVector(headers_j, allFeatures, usedFeatures, isSortRequiredForFeatures));
            }
            return vectors;
        }


        // Private Members

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
