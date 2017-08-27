using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart
{
    abstract class TypeSigned
    {
        protected Type[] typeSignatureTypes;

        public TypeSigned(IEnumerable<Type> typeSignatureTypes) : this(typeSignatureTypes.ToArray())
        {

        }

        public TypeSigned(params Type[] typeSignatureTypes)
        {
            this.typeSignatureTypes = typeSignatureTypes;
        }

        public bool matchesTypeSignatureOf(TypeSigned other)
        {
            return typeSignatureTypes.SequenceEqual(other.typeSignatureTypes);
        }
    }

    class TargetRuleSet : TypeSigned
    {
        private TargetRule[] rules;

        public TargetRuleSet(params TargetRule[] rules) : base(rules.Select(t => t.targetType))
        {
            this.rules = rules;
        }

        public TargetSet[] fillCast(HackStruct hs)
        {
            var cache = new TargetSet[rules.Length];
            for (int i = 0; i < rules.Length; i++)
            {
                var targetSet = rules[i].fillCastTargets(hs);
                if (targetSet == null)
                {
                    return null;
                }
                cache[i] = targetSet;
            }
            return cache;
        }

        public TargetSet[] fillResolve(HackStruct hs, TargetSet[] cached)
        {
            if (rules.Length != cached.Length) throw new Exception();

            for (int i = 0; i < rules.Length; i++)
            {
                var cachedTargetSet = cached[i];
                if (cachedTargetSet == null) throw new Exception();
                var newTargetSet = rules[i].fillResolveTargets(hs, cachedTargetSet);
                if (newTargetSet == null)
                {
                    return null;
                }
                cached[i] = newTargetSet;
            }

            return cached;
        }

    }

    class TargetSet
    {
        public Targetable[] targets { get; }

        public TargetSet(params Targetable[] targets)
        {
            this.targets = targets;
        }

        public TargetSet(IEnumerable<Targetable> ts) : this(ts.ToArray())
        {

        }
    }

    class TargetRow
    {
        private Targetable[] targets;

        public Targetable this[int index]
        {
            get { return targets[index]; }
        }

        public TargetRow(params Targetable[] targets)
        {
            this.targets = targets;
        }

        public TargetRow(IEnumerable<Targetable> ts) : this(ts.ToArray())
        {
            
        }
    }

    abstract class TargetRule
    {
        public Type targetType { get; protected set; }

        public TargetRule(Type targetType)
        {
            this.targetType = targetType;
        }
        
        public abstract TargetSet fillCastTargets(HackStruct hs);
        public abstract TargetSet fillResolveTargets(HackStruct hs, TargetSet ts);
    }

    interface chooserface<T> where T : Targetable
    {
        IEnumerable<T> candidates(HackStruct hs);
        T pickOne(Player chooser, Func<T, bool> filter, HackStruct hs);
    }

    class ChooseRule<T> : TargetRule where T : Targetable
    {
        private chooserface<T> chooser = dflt();
        private TargetRule playerRule = new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController);
        private ChooseAt chooseAt = ChooseAt.Cast;
        protected Func<T, bool> filter = t => true;
        private int count = 1;
        private bool allowDuplicates = false;

        private static chooserface<T> dflt()
        {
            Type t = typeof (T);
            if (t == typeof (Card)) return (chooserface<T>)new ClickCardRule();
            if (t == typeof (Tile)) return (chooserface<T>)new ClickTileRule();
            if (t == typeof (Player)) return (chooserface<T>)new ClickPlayerRule();
            if (t == typeof (ManaOrb)) return (chooserface<T>)new ClickManaRule();
            throw new Exception();
        }

        public ChooseRule(chooserface<T> chooser, TargetRule playerRule, ChooseAt chooseAt, Func<T, bool> filter, int count, bool allowDuplicates) : base(typeof(T))
        {
            this.playerRule = playerRule;
            this.count = count;
            this.allowDuplicates = allowDuplicates;
            this.chooseAt = chooseAt;
            this.filter = filter;
            this.chooser = chooser;
        }

        public ChooseRule(Func<T, bool> filter) : base(typeof(T))
        {
            this.filter = filter;
        }

        public ChooseRule(chooserface<T> chooser, TargetRule playerRule, ChooseAt chooseAt, Func<T, bool> filter) : base(typeof(T))
        {
            this.playerRule = playerRule;
            this.chooseAt = chooseAt;
            this.filter = filter;
            this.chooser = chooser;
        }

        public ChooseRule(ChooseAt chooseAt, Func<T, bool> filter, int count, bool allowDuplicates) : base(typeof(T))
        {
            this.chooseAt = chooseAt;
            this.filter = filter;
            this.count = count;
            this.allowDuplicates = allowDuplicates;
        }

        public ChooseRule(ChooseAt chooseAt) : base(typeof(T))
        {
            this.chooseAt = chooseAt;
        }

        public ChooseRule(chooserface<T> chooser, ChooseAt chooseAt, Func<T, bool> filter) : base(typeof(T))
        {
            this.chooser = chooser;
            this.chooseAt = chooseAt;
            this.filter = filter;
        }

        public ChooseRule(chooserface<T> chooser) : base(typeof(T))
        {
            this.chooser = chooser;
        }

        public ChooseRule(ChooseAt chooseAt, Func<T, bool> filter) : base(typeof(T))
        {
            this.filter = filter;
            this.chooseAt = chooseAt;
        }

        public ChooseRule() : base(typeof(T))
        {
        }

        public ChooseRule(chooserface<T> chooser, ChooseAt chooseAt) : base(typeof(T))
        {
            this.chooser = chooser;
            this.chooseAt = chooseAt;
        }


        public override TargetSet fillCastTargets(HackStruct hs)
        {
            var choosers = playerRule.fillCastTargets(hs);
            if (chooseAt == ChooseAt.Cast)
            {
                choosers = playerRule.fillResolveTargets(hs, choosers);
                return choose(choosers.targets.Cast<Player>(), hs);
            }
            else return choosers;
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet ts)
        {
            if (chooseAt == ChooseAt.Cast) return ts;
            var choosers = playerRule.fillResolveTargets(hs, ts);
            return choose(choosers.targets.Cast<Player>(), hs);
        }

        private TargetSet choose(IEnumerable<Player> players, HackStruct hs)
        {
            if (count != 1) throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
            if (players.Count() != 1) throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
            foreach (var player in players)
            {
                var chosen = chooser.pickOne(player, filter, hs);
                if (chosen == null) return null;
                return new TargetSet(chosen);
            }
            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
        }

        public enum ChooseAt
        {
            Cast,
            Resolve
        }
    }

    class SelectCardRule : chooserface<Card>
    {
        private Mode mode;
        private PileLocation pile;

        public SelectCardRule(PileLocation pile, Mode mode)
        {
            this.mode = mode;
            this.pile = pile;
        }

        public IEnumerable<Card> candidates(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        public Card pickOne(Player chooser, Func<Card, bool> filter, HackStruct hs)
        {
            throw new NotImplementedException();
        }

        public enum Mode
        {
            ResolverLooksAtPlayer,
            PlayerLooksAtPlayer,
            PlayerLooksAtResolver,
        }
    }


    class ClickCardRule : chooserface<Card>
    {
        public IEnumerable<Card> candidates(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        public Card pickOne(Player chooser, Func<Card, bool> filter, HackStruct hs)
        {
            Tile tl = hs.game.chooseTileSynced(chooser, t => t.card != null && filter(t.card), "swroigjs", "ioerhjgohr", true);
            if (tl == null) return null;
            return (tl.card);
        }
    }

    class ClickTileRule : chooserface<Tile>
    {
        public IEnumerable<Tile> candidates(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        public Tile pickOne(Player chooser, Func<Tile, bool> filter, HackStruct hs)
        {
            return hs.game.chooseTileSynced(chooser, filter, "", "", true);
        }
    }

    class ClickPlayerRule : chooserface<Player>
    {
        public IEnumerable<Player> candidates(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        public Player pickOne(Player chooser, Func<Player, bool> filter, HackStruct hs)
        {
            throw new NotImplementedException();
        }
    }

    class ClickManaRule : chooserface<ManaOrb>
    {
        public IEnumerable<ManaOrb> candidates(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        public ManaOrb pickOne(Player chooser, Func<ManaOrb, bool> filter, HackStruct hs)
        {
            throw new NotImplementedException();
        }
    }

    class ManaCostRule : TargetRule
    {
        private ManaSet cst;

        public ManaCostRule(ManaSet cst) : base(typeof(ManaOrb))
        {
            this.cst = cst;
        }

        public override TargetSet fillCastTargets(HackStruct hs)
        {
            return meme(hs.castingPlayer, hs);
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet ts)
        {
            return ts;
        }

        private TargetSet meme(Player p, HackStruct hs)
        {
            ManaSet cost = cst.clone();
            for (int i = 0; i < ManaSet.size; i++)
            {
                if ((ManaColour)i == ManaColour.Colourless) continue;
                if (p.manaPool.currentMana((ManaColour)i) < cost[i]) return null;
            }

            IEnumerable<ManaColour> poolOrbs = p.manaPool.orbs;
            IEnumerable<ManaColour> costOrbs = cost.orbs.Select(o => o.colour);

            int diff = costOrbs.Count() - poolOrbs.Count();

            if (diff > 0) return null;

            if (cost[ManaColour.Colourless] > 0)
            {
                if (diff < 0) // we have more total mana than the cost
                {

                    throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");/*
                    ManaSet colours = cost.clone();
                    colours[ManaColour.Colourless] = 0;
                    p.stuntLoss(colours);
                    while (cost[ManaColour.Colourless] > 0)
                    {
                        var clr = hs.game.chooseManaColourSynced(p, c => true).Value;
                        if (bc.targets.Length == 1)
                        {
                            var orb = (ManaOrb)bc.targets[0];
                            var colour = orb.colour;
                            if (p.manaPool.currentMana(colour) - cost[colour] > 0)
                            {
                                cost[colour]++;
                                cost[ManaColour.Colourless]--;
                                colours[colour]++;
                                p.stuntLoss(colours);
                            }
                        }
                        else
                        {
                                p.unstuntMana();
                                return null;
                        }
                    }
                    p.unstuntMana();*/
                }
                else
                {
                    cost = new ManaSet(hs.castingPlayer.manaPool.orbs);
                }
            }
            return new TargetSet(cost.orbs);
        }
    }

    class StaticManaRule : TargetRule
    {
        private ManaColour[] colours;

        public StaticManaRule(params ManaColour[] colours) : base(typeof(ManaOrb))
        {
            this.colours = colours;
        }

        public override TargetSet fillCastTargets(HackStruct f)
        {
            return new TargetSet();
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet c)
        {
            return new TargetSet(colours.Select(clr => new ManaOrb(clr)));
        }
    }

    class TriggeredTargetRule<G, T> : TargetRule where T : Targetable where G : GameEvent
    {
        private Func<G, T> func;

        public TriggeredTargetRule(Func<G, T> func) : base(typeof(T))
        {
            this.func = func;
        }

        public override TargetSet fillCastTargets(HackStruct f)
        {
            G g = (G)f.triggeringEvent;
            var t = (Targetable)func(g);
            return new TargetSet(new []{t});
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet ts)
        {
            return ts;
        }
        
    }
    
    class CreateTokenRule : TargetRule
    {
        private TargetRule summonFor;
        private CardTemplate[] tokens;

        public CreateTokenRule(TargetRule summonFor, params CardTemplate[] tokens) : base(typeof(Card))
        {
            this.summonFor = summonFor;
            this.tokens = tokens;
        }

        public override TargetSet fillCastTargets(HackStruct f)
        {
            return summonFor.fillCastTargets(f);
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet c)
        {
            TargetSet players = summonFor.fillResolveTargets(hs, c);
            Player[] ps = players.targets.Cast<Player>().ToArray();
            List<Card> ts = new List<Card>(ps.Length);

            foreach (Player p in ps)
            {
                foreach (var t in tokens)
                {
                    ts.Add(hs.game.createToken(t, p));
                }
            }
            return new TargetSet(ts);
        }
    }

    class ModifyPreviousRule<P, T> : TargetRule where P : Targetable where T : Targetable
    {
        private int column;
        private Func<P, T> fn;

        public ModifyPreviousRule(int column, Func<P, T> f) : base(typeof(T))
        {
            this.column = column;
            this.fn = f;
        }

        public override TargetSet fillCastTargets(HackStruct hs)
        {
            return new TargetSet();
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet c)
        {

            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");/*
            return new TargetSet(hs.previousTargets.columns[column].targets.Cast<P>().Select(p => fn(p)).Cast<Targetable>());
            //return hs.previousTargets.columns[column];*/
        }

    }

    class AoeRule : TargetRule
    {
        private ChooseRule<Tile> ruler;
        private int radius;
        private Func<Card, bool> cardFilter;


        public AoeRule(Func<Tile, bool> filter, int radius, Func<Card, bool> cardFilter) : base(typeof(Card))
        {
            ruler = new ChooseRule<Tile>(filter);
            this.radius = radius;
            this.cardFilter = cardFilter;
        }

        public override TargetSet fillCastTargets(HackStruct f)
        {
            return ruler.fillCastTargets(f);
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet c)
        {
            c = ruler.fillResolveTargets(hs, c);
            if (c.targets.Length != 1) throw new Exception();
            Tile centre = (Tile)c.targets[0];
            var v = centre.withinDistance(radius).Where(t => t.card != null && cardFilter(t.card)).Select(t => t.card);
            return new TargetSet(v);
        }
        
    }

    class CardResolveRule : TargetRule
    {
        private Rule rule;

        public CardResolveRule(Rule rule) : base(typeof(Card))
        {
            this.rule = rule;
        }

        public override TargetSet fillCastTargets(HackStruct hs)
        {
            return new TargetSet();
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet c)
        {
            if (c.targets.Length != 0) throw new Exception();
            switch (rule)
            {
                case Rule.ResolveCard:
                {
                    return new TargetSet(hs.resolveCard);
                }

                case Rule.ResolveControllerCard:
                {
                    return new TargetSet(hs.resolveCard.controller.heroCard);
                }
            }
            throw new Exception();
        }
        

        public enum Rule
        {
            ResolveControllerCard,
            ResolveCard,
        }
        
    }

    class CardsRule : TargetRule
    {
        private Func<Card, bool> filter;

        public CardsRule(Func<Card, bool> filter) : base(typeof(Card))
        {
            this.filter = filter;
        }

        public override TargetSet fillCastTargets(HackStruct hs)
        {
            return new TargetSet();
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet ts)
        {
            return new TargetSet(hs.cards.Where(filter));
        }
    }

    class PlayerResolveRule : TargetRule
    {
        private Rule rule;

        public PlayerResolveRule(Rule rule) : base(typeof(Player))
        {
            this.rule = rule;
        }

        public override TargetSet fillCastTargets(HackStruct hs)
        {
            return new TargetSet();
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet c)
        {
            if (c.targets.Length != 0) throw new Exception();
            switch (rule)
            {
                case Rule.ResolveController:
                {
                    return new TargetSet(hs.resolveCard.controller);
                }

                case Rule.AllPlayers:
                {
                    return new TargetSet(hs.players);
                }
            }
            throw new Exception();
        }

        public enum Rule
        {
            ResolveController,
            AllPlayers,
        }

    }


    interface Targetable
    {
        int stateCtr();
    }
}
