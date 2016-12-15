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
        public ManaSet cost { get; }

        public ManaCost(params ManaColour[] cs)
        {
            cost = new ManaSet(cs);
        }

        public ManaCost(int chaosCost, int deathCost, int lifeCost, int mightCost,  int natureCost, int orderCost)
        {
            int[] cs = new int[6];
            cs[(int)ManaColour.Chaos]  = chaosCost;
            cs[(int)ManaColour.Death]  = deathCost;
            cs[(int)ManaColour.Life]   = lifeCost;
            cs[(int)ManaColour.Might]  = mightCost;
            cs[(int)ManaColour.Nature] = natureCost;
            cs[(int)ManaColour.Order]  = orderCost;
            cost = new ManaSet(cs);
        }

        public int[] measure(Player p)
        {
            for (int i = 0; i < 6; i++)
            {
                if (p.manaPool.current[i] < cost[i]) return null;
            }
            return cost.ToArray();
        }

        public void cut(Player p, int[] iz)
        {
            p.payMana(new ManaSet(iz));
        }
    }
}
