using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Foo
    {
        protected Effect[] effects;

        public Foo()
        {
            effects = new Effect[0];
        }

        public Foo(params Effect[] effects)
        {
            this.effects = effects;
        }

        public IEnumerable<GameEvent> resolve(HackStruct hs, TargetMatrix[] ts)
        {
            List<GameEvent> rt = new List<GameEvent>();
            for (int i = 0; i < ts.Length; i++)
            {
                Effect effect = effects[i];
                TargetMatrix matrix = effect.ts.fillResolve(ts[i], hs);
                
                rt.AddRange(effect.doer.act(hs, matrix.generateRows()));
            }
            return rt;
        }

        public TargetMatrix[] fillCast(HackStruct hs)
        {
            TargetMatrix[] rt = new TargetMatrix[effects.Length];

            for (int i = 0; i < effects.Length; i++)
            {
                rt[i] = effects[i].fillCast(hs);
                if (rt[i] == null) return null;
            }

            return rt;
        }

        public TargetMatrix[] fillResolve(TargetMatrix[] tms, HackStruct hs)
        {
            TargetMatrix[] rt = new TargetMatrix[effects.Length];

            for (int i = 0; i < effects.Length; i++)
            {
                rt[i] = effects[i].fillResolve(tms[i], hs);
                if (rt[i] == null) return null;
            }

            return rt;
        }

        public bool possible(HackStruct hs)
        {
            return effects.All(e => e.possible(hs));
        }
    }

    /*
    interface SubCost
    {
        int[] measure(Player p, HackStruct s);

        IEnumerable<GameEvent> cut(Player p, HackStruct s, int[] iz);

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
            if (p.manaPool.orbs.Count() < cost.orbs.Count) return false;
            for (int i = 0; i < ManaSet.size; i++)
            {
                if ((ManaColour)i == ManaColour.Colourless) continue;
                if (p.manaPool.currentMana((ManaColour)i) < cost[i]) return false;
            }
            return true;
        }

        public int[] measure(Player p, HackStruct s)
        {
            ManaSet cost = this.cost.clone();
            for (int i = 0; i < ManaSet.size; i++)
            {
                if ((ManaColour)i == ManaColour.Colourless) continue;
                if (p.manaPool.currentMana((ManaColour)i) < cost[i]) return null;
            }

            IEnumerable<ManaColour> poolOrbs = p.manaPool.orbs;
            IEnumerable<ManaColour> costOrbs = cost.orbs;

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
                            if (p.manaPool.currentMana(colour) - cost[colour] > 0)
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

                return cost.ToArray(); // we have the same amount of mana as the cost

            }
            else
            {
                return cost.ToArray();
            }
        }

        public IEnumerable<GameEvent> cut(Player p, HackStruct s, int[] iz)
        {
            return new GameEvent[] {new PayManaEvent(p, new ManaSet(iz)) };
        }
    }

    class SelectAndMoveCost : SubCost
    {
        private Func<Card, bool> predicate;
        private PileLocation frm;
        private PileLocation to;

        public SelectAndMoveCost(Func<Card, bool> predicate, PileLocation frm, PileLocation to)
        {
            this.predicate = predicate;
            this.frm = frm;
            this.to = to;
        }

        public int[] measure(Player p, HackStruct s)
        {
            Card c = s.selectCard(p.pileFrom(frm), true);
            if (c == null) return null;
            return new[] {s.ordC(c)};
        }

        public IEnumerable<GameEvent> cut(Player p, HackStruct s, int[] iz)
        {
            return new[] {new MoveToPileEvent(s.Cord(iz[0]), p.pileFrom(to)),};
        }

        public bool possible(Player p)
        {
            return p.pileFrom(frm).Count(predicate) > 0;
        }
    }
*/
}