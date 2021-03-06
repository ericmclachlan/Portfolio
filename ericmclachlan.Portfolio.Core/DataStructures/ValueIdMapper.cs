﻿using System;
using System.Collections.Generic;

namespace ericmclachlan.Portfolio.Core
{
    /// <summary>
    /// <para>
    /// This class provides a mapping between some unique value and some internal identifier for that value.
    /// </para><para>
    /// This makes it possible to do calculations using numbers only; rather than strings, etc.
    /// </para>
    /// </summary>
    public class ValueIdMapper<T>
    {
        // Private Members

        private Dictionary<T, int> _lookup = new Dictionary<T, int>();
        private int _maxId = 0;
        private List<T> _values = new List<T>();

        // Public Members

        /// <summary>Returns the internal identifier for the given value.</summary>
        public int this[T value]
        {
            get
            {
                int id;
                // If this value hasn't been registered before, register it now.
                if (!_lookup.TryGetValue(value, out id))
                {
                    id = _maxId++;
                    _lookup[value] = id;
                    _values.Add(value);
                }
                // Return the unique internal idenfifier that represents the specified value.
                return id;
            }
        }

        /// <summary>Returns the value represented by this internal identifier.</summary>
        public T this[int id]
        {
            get { return _values[id]; }
        }

        public T[] GetValues(int[] ids)
        {
            T[] values = new T[ids.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                values[i] = _values[ids[i]];
            }
            return values;
        }

        /// <summary>Returns the number of items in the collection.</summary>
        public int Count { get { return _maxId; } }
    }

    public class TextIdMapper : ValueIdMapper<string>
    {
        // Nothing else needs to be done.
    }
}
