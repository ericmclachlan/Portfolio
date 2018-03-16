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


        // Methods

        #region GetTuplesOfOddIntegers

        private static IList<IList<int>> _oddIntegers;

        internal static IList<IList<int>> GetTuplesOfOddIntegers()
        {
            // Rare case: Generate the tuples the first time they are referenced:
            if (_oddIntegers == null)
                _oddIntegers = CombinatoricsHelper.GenerateNTuples(Numbers, OddExhaustiveCollectionSize);

            // Return a COPY of the collection:
            List<IList<int>> results = new List<IList<int>>();
            for (int i = 0; i < _oddIntegers.Count; i++)
            {
                results.Add(_oddIntegers[i].ToArray());
            }
            return results;
        }

        #endregion

        #region GetTuplesOfEvenIntegers

        private static IList<IList<int>> _evenIntegers;

        internal static IList<IList<int>> GetTuplesOfEvenIntegers()
        {
            // Rare case: Generate the tuples the first time they are referenced:
            if (_evenIntegers == null)
                _evenIntegers = CombinatoricsHelper.GenerateNTuples(Numbers, EvenExhaustiveCollectionSize);

            // Return a copy of the collection:
            List<IList<int>> results = new List<IList<int>>();
            for (int i = 0; i < _evenIntegers.Count; i++)
            {
                results.Add(_evenIntegers[i].ToArray());
            }
            return results;
        }

        #endregion


        #region GetRandomIntegers_1Million

        private static IList<int> _randomIntegers;

        internal static IList<int> GetRandomIntegers_1Million()
        {
            // Rare case: Generate the tuples the first time they are referenced:
            if (_randomIntegers == null)
            {
                _randomIntegers = new int[1000000];
                Random randGen = new Random();

                for (int i = 0; i < 1000000; i++)
                {
                    _randomIntegers[i] = randGen.Next();
                }
            }

            // Return a COPY of the collection:
            return _randomIntegers.ToArray();
        }

        #endregion
    }
}
