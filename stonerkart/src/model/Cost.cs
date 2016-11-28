using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Cost
    {
        private SubCost[] costs;

        public Cost(params SubCost[] costs)
        {
            this.costs = costs;
        }

        public int[][] measure(Player p)
        {
            int[][] r = new int[costs.Length][];
            for (int i = 0; i < costs.Length; i++)
            {
                int[] v = costs[i].measure(p);
                if (v == null) return null;
                r[i] = v;
            }

            return r;
        }

        public void cut(Player p, int[][] iz)
        {
            if (iz.Length != costs.Length) throw new Exception();
            for (int i = 0; i < costs.Length; i++)
            {
                costs[i].cut(p, iz[i]);
            }
        }

        public T getSubCost<T>()
        {
            foreach (var v in costs)
            {
                if (v is T)
                {
                    return (T)v;
                }
            }
            throw new Exception("Card has no mana cost idiot");
        }
    }

    interface SubCost
    {
        int[] measure(Player p);

        void cut(Player p, int[] iz);
    }

    class ManaCost : SubCost
    {
        public int[] costs;

        public ManaCost(params ManaColour[] cs)
        {
            costs = new int[6];
            foreach (var c in cs)
            {
                costs[(int)c]++;
            }
        }

        public int[] measure(Player p)
        {
            for (int i = 0; i < 6; i++)
            {
                if (p.manaPool.current[i] < costs[i]) return null;
            }
            return costs;
        }

        public void cut(Player p, int[] iz)
        {
            p.payMana(iz);
        }
    }
}
