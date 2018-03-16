using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ericmclachlan.Portfolio.Core
{
    // TODO: Make this class abstract and create two implementing classes, such that:
    // class #1. Use dictionary as the underlying storage.
    // class #2. Use a double[] as the underlying storage.
    public class ValueCollection
    {
        private Dictionary<int, double> _storage = new Dictionary<int, double>();
        private readonly int _capacity;

        public ValueCollection(int capacity)
        {
            // Ignore the capacity to reduce the memory footprint.
            _storage = new Dictionary<int, double>();
            _capacity = capacity;
        }

        public double this[int id]
        {
            get
            {
                double val;
                if (_storage.TryGetValue(id, out val))
                    return _storage[id];
                else
                    return 0D;
            }
            set
            {
                _storage[id] = value;
            }
        }

        public int Length { get { return _capacity; } }
    }
}
