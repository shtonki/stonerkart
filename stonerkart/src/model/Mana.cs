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
        public IEnumerable<ManaColour> orbs => current.colours.Concat(bonus);

        public ManaSet max { get; private set; }
        public ManaSet current { get; private set; }

        private List<ManaColour> bonus { get; set; }

        public ManaPool()
        {
            max = new ManaSet();
            current = new ManaSet();
            bonus = new List<ManaColour>();
        }

        public ManaPool(ManaSet max, ManaSet current)
        {
            this.max = max;
            this.current = current;
            bonus = new List<ManaColour>();
        }

        public bool covers(IEnumerable<ManaOrb> os)
        {
            ManaSet ms = current.clone();
            foreach (var o in bonus)
            {
                ms[o]++;
            }

            int c = 0;
            foreach (var o in os)
            {
                var clr = o.colour;
                if (clr == ManaColour.Colourless) c++;
                else if (--ms[clr] < 0) return false;
            }

            return ms.count >= c;
        }

        public void gainMana(ManaColour c)
        {
            max[c]++;
            current[c]++;
        }

        public int currentMana(ManaColour c, bool withBonus = true)
        {
            return current[c] + (withBonus ? 1 : 0)*bonus.Count(v => v == c);
        }

        public int maxMana(ManaColour c)
        {
            return max[c];
        }

        public void addMana(ManaColour c, int cnt = 1)
        {
            current[c] += cnt;
        }

        public void addMax(ManaColour c, int cnt = 1)
        {
            max[c] += cnt;
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
            List<ManaColour> r = new List<ManaColour>();
            foreach (ManaColour mc in bonus)
            {
                if (costs[(int)mc] > 0)
                {
                    r.Add(mc);
                    costs[(int)mc]--;
                }
            }
            foreach (var c in r)
            {
                bonus.Remove(c);
            }

            current = current - costs;
        }

        public void addBonusMana(ManaColour c)
        {
            bonus.Add(c);
        }

        public void resetBonus()
        {
            bonus.Clear();
        }

        public ManaPool clone()
        {
            return new ManaPool(max.clone(), current.clone());
        }
    }

    class ManaSet : IEnumerable<int>
    {
        public const int size = 7;
        private int[] manas;

        public int count => manas.Sum();

        public List<ManaColour> colours => coloursEx();
        public IEnumerable<ManaOrb> orbs => colours.Select(c => new ManaOrb(c));

        public ManaSet()
        {
            manas = new int[size];
        }

        public ManaSet(int[] e)
        {
            if (e.Count() != size) throw new Exception();
            manas = e;
        }

        public ManaSet(IEnumerable<ManaColour> cs)
        {
            manas = new int[size];
            foreach (ManaColour c in cs)
            {
                manas[(int)c]++;
            }
        }

        public ManaSet(params ManaColour[] cs) : this((IEnumerable<ManaColour>)cs)
        {
            
        }

        public void add(ManaOrb orb)
        {
            manas[(int)orb.colour]++;
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

        private List<ManaColour> coloursEx()
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
            return new List<int>(manas).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return manas.GetEnumerator();
        }

        public ManaSet clone()
        {
            return new ManaSet((int[])manas.Clone());
        }
    }

    /// <summary>
    /// Nothing fancier than a wrapper around ManaColour allowing ManaOrb to implement Stuff and Targetable
    /// </summary>
    class ManaOrb : Stuff, Targetable
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
