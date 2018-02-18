using System;
using System.Collections.Generic;

namespace ericmclachlan.Portfolio
{
    // TODO: Make this class abstract and create two implementing classes, such that:
    // class #1. Use dictionary as the underlying storage.
    // class #2. Use a double[] as the underlying storage.
    public class ValueCollection
    {
        private Dictionary<int, double> _storage = new Dictionary<int, double>();

        public ValueCollection(int capacity)
        {
            // Ignore the capacity to reduce the memory footprint.
            _storage = new Dictionary<int, double>();
        }

        public double this[int id]
        {
            get
            {
                double value;
                if (_storage.TryGetValue(id, out value))
                    return value;
                else
                    throw new IndexOutOfRangeException($"'{id}' is not a registered value in this collection.");
            }
            set
            {
                _storage[id] = value;
            }
        }

        public int Length { get { return _storage.Keys.Count; } }
    }
}
