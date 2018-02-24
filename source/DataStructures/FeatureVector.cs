using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ericmclachlan.Portfolio
{
    /// <summary>A numbers-only encapsulation of training or testing data; and related metadata.</summary>
    public class FeatureVector
    {
        // Properties

        /// <summary>The given class of this document; as opposed to the predicted class.</summary>
        public readonly int GoldClass;

        /// <summary>Each value in this array stores the value of a feature, where the array's index==featureID.</summary>
        // TODO: Rename to Values. Possibly deprecate and modify UsedFeatures to provide direct access.
        public readonly ValueCollection AllFeatures;

        /// <summary>Each value in this array stores the index(==identifier) of a feature used in this document.</summary>
        public readonly int[] UsedFeatures;

        private readonly string _text;
        private readonly int _hashCode;

        /// <summary>
        /// Storage for any data a classifier wants to store for a given vector. (e.g. metadata, pre-computed values, etc)
        /// </summary>
        public object Tag { get; set; }


        // Construction

        public FeatureVector(int classId, ValueCollection values, int[] usedFeatures, bool sortUsedFeatures)
        {
            GoldClass = classId;
            AllFeatures = values;
            UsedFeatures = usedFeatures;
            if (sortUsedFeatures)
                SortHelper.QuickSort(UsedFeatures);

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

            // The following members are defined to speed up caching.
            _text = sb.ToString();
            _hashCode = _text.GetHashCode();
        }


        // Overrides

        public override string ToString()
        {
            return _text;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }


        // Methods


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

        public static List<FeatureVector> LoadFromSVMLight(
            string uri
            , ValueIdMapper<string> featureToFeatureId
            , ValueIdMapper<string> classToClassId
            , Func<int, int> transformationCount)
        {
            string text = File.ReadAllText(uri);
            // Step 1: Read the data file:
            var classes = new List<int>();
            var trainingInstances = new List<Dictionary<int, int>>();

            var lineNo = 0;
            var lines = TextHelper.SplitOnNewline(text);
            foreach (string line in lines)
            {
                lineNo++;
                var chunks = TextHelper.SplitOnWhitespace(line);

                // The first chunk contains the class:
                classes.Add(classToClassId[chunks[0]]);

                // For each of the words in the document, ...
                var wordCount = new Dictionary<int, int>();
                for (int i = 1; i < chunks.Length; i++)
                {
                    var index = chunks[i].LastIndexOf(':');
                    //Debug.Assert(index >= 0);
                    var word = chunks[i].Substring(0, index);
                    int noOfReferences;
                    if (!Int32.TryParse(chunks[i].Substring(index + 1, chunks[i].Length - index - 1), out noOfReferences))
                    {
                        Console.Error.WriteLine("Error: Line No {0}:   '{1}' did not meet the expected format and has been ignored.", lineNo, chunks[i]);
                        continue;
                    }
                    int num;
                    var wordId = featureToFeatureId[word];
                    if (!wordCount.TryGetValue(wordId, out num))
                    {
                        num = 0;
                    }
                    wordCount[wordId] = num + noOfReferences;
                }
                trainingInstances.Add(wordCount);
            }

            // Step 2: 
            // This array is a matrix where each row represents a class and each column represents a word in our dictionary
            // (where the dictionary itself is a dictionary of ALL words in ALL classes).
            var vectors = new List<FeatureVector>();
            //Debug.Assert(classes.Count == trainingInstances.Count);
            for (int c_i = 0; c_i < classes.Count; c_i++)
            {
                var wordCounts = trainingInstances[c_i];
                var allFeatures = new ValueCollection(featureToFeatureId.Count);
                var usedFeatures = new int[wordCounts.Keys.Count];
                int w_i = 0;
                foreach (int f_i in wordCounts.Keys)
                {
                    //Debug.Assert(f_i < allFeatures.Length);
                    allFeatures[f_i] = transformationCount(wordCounts[f_i]);
                    usedFeatures[w_i++] = f_i;
                }
                vectors.Add(new FeatureVector(classes[c_i], allFeatures, usedFeatures, true));
            }
            return vectors;
        }

        public static List<FeatureVector> FromModifiedSVMLight(
            string text
            , ValueIdMapper<string> featureToFeatureId
            , ValueIdMapper<string> classToClassId
            , Func<int, int> transformationF
            , out List<string> instances)
        {
            // Step 1: Read the data file:
            var goldClasses = new List<int>();
            var trainingInstances = new List<Dictionary<int, int>>();
            instances = new List<string>();

            var lineNo = 0;
            var lines = TextHelper.SplitOnNewline(text);
            foreach (string line in lines)
            {
                lineNo++;
                var chunks = TextHelper.SplitOnWhitespace(line);

                // The first chunk contains the class:
                instances.Add(chunks[0]);
                goldClasses.Add(classToClassId[chunks[1]]);

                // For each of the words in the document, ...
                var wordCount = new Dictionary<int, int>();
                for (int i = 2; i < chunks.Length; i += 2)
                {
                    //Debug.Assert(index >= 0);
                    var word = chunks[i];
                    int noOfReferences;
                    if (!Int32.TryParse(chunks[i + 1], out noOfReferences))
                    {
                        Console.Error.WriteLine("Error: Line No {0}:   '{1}' did not meet the expected format and has been ignored.", lineNo, chunks[i]);
                        continue;
                    }
                    int num;
                    var wordId = featureToFeatureId[word];
                    if (!wordCount.TryGetValue(wordId, out num))
                    {
                        num = 0;
                    }
                    wordCount[wordId] = num + noOfReferences;
                }
                trainingInstances.Add(wordCount);
            }

            // Step 2: 
            // This array is a matrix where each row represents a class and each column represents a word in our dictionary
            // (where the dictionary itself is a dictionary of ALL words in ALL classes).
            var vectors = new List<FeatureVector>();
            //Debug.Assert(classes.Count == trainingInstances.Count);
            for (int v_i = 0; v_i < trainingInstances.Count; v_i++)
            {
                var wordCounts = trainingInstances[v_i];
                var allFeatures = new ValueCollection(featureToFeatureId.Count);
                var usedFeatures = new int[wordCounts.Keys.Count];
                int w_i = 0;
                foreach (int f_i in wordCounts.Keys)
                {
                    //Debug.Assert(f_i < allFeatures.Length);
                    allFeatures[f_i] = transformationF(wordCounts[f_i]);
                    usedFeatures[w_i++] = f_i;
                }
                vectors.Add(new FeatureVector(goldClasses[v_i], allFeatures, usedFeatures, true));
            }
            return vectors;
        }
    }
}
