using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    struct CostPayStruct
    {
        public Func<Stuff> getStuff { get; }

        public CostPayStruct(Func<Stuff> getStuff)
        {
            this.getStuff = getStuff;
        }
    }

    class Cost
    {
        private SubCost[] costs;

        public Cost(params SubCost[] costs)
        {
            this.costs = costs;
        }

        public int[][] measure(Player p, CostPayStruct str)
        {
            int[][] r = new int[costs.Length][];
            for (int i = 0; i < costs.Length; i++)
            {
                int[] v = costs[i].measure(p, str);
                if (v == null) return null;
                r[i] = v;
            }

            return r;
        }

        public bool possible(Player p)
        {
            return costs.All(c => c.possible(p));
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
        int[] measure(Player p, CostPayStruct s);

        void cut(Player p, int[] iz);

        bool possible(Player p);
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

        public bool possible(Player p)
        {
            if (p.manaPool.current.orbs.Count < cost.orbs.Count) return false;
            for (int i = 0; i < ManaSet.size; i++)
            {
                if ((ManaColour)i == ManaColour.Colourless) continue;
                if (p.manaPool.current[i] < cost[i]) return false;
            }
            return true;
        }

        public int[] measure(Player p, CostPayStruct s)
        {
            if (!possible(p)) return null;

            ManaSet cost = this.cost.clone();
            for (int i = 0; i < ManaSet.size; i++)
            {
                if ((ManaColour)i == ManaColour.Colourless) continue;
                if (p.manaPool.current[i] < cost[i]) return null;
            }

            ManaSet pool = p.manaPool.current;
            List<ManaColour> poolOrbs = pool.orbs;
            List<ManaColour> costOrbs = cost.orbs;

            int diff = costOrbs.Count() - poolOrbs.Count();

            if (diff > 0) return null;

            if (cost[ManaColour.Colourless] > 0)
            {
                if (diff < 0) // we have more total mana than the cost
                {
                    Controller.setPrompt("Cast using what mana", ButtonOption.Cancel);
                    var colours = cost.clone();
                    colours[ManaColour.Colourless] = 0;
                    p.stuntCurrentLoss(colours);
                    while (cost[ManaColour.Colourless] > 0)
                    {
                        var v = s.getStuff();
                        if (v is ManaOrb)
                        {
                            ManaOrb orb = (ManaOrb)v;
                            ManaColour colour = orb.colour;
                            if (pool[colour] - cost[colour] > 0)
                            {
                                cost[colour]++;
                                cost[ManaColour.Colourless]--;
                                p.stuntCurrentDiff(colour, -1);
                            }
                        }
                        if (v is ShibbuttonStuff)
                        {
                            ShibbuttonStuff b = (ShibbuttonStuff)v;
                            if (b.option == ButtonOption.Cancel)
                            {
                                p.unstuntMana();
                                return null;
                            }
                        }
                    }
                    p.unstuntMana();
                    return cost.ToArray();
                }

                return pool.ToArray(); // we have the same amount of mana as the cost

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
