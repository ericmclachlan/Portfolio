using ericmclachlan.Portfolio.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ericmclachlan.Portfolio.Tests
{
    internal static class InputProvider
    {
        // Properties

        private static int OddExhaustiveCollectionSize = 7;
        private static int EvenExhaustiveCollectionSize = 6;

        private static readonly int[] Numbers = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        private static readonly IList<IList<int>> _evenIntegers;
        private static readonly IList<IList<int>> _oddIntegers;
        private static readonly IList<int> _randomIntegers;

        // Construction

        static InputProvider()
        {
            _evenIntegers = CombinatoricsHelper.GenerateNTuples(Numbers, EvenExhaustiveCollectionSize);
            _oddIntegers = CombinatoricsHelper.GenerateNTuples(Numbers, OddExhaustiveCollectionSize);

            _randomIntegers = new int[10000000];
            Random randGen = new Random();
            for (int i = 0; i < _randomIntegers.Count; i++)
                _randomIntegers[i] = randGen.Next();
        }

        // Methods

        internal static IList<IList<int>> GetIntegerTuples_Odd()
        {
            // Return a COPY of the collection:
            List<IList<int>> results = new List<IList<int>>();
            for (int i = 0; i < _oddIntegers.Count; i++)
            {
                results.Add(_oddIntegers[i].ToArray());
            }
            return results;
        }

        internal static IList<IList<int>> GetIntegerTuples_Even()
        {
            // Return a copy of the collection:
            List<IList<int>> results = new List<IList<int>>();
            for (int i = 0; i < _evenIntegers.Count; i++)
            {
                results.Add(_evenIntegers[i].ToArray());
            }
            return results;
        }

        internal static IList<int> GetRandomIntegers_1Million()
        {
            // Return a COPY of the collection.
            return _randomIntegers.ToArray();
        }
    }
}
