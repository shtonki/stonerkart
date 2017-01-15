using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public TargetRule[] rules;

        public TargetRuleSet(params TargetRule[] rules) : base(rules.Select(r => r.targetType))
        {
            this.rules = rules;
        }

        public TargetMatrix fillCast(ChooseTargetToolbox str)
        {
            TargetColumn[] r = new TargetColumn[rules.Length];

            for (int i = 0; i < rules.Length; i++)
            {
                TargetColumn? v = rules[i].fillCastTargets(str);
                if (v == null) return null;
                r[i] = v.Value;
            }

            return new TargetMatrix(r);
        }

        public TargetMatrix fillResolve(TargetMatrix tm, ResolveEnv re, Game g)
        {
            TargetColumn[] r = tm.columns;

            for (int i = 0; i < rules.Length; i++)
            {
                r[i] = rules[i].fillResolveTargets(re, r[i]);
            }

            return new TargetMatrix(r);
        }

    }


    struct ChooseTargetToolbox
    {
        public Func<Stuff> getTargetable { get; }

        public ChooseTargetToolbox(Func<Stuff> getTargetable)
        {
            this.getTargetable = getTargetable;
        }
    }

    class TargetMatrix
    {
        public TargetColumn[] columns;

        public TargetMatrix(TargetColumn[] columns)
        {
            this.columns = columns;
        }

        public TargetRow[] generateRows()
        {
            int l = columns.Aggregate(1, (current, c) => current*c.targets.Length);
            int[] ms = columns.Select(c => c.targets.Length).ToArray();
            int[] cs = columns.Select(c => 0).ToArray();

            TargetRow[] r = new TargetRow[l];
            int rc = 0;
            while (true)
            {
                Targetable[] tr = new Targetable[columns.Length];
                for (int i = 0; i < columns.Length; i++)
                {
                    tr[i] = columns[i].targets[cs[i]];
                }
                r[rc++] = new TargetRow(tr);

                cs[columns.Length - 1]++;

                for (int i = columns.Length - 1; i > 0; i--)
                {
                    if (cs[i] == ms[i])
                    {
                        cs[i] = 0;
                        cs[i - 1]++;
                    }
                }

                if (cs[0] == ms[0]) return r;
            }
        }
    }

    abstract class TargetRule
    {
        public Type targetType { get; set; }

        public TargetRule(Type targetType)
        {
            this.targetType = targetType;
        }

        public abstract TargetColumn? fillCastTargets(ChooseTargetToolbox f);
        public abstract TargetColumn fillResolveTargets(ResolveEnv re, TargetColumn c);
    }

    class SelectCardRule : TargetRule
    {
        private TargetRule pg;
        private PileLocation l;

        public SelectCardRule(PileLocation location, TargetRule playerGenerator) : base(typeof(Card))
        {
            pg = playerGenerator;
            l = location;
        }

        public override TargetColumn? fillCastTargets(ChooseTargetToolbox f)
        {
            return pg.fillCastTargets(f);
        }

        public override TargetColumn fillResolveTargets(ResolveEnv re, TargetColumn c)
        {
            TargetColumn r = pg.fillResolveTargets(re, c);
            if (r.targets.Length != 1) throw new Exception();
            Player p = (Player)r.targets[0];
            Card crd = re.selector(p.pileFrom(l));
            return new TargetColumn(crd);
        }
    }

    class AoeRule : TargetRule
    {
        private PryTileRule ruler;
        private int radius;
        private Func<Card, bool> cardFilter;


        public AoeRule(Func<Tile, bool> filter, int radius, Func<Card, bool> cardFilter) : base(typeof(Card))
        {
            ruler = new PryTileRule(filter);
            this.radius = radius;
            this.cardFilter = cardFilter;
        }

        public override TargetColumn? fillCastTargets(ChooseTargetToolbox f)
        {
            return ruler.fillCastTargets(f);
        }

        public override TargetColumn fillResolveTargets(ResolveEnv re, TargetColumn c)
        {
            if (c.targets.Length != 1) throw new Exception();
            Tile centre = (Tile)c.targets[0];
            var v = centre.withinDistance(radius).Where(t => t.card != null && cardFilter(t.card)).Select(t => t.card);
            return new TargetColumn(v);
        }

        
    }

    class PryTileRule : TargetRule
    {
        private Func<Tile, bool> filter;

        public PryTileRule(Func<Tile, bool> filter) : base(typeof(Tile))
        {
            this.filter = filter;
        }

        public override TargetColumn? fillCastTargets(ChooseTargetToolbox box)
        {
            while (true)
            {
                Stuff v = box.getTargetable();

                var b = v as ShibbuttonStuff;
                if (b?.option == ButtonOption.Cancel) return null;

                if (!(v is Tile)) continue;
                Tile t = (Tile)v;
                if (filter(t)) return new TargetColumn(t);
            }
        }

        public override TargetColumn fillResolveTargets(ResolveEnv re, TargetColumn c)
        {
            return c;
        }
    }

    class PryCardRule : TargetRule
    {
        private Func<Card, bool> filter;

        public PryCardRule() : this(c => true)
        {
            
        }

        public PryCardRule(Func<Card, bool> filter) : base(typeof(Card))
        {
            this.filter = filter;
        }

        public override TargetColumn? fillCastTargets(ChooseTargetToolbox box)
        {
            while (true)
            {
                Stuff v = box.getTargetable();

                if (v is ShibbuttonStuff)
                {
                    var b = (ShibbuttonStuff)v;
                    if (b.option == ButtonOption.Cancel) return null;
                }

                if (!(v is Tile)) continue;

                Tile t = (Tile)v;
                if (t.card == null) continue;

                if (filter(t.card)) return new TargetColumn(t.card);
            }
        }

        public override TargetColumn fillResolveTargets(ResolveEnv re, TargetColumn c)
        {
            return c;
        }
    }



    class CardResolveRule : ResolveRule
    {
        private Rule rule;

        public CardResolveRule(Rule rule) : base(typeof(Card))
        {
            this.rule = rule;
        }

        public override TargetColumn fillResolveTargets(ResolveEnv re, TargetColumn c)
        {
            if (c.targets.Length != 0) throw new Exception();
            switch (rule)
            {
                case Rule.ResolveCard:
                {
                    return new TargetColumn(re.resolveCard);
                }

                case Rule.ResolveControllerCard:
                {
                    return new TargetColumn(re.resolveCard.controller.heroCard);
                }

                case Rule.VillainHeroes:
                {
                    return new TargetColumn(re.cards.Where(crd => crd.isHeroic && crd.controller != re.resolveCard.controller));
                }
            }
            throw new Exception();
        }

        public enum Rule
        {
            ResolveControllerCard,
            ResolveCard,
            
            VillainHeroes,
        }
    }

    class PlayerResolveRule : ResolveRule
    {
        private Rule rule;

        public PlayerResolveRule(Rule rule) : base(typeof(Player))
        {
            this.rule = rule;
        }

        public override TargetColumn fillResolveTargets(ResolveEnv re, TargetColumn c)
        {
            if (c.targets.Length != 0) throw new Exception();
            switch (rule)
            {
                case Rule.ResolveController:
                {
                    return new TargetColumn(re.resolveCard.controller);
                }
            }
            throw new Exception();
        }


        public enum Rule
        {
            ResolveController,

        }
    }

    abstract class ResolveRule : TargetRule
    {
        public ResolveRule(Type t) : base(t)
        {
        }

        public override TargetColumn? fillCastTargets(ChooseTargetToolbox box)
        {
            return new TargetColumn(new Targetable[] {});
        }

    }

    struct ResolveEnv
    {
        public Card resolveCard;
        public IEnumerable<Card> cards;
        public Func<IEnumerable<Card>, Card> selector;

        public ResolveEnv(Card resolveCard, IEnumerable<Card> cards, Func<IEnumerable<Card>, Card> selector)
        {
            this.resolveCard = resolveCard;
            this.cards = cards;
            this.selector = selector;
        }
    }

    struct TargetRow
    {
        public Targetable[] ts;

        public TargetRow(Targetable[] ts)
        {
            this.ts = ts;
        }
    }
    

    struct TargetColumn
    {
        public Targetable[] targets;
        
        public TargetColumn(params Targetable[] targets)
        {
            this.targets = targets;
        }

        public TargetColumn(IEnumerable<Targetable> ts) : this(ts.ToArray())
        {
            
        }
    }
    
    
    interface Targetable
    {
        
    }
}
