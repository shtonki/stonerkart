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

    struct DoerToolKit
    {
        public bool activePlayer { get; }

        public Action<int[]> sendChoices { get; }
        public Func<int[]> receiveChoices { get; }

        public DoerToolKit(bool activePlayer, Action<int[]> sendChoices, Func<int[]> receiveChoices)
        {
            this.activePlayer = activePlayer;
            this.sendChoices = sendChoices;
            this.receiveChoices = receiveChoices;
        }
    }

    abstract class Doer : TypeSigned
    {
        public Doer(params Type[] typeSignatureTypes) : base(typeSignatureTypes)
        {
        }

        public abstract GameEvent[] act(TargetRow[] ts);
    }

    abstract class SimpleDoer : Doer
    {
        public SimpleDoer(params Type[] typeSignatureTypes) : base(typeSignatureTypes)
        {
        }

        public override GameEvent[] act(TargetRow[] ts)
        {
            List<GameEvent> r = new List<GameEvent>();
            foreach (TargetRow row in ts)
            {
                r.AddRange(simpleAct(row));
            }
            return r.ToArray();
        }

        protected abstract GameEvent[] simpleAct(TargetRow row);
    }

    class DrawCardsDoer : SimpleDoer
    {
        public int cards;

        public DrawCardsDoer(int cards) : base(typeof(Player))
        {
            this.cards = cards;
        }

        protected override GameEvent[] simpleAct(TargetRow row)
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

        protected override GameEvent[] simpleAct(TargetRow row)
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

        protected override GameEvent[] simpleAct(TargetRow row)
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

        protected override GameEvent[] simpleAct(TargetRow row)
        {
            Card damager = (Card)row.ts[0];
            Card damaged = (Card)row.ts[1];
            return new[] {new DamageEvent(damager, damaged, damage)};
        }
    }

    class ToOwners : SimpleDoer
    {
        public PileLocation pileLocation;

        public ToOwners(PileLocation pileLocation) : base(typeof(Card))
        {
            this.pileLocation = pileLocation;
        }

        protected override GameEvent[] simpleAct(TargetRow row)
        {
                Card c = (Card)row.ts[0];
                return new[] { new MoveToPileEvent(c, c.owner.pileFrom(pileLocation))};
        }
    }
}
