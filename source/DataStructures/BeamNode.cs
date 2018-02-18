using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ericmclachlan.Portfolio
{
    /// <summary>A node to store in a BeamSearch structure.</summary>
    /// <typeparam name="T">The type of the item to store in each node.</typeparam>
    public class BeamNode<T> : IComparable<BeamNode<T>>
    {
        // Properties

        /// <summary>The previous node in the sequence.</summary>
        public readonly BeamNode<T> PreviousNode;

        /// <summary>The weight of this node in the BeamStructure. Higher weighted nodes are given greater priority.</summary>
        public double Weight { get; set; }

        /// <summary>An item that consuming code can store in this <c>BeamNode</c>.</summary>
        public readonly T Item;

        /// <summary>Indicates the depth of this node in the <c>BeamSearch</c> structure.</summary>
        public readonly int Depth;

        /// <summary>The BeamSearch structure to which this node is added.</summary>
        private readonly BeamSearch<T> _beam;


        // Construction

        internal BeamNode(BeamSearch<T> beam, double weight)
        {
            PreviousNode = null;
            Weight = weight;
            Item = default(T);
            _beam = beam;
            Depth = 0;
        }

        /// <summary><para>Creates a new BeamNode instance.
        /// </para><para>Next, call parentNode.AddChild(...) to add this node to the appropriate level of the Beam.
        /// </para></summary>
        /// <param name="parent"></param>
        /// <param name="item"></param>
        /// <param name="weight"></param>
        public BeamNode(BeamNode<T> parent, T item, double weight)
        {
            Debug.Assert(parent != null);
            Debug.Assert(item != null);

            PreviousNode = parent;
            Item = item;
            Weight = weight;
            _beam = parent._beam;
            Depth = parent.Depth + 1;
        }


        // Methods

        /// <summary>Adds the specified <c>item</c> as the next node in the <c>BeamSearch</c> structure.</summary>
        /// <param name="weight">
        /// This is the weight of this node, which will determine which nodes are given priority by the <c>BeamSearch</c> structure.
        /// </param>
        internal void AddNextNode(T item, double weight)
        {
            var newNode = new BeamNode<T>(this, item, weight);
            if (newNode.Depth >= _beam.Level.Count)
            {
                Debug.Assert(_beam.Level.Count == newNode.Depth);
                _beam.Level.Add(new List<BeamNode<T>>());
            }
            _beam.Level[newNode.Depth].Add(newNode);
        }
        
        public int CompareTo(BeamNode<T> other)
        {
            // This method is important for comparing and ranking nodes in terms of their relative weights.
            var result = this.Weight.CompareTo(other.Weight);
            return result;
        }

        public override string ToString()
        {
            return string.Format($"Depth={Depth} Weight={Weight}");
        }
    }
}
