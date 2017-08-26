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

    abstract class InteractiveRule<T> : TargetRule where T : Targetable
    {
        private TargetRule playerRule = new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController);
        private int count = 1;
        private bool allowDuplicates = false;
        private ChooseAt chooseAt;
        protected Func<T, bool> filter = t => true;

        public InteractiveRule(TargetRule playerRule, int count, bool allowDuplicates, ChooseAt chooseAt, Func<T, bool> filter) : base(typeof(T))
        {
            this.playerRule = playerRule;
            this.count = count;
            this.allowDuplicates = allowDuplicates;
            this.chooseAt = chooseAt;
            this.filter = filter;
        }

        public InteractiveRule(TargetRule playerRule, ChooseAt chooseAt, Func<T, bool> filter) : base(typeof (T))
        {
            this.playerRule = playerRule;
            this.chooseAt = chooseAt;
            this.filter = filter;
        }

        public InteractiveRule(TargetRule playerRule, ChooseAt chooseAt) : base(typeof(T))
        {
            this.playerRule = playerRule;
            this.chooseAt = chooseAt;
        }

        public InteractiveRule(ChooseAt chooseAt) : base(typeof(T))
        {
            this.chooseAt = chooseAt;
        }

        public InteractiveRule(ChooseAt chooseAt, Func<T, bool> filter) : base(typeof(T))
        {
            this.chooseAt = chooseAt;
            this.filter = filter;
        }

        public InteractiveRule(ChooseAt chooseAt, Func<T, bool> filter, TargetRule playerRule) : base(typeof(T))
        {
            this.chooseAt = chooseAt;
            this.filter = filter;
            this.playerRule = playerRule;
        }

        public InteractiveRule(TargetRule playerRule, int count, bool allowDuplicates, ChooseAt chooseAt) : base(typeof(T))
        {
            this.playerRule = playerRule;
            this.count = count;
            this.allowDuplicates = allowDuplicates;
            this.chooseAt = chooseAt;
        }

        public InteractiveRule(int count, bool allowDuplicates, ChooseAt chooseAt, Func<T, bool> filter) : base(typeof(T))
        {
            this.count = count;
            this.allowDuplicates = allowDuplicates;
            this.chooseAt = chooseAt;
            this.filter = filter;
        }


        public override TargetSet fillCastTargets(HackStruct hs)
        {
            var choosers = playerRule.fillCastTargets(hs);
            if (chooseAt == ChooseAt.Cast)
            {
                choosers = playerRule.fillResolveTargets(hs, choosers);
                return interact(choosers.targets.Cast<Player>());
            }
            else return choosers;
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet ts)
        {
            if (chooseAt == ChooseAt.Cast) return ts;
            var choosers = playerRule.fillResolveTargets(hs, ts);
            return interact(choosers.targets.Cast<Player>());
        }

        private TargetSet interact(IEnumerable<Player> players)
        {
            if (count != 1) throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
            if (players.Count() != 1) throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
            foreach (var player in players)
            {
                return interact(player, filter);
            }
            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
        }

        protected abstract TargetSet interact(Player p, Func<T, bool> filter);

        public enum ChooseAt
        {
            Cast,
            Resolve
        }
    }

    class SelectCardRule : InteractiveRule<Card>
    {
        private Mode mode;
        private PileLocation pile;

        public SelectCardRule(TargetRule playerRule, PileLocation pile, Func<Card, bool> filter, Mode mode, ChooseAt chooseAt) : base(playerRule, chooseAt, filter)
        {
            this.mode = mode;
            this.pile = pile;
        }

        public SelectCardRule(TargetRule playerRule, PileLocation pile, Func<Card, bool> filter, int count, bool allowDuplicates, Mode mode, ChooseAt chooseAt) : base(playerRule, count, allowDuplicates, chooseAt, filter)
        {
            this.mode = mode;
            this.pile = pile;
        }

        public SelectCardRule(PileLocation pile, ChooseAt chooseAt) : this(pile, c => true, chooseAt)
        {
        }

        public SelectCardRule(PileLocation pile, Func<Card, bool> filter, ChooseAt chooseAt) : base(chooseAt, filter)
        {
            this.pile = pile;
            mode = Mode.ResolverLooksAtTarget;
        }

        protected override TargetSet interact(Player p, Func<Card, bool> filter)
        {
            throw new NotImplementedException();
        }

        public enum Mode
        {
            ResolverLooksAtTarget,
            TargetLooksAtTarget,
            TargetLooksAtResolver,
        }
    }


    class ClickCardRule : InteractiveRule<Card>
    {
        public ClickCardRule(Func<Card, bool> filter) : base(ChooseAt.Cast, filter)
        {
        }

        public ClickCardRule() : base(ChooseAt.Cast)
        {
        }

        protected override TargetSet interact(Player p, Func<Card, bool> filter)
        {
            throw new NotImplementedException();
        }
    }

    class ClickTileRule : InteractiveRule<Tile>
    {
        public ClickTileRule(Func<Tile, bool> filter) : base(ChooseAt.Cast, filter)
        {
        }

        public ClickTileRule(ChooseAt chooseAt, Func<Tile, bool> filter) : base(chooseAt, filter)
        {
        }

        public ClickTileRule(TargetRule playerRule, Func<Tile, bool> filter, ChooseAt chooseAt) : base(playerRule, chooseAt, filter)
        {
        }

        public ClickTileRule(TargetRule playerRule, Func<Tile, bool> filter, int count, bool allowDuplicates, ChooseAt chooseAt) : base(playerRule, count, allowDuplicates, chooseAt, filter)
        {
        }

        protected override TargetSet interact(Player p, Func<Tile, bool> filter)
        {
            throw new NotImplementedException();
        }
    }

    class ClickPlayerRule : InteractiveRule<Player>
    {
        public ClickPlayerRule() : base(ChooseAt.Cast)
        {
            
        }

        protected override TargetSet interact(Player p, Func<Player, bool> filter)
        {
            throw new NotImplementedException();
        }
    }

    class SelectManaRule : TargetRule
    {
        private TargetRule playerRule;

        public SelectManaRule(TargetRule playerRule) : base(typeof(ManaOrb))
        {
            this.playerRule = playerRule;
        }

        public override TargetSet fillCastTargets(HackStruct f)
        {
            return playerRule.fillCastTargets(f);
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet ts)
        {
            var selectingPlayers = playerRule.fillResolveTargets(hs, ts);
            return
                new TargetSet(
                    selectingPlayers.targets.Cast<Player>().Select(p => new ManaOrb(hs.game.chooseManaColourSynced(p, clr => true))));
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
        private ClickTileRule ruler;
        private int radius;
        private Func<Card, bool> cardFilter;


        public AoeRule(Func<Tile, bool> filter, int radius, Func<Card, bool> cardFilter) : base(typeof(Card))
        {
            ruler = new ClickTileRule(filter);
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
