using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ericmclachlan.Portfolio.Tests
{
    public class TestComparable : IComparable
    {
        private int id;
        public TestComparable(int i)
        {
            this.id = i;
        }

        public int CompareTo(object that)
        {
            return this.id.CompareTo(((TestComparable)that).id);
        }
    }
}
