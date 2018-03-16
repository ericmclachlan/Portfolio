using System;

namespace ericmclachlan.Portfolio.Core
{
    /// <summary>
    /// <para>This small class encapsulates an Id-Value pair.</para>
    /// <para>This makes it easy to sort and still O(1) index the result.</para>
    /// </summary>
    /// <typeparam name="T">The type of the value being stored.</typeparam>
    public class IdValuePair<T>: IComparable, IComparable<IdValuePair<T>> where T:IComparable
    {
        // Properties

        public readonly int Id;
        public readonly T Value;

        
        // Construction

        public IdValuePair(int id, T val)
        {
            Id = id;
            Value = val;
        }

        
        // Methods

        public int CompareTo(object obj)
        {
            IdValuePair<T> that = (IdValuePair<T>)obj;
            return CompareTo(that);
        }

        public int CompareTo(IdValuePair<T> that)
        {
            return Value.CompareTo(that.Value);
        }

        public override string ToString()
        {
            return string.Format($"(Id={Id}, Value={Value.ToString()})");
        }
    }
}
