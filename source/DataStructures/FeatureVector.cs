using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private bool isSorted = false;

        // Construction

        public FeatureVector(int[] headers, ValueCollection features, int[] usedFeatures, bool sortUsedFeatures)
        {
            Headers = headers;
            AllFeatures = features;
            UsedFeatures = usedFeatures;

            // Sometimes, it is preferable to have the features sorted. In these cases, sort the features.
            if (sortUsedFeatures)
            {
                SortHelper.QuickSort(UsedFeatures);
                isSorted = true;
            }

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
        public string Display(TextIdMapper featureToFeatureId)
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
        
        /// <summary>Facilitates iterating over all features that are shared between this and <c>that</c>.</summary>
        public IEnumerable<int> FeatureIntersectionWith(FeatureVector that)
        {
            if (!isSorted || !that.isSorted)
                throw new Exception("The FeatureIntersectionWith(...) method can only be used a sorted feature vector.");
            int w1_i = 0;
            int w2_i = 0;
            while (w1_i < this.UsedFeatures.Length || w2_i < that.UsedFeatures.Length)
            {
                int f_i;
                if (w1_i < this.UsedFeatures.Length && w2_i < that.UsedFeatures.Length)
                {
                    if (this.UsedFeatures[w1_i] == that.UsedFeatures[w2_i])
                    {
                        f_i = this.UsedFeatures[w1_i++];
                        w2_i++;
                        yield return f_i;
                    }
                    else if (this.UsedFeatures[w1_i] < that.UsedFeatures[w2_i])
                        w1_i++;
                    else
                    {
                        //Debug.Assert(that.UsedFeatures[w2_i] < this.UsedFeatures[w1_i]);
                        w2_i++;
                    }
                }
                else if (w1_i < this.UsedFeatures.Length)
                    w1_i++;
                else
                {
                    //Debug.Assert(w2_i < that.UsedFeatures.Length);
                    w2_i++;
                }
            }
        }

        /// <summary>Facilitates iterating over all features that in either this or <c>that</c>.</summary>
        public IEnumerable<int> FeatureUnionWith(FeatureVector that)
        {
            if (!isSorted || !that.isSorted)
                throw new Exception("The FeatureUnionWith(...) method can only be used a sorted feature vector.");
            int w1_i = 0;
            int w2_i = 0;
            while (w1_i < this.UsedFeatures.Length || w2_i < that.UsedFeatures.Length)
            {
                int f_i;
                if (w1_i < this.UsedFeatures.Length && w2_i < that.UsedFeatures.Length)
                {
                    if (this.UsedFeatures[w1_i] == that.UsedFeatures[w2_i])
                    {
                        f_i = this.UsedFeatures[w1_i++];
                        w2_i++;
                    }
                    else if (this.UsedFeatures[w1_i] < that.UsedFeatures[w2_i])
                        f_i = this.UsedFeatures[w1_i++];
                    else
                    {
                        //Debug.Assert(that.UsedFeatures[w2_i] < this.UsedFeatures[w1_i]);
                        f_i = that.UsedFeatures[w2_i++];
                    }
                }
                else if (w1_i < this.UsedFeatures.Length)
                    f_i = this.UsedFeatures[w1_i++];
                else
                {
                    //Debug.Assert(w2_i < that.UsedFeatures.Length);
                    f_i = that.UsedFeatures[w2_i++];
                }
                yield return f_i;
            }
        }
    }
}
