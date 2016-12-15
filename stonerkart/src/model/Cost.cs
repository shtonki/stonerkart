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

        public ManaCost(int chaosCost, int deathCost, int lifeCost, int mightCost,  int natureCost, int orderCost, int colourlessCost)
        {
            int[] cs = new int[ManaSet.size];
            cs[(int)ManaColour.Chaos]  = chaosCost;
            cs[(int)ManaColour.Death]  = deathCost;
            cs[(int)ManaColour.Life]   = lifeCost;
            cs[(int)ManaColour.Might]  = mightCost;
            cs[(int)ManaColour.Nature] = natureCost;
            cs[(int)ManaColour.Order]  = orderCost;
            cs[(int)ManaColour.Colourless]  = colourlessCost;
            cost = new ManaSet(cs);
        }

        public int[] measure(Player p)
        {
            for (int i = 0; i < ManaSet.size; i++)
            {
                if ((ManaColour)i == ManaColour.Colourless) continue;
                if (p.manaPool.current[i] < cost[i]) return null;
            }

            ManaSet pool = p.manaPool.current;
            List<ManaColour> poolOrbs = pool.orbs;
            List<ManaColour> costOrbs = cost.orbs;

            if (poolOrbs.Count() < costOrbs.Count()) return null;

            if (cost[ManaColour.Colourless] > 0)
            {
                if (poolOrbs.Count() > costOrbs.Count())
                {
                    throw new Exception();
                }

                return pool.ToArray();

            }
            else
            {
                return cost.ToArray();
            }
        }

        public void cut(Player p, int[] iz)
        {
            p.payMana(new ManaSet(iz));
        }
    }
}
