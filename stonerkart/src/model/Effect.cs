using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Effect
    {
        public TargetRuleSet ts;
        public Doer doer;


        public Effect(TargetRuleSet ts, Doer d)
        {
            if (!ts.matchesTypeSignatureOf(d)) throw new Exception();
            this.ts = ts;
            doer = d;
        }

    }


    abstract class Doer : TypeSigned
    {
        public Doer(params Type[] typeSignatureTypes) : base(typeSignatureTypes)
        {
        }

        public abstract GameEvent[] act(HackStruct dkt, TargetRow[] ts);
    }

    abstract class SimpleDoer : Doer
    {
        public SimpleDoer(params Type[] typeSignatureTypes) : base(typeSignatureTypes)
        {
        }

        public override GameEvent[] act(HackStruct dkt, TargetRow[] ts)
        {
            List<GameEvent> r = new List<GameEvent>();
            foreach (TargetRow row in ts)
            {
                r.AddRange(simpleAct(dkt, row));
            }
            return r.ToArray();
        }

        protected abstract GameEvent[] simpleAct(HackStruct dkt, TargetRow row);
    }

    class GainBonusManaDoer : SimpleDoer
    {
        public IEnumerable<ManaColour> colour;

        public GainBonusManaDoer(ManaColour colour) : base(typeof(Player))
        {
            this.colour = new[] { colour};
        }

        public GainBonusManaDoer(params ManaColour[] colour) : base(typeof(Player))
        {
            this.colour = colour;
        }

        protected override GameEvent[] simpleAct(HackStruct dkt, TargetRow ts)
        {
            Player p = (Player)ts.ts[0];
            return colour.Select(c => new GainBonusManaEvent(p, c)).ToArray();
        }
    }

    class DrawCardsDoer : SimpleDoer
    {
        public int cards;

        public DrawCardsDoer(int cards) : base(typeof(Player))
        {
            this.cards = cards;
        }

        protected override GameEvent[] simpleAct(HackStruct dkt, TargetRow row)
        {
            Player player = (Player)row.ts[0];
            return new[] {new DrawEvent(player, cards)};
        }
    }

    class SwapWithCard : SimpleDoer
    {
        public SwapWithCard() : base(typeof(Card), typeof(Card))
        {

        }

        protected override GameEvent[] simpleAct(HackStruct dkt, TargetRow row)
        {
            Card moved1 = (Card)row.ts[0];
            Tile move1To = ((Card)row.ts[1]).tile;
            Card moved2 = (Card)row.ts[1];
            Tile move2To = ((Card)row.ts[0]).tile;
            return new[] { new PlaceOnTileEvent(moved1, new Tile(null, 0, 0)), new PlaceOnTileEvent(moved2, move2To), new PlaceOnTileEvent(moved1, move1To)};
        }
    }

    class MoveToTileDoer : SimpleDoer
    {
        public MoveToTileDoer() : base(typeof(Card), typeof(Tile))
        {

        }

        protected override GameEvent[] simpleAct(HackStruct dkt, TargetRow row)
        {
            Card moved = (Card)row.ts[0];
            Tile moveTo = (Tile)row.ts[1];
            return new[] {new PlaceOnTileEvent(moved, moveTo)};
        }
    }

    class ZepperDoer : SimpleDoer
    {
        public int damage;

        public ZepperDoer(int damage) : base(typeof(Card), typeof(Card))
        {
            this.damage = damage;
        }

        protected override GameEvent[] simpleAct(HackStruct dkt, TargetRow row)
        {
            Card damager = (Card)row.ts[0];
            Card damaged = (Card)row.ts[1];
            return new[] {new DamageEvent(damager, damaged, damage)};
        }
    }

    class ToOwnersDoer : SimpleDoer
    {
        public PileLocation pileLocation;

        public ToOwnersDoer(PileLocation pileLocation) : base(typeof(Card))
        {
            this.pileLocation = pileLocation;
        }

        protected override GameEvent[] simpleAct(HackStruct dkt, TargetRow row)
        {
            Card c = (Card)row.ts[0];

            var e = new MoveToPileEvent(c, c.owner.pileFrom(pileLocation));
            return c.pile.location.pile == PileLocation.Deck
                ? new GameEvent[] {new ShuffleDeckEvent(c.owner), e}
                : new GameEvent[] {e};

        }
    }
}
