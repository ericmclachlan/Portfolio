using System.Collections.Generic;
using System.Diagnostics;

namespace ericmclachlan.Portfolio
{
    /// <summary>
    /// <c>BeamSearch</c> is used to find a near optimal sequence by the sequences of nodes with the highest weights.
    /// </summary>
    /// <typeparam name="T">Any value that you want to be able to store in a <c>BeamNode</c>.</typeparam>
    public class BeamSearch<T>
    {
        // Members

        public readonly List<List<BeamNode<T>>> Level = new List<List<BeamNode<T>>>();
        
        private readonly BeamNode<T> _root;


        // Construction


        /// <summary>Creates a new <c>BeanSearch</c> structure.</summary>
        /// <param name="rootWeight">
        /// If you are using logarithmic numbers, set the value to 0. If you are using percentages, set this value to 1.
        /// </param>
        public BeamSearch(double rootWeight)
        {
            _root = new BeamNode<T>(this, rootWeight);
            Level.Add(new List<BeamNode<T>>() { _root });
        }


        // Methods

        /// <summary>Returns the best sequence of items stored in the Beam.</summary>
        public T[] GetBestSequence()
        {
            T[] results = new T[Level.Count - 1];
            var bestSequence = SearchHelper.GetMaxNItems(1, Level[Level.Count - 1]);
            BeamNode<T> currentLevel = Level[Level.Count - 1][bestSequence[0]];
            while (currentLevel.Depth > 0)
            {
                results[currentLevel.Depth - 1] = currentLevel.Item;
                currentLevel = currentLevel.PreviousNode;
            }
            return results;
        }

        /// <summary>
        /// Prunes the highest level of the beam to the <c>k</c> most probably optimal sequences.
        /// </summary>
        /// <param name="significantDifference">
        /// Ignore weights that <c>significantDifference</c> from the most optimal node at this level.
        /// </param>
        /// <param name="maxNodes">
        /// The max number of nodes preserved for the whole level.
        /// </param>
        internal void Prune(int maxNodes, double significantDifference)
        {
            Debug.Assert(maxNodes > 0);

            int maxLevel = Level.Count - 1;
            // Minor optimization: Early exit if pruning is not required.
            if (Level[maxLevel].Count <= maxNodes)
                return;

            // Clone the list:
            Debug.Assert(Level[maxLevel].Count > 0);
            BeamNode<T>[] items = Level[maxLevel].ToArray();
            IList<int> topKItems = SearchHelper.GetMaxNItems(maxNodes, items);
            double highestWeight = items[topKItems[0]].Weight;
            Level[maxLevel].Clear();
            foreach (var winner in topKItems)
            {
                if (items[winner].Weight  + significantDifference >= highestWeight)
                {
                    Level[maxLevel].Add(items[winner]);
                }
            }
            Debug.Assert(Level[maxLevel].Count > 0);
        }
    }
}
