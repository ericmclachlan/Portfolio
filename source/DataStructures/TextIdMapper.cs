using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ericmclachlan.Portfolio
{
    /// <summary>
    /// Maps text to a numeral. This makes it possible to do other operations on the numbers rather than using strings.
    /// </summary>
    public class ValueIdMapper<T>
    {
        private Dictionary<T, int> _lookup = new Dictionary<T, int>();
        private int _maxId = 0;
        private List<T> _words = new List<T>();

        public int this[T word]
        {
            get
            {
                int id;
                if (!_lookup.TryGetValue(word, out id))
                {
                    id = _maxId++;
                    _lookup[word] = id;
                    _words.Add(word);
                }
                return id;
            }
        }

        public T this[int id]
        {
            get { return _words[id]; }
        }

        public int Count { get { return _maxId; } }
    }
}
