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

        public TargetRuleSet(params TargetRule[] rules)
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
        private TargetRule playerRule;
        private int count;
        private bool allowDuplicates;
        private Func<T, bool> filter;

        public InteractiveRule(Type targetType, TargetRule playerRule, int count, bool allowDuplicates, Func<T, bool> filter) : base(targetType)
        {
            this.playerRule = playerRule;
            this.count = count;
            this.allowDuplicates = allowDuplicates;
            this.filter = filter;
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
            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");/*
            ruler = new PryTileRule(filter, new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController));
            this.radius = radius;
            this.cardFilter = cardFilter;*/
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

    class ClickCardRule : TargetRule
    {
        public ClickCardRule() : base(typeof(Card))
        {
        }

        public override TargetSet fillCastTargets(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet ts)
        {
            throw new NotImplementedException();
        }
    }

    class ClickTileRule : TargetRule
    {
        public ClickTileRule() : base(typeof(Tile))
        {
        }

        public override TargetSet fillCastTargets(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet ts)
        {
            throw new NotImplementedException();
        }
    }

    class ClickPlayerRule : TargetRule
    {
        public ClickPlayerRule() : base(typeof(Player))
        {
        }

        public override TargetSet fillCastTargets(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet ts)
        {
            throw new NotImplementedException();
        }
    }
    

    interface Targetable
    {
        int stateCtr();
    }
}
