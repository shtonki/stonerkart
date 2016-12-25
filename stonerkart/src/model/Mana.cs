using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class ManaPool
    {
        public ManaSet max { get; private set; }
        public ManaSet current { get; private set; }

        public ManaPool()
        {
            max = new ManaSet();
            current = new ManaSet();
        }

        public ManaPool(ManaSet max, ManaSet current)
        {
            this.max = max;
            this.current = current;
        }

        public void reset()
        {
            for (int i = 0; i < ManaSet.size; i++)
            {
                current[i] = max[i];
            }
        }

        public void subtractCurrent(ManaSet costs)
        {
            current = current - costs;
        }

        public ManaPool clone()
        {
            return new ManaPool(max.clone(), current.clone());
        }
    }

    class ManaSet : IEnumerable<int>
    {
        public const int size = 7;
        private List<int> manas;

        public List<ManaColour> orbs => orbsEx();

        

        public ManaSet()
        {
            manas = new List<int>(new int[size]);
        }

        public ManaSet(IEnumerable<int> e)
        {
            if (e.Count() != size) throw new Exception();
            manas = new List<int>(e);
        }

        public ManaSet(IEnumerable<ManaColour> cs)
        {
            int[] vs = new int[size];
            foreach (ManaColour c in cs)
            {
                vs[(int)c]++;
            }
            manas = new List<int>(vs);
        }

        public static ManaSet operator -(ManaSet c1, ManaSet c2)
        {
            int[] a = new int[ManaSet.size];
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = c1.manas[i] - c2.manas[i];
            }
            return new ManaSet(a);
        }

        public int this[ManaColour c]
        {
            get { return manas[(int)c]; }
            set { manas[(int)c] = value; }
        }

        public int this[int i]
        {
            get { return manas[(int)i]; }
            set { manas[(int)i] = value; }
        }

        private List<ManaColour> orbsEx()
        {
            List<ManaColour> l = new List<ManaColour>();
            for (int i = 0; i < ManaSet.size; i++)
            {
                var c = (ManaColour)i;
                for (int j = 0; j < manas[i]; j++)
                {
                    l.Add(c);
                }
            }
            return l;
        }

        public IEnumerator<int> GetEnumerator()
        {
            return manas.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return manas.GetEnumerator();
        }

        public ManaSet clone()
        {
            return new ManaSet(manas);
        }
    }


    class ManaOrb : Stuff
    {
        public ManaColour colour { get; }

        public ManaOrb(ManaColour colour)
        {
            this.colour = colour;
        }
    }

    enum ManaColour
    {
        Colourless,
        Death,
        Might,
        Order,
        Life,
        Nature,
        Chaos,
        Multi,
    }
}
