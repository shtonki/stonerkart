using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    enum StacksWords
    {
        Stunned,
        Crippled,
    }

    class StacksCounter
    {
        private static int wordscount { get; }= Enum.GetValues(typeof(StacksWords)).Cast<StacksWords>().Count();

        private int[] counts;
        public int[] Counts => (int[])counts.Clone();


        public int this[StacksWords ix]
        {
            get { return counts[(int)ix]; }
            set { counts[(int)ix] = value; }
        }

        public StacksCounter()
        {
            counts = new int[wordscount];
        }

        public void TickDown()
        {
            for (int i = 0; i < counts.Length; i++)
            {
                if (counts[i] > 0)
                {
                    counts[i] = counts[i] - 1;
                }
            }
        }

    }
}
