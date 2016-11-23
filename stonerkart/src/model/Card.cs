using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Card : Observable<CardChangedMessage>, Stuff, Targetable
    {
        public string name { get; }
        public Image image { get; }
        public Pile pile { get; private set; }
        public Tile tile { get; set; }
        public Path path { get; set; }
        public Player owner { get; }
        public CardType cardType { get; }
        public int castRange { get; }
        public string breadText { get; }
        public Location location => pile.location;

        public List<Ability> abilities = new List<Ability>();
        public Ability[] usableHere => abilities.Where(a => a.usableIn == location.pile).ToArray();


        public readonly Modifiable<int> power;
        public readonly Modifiable<int> toughness;
        public readonly Modifiable<int> movement;

        private Modifiable[] ms;

        public Card(CardTemplate ct, Player owner)
        {
            int basePower = -1;
            int baseToughness = -1;
            int baseMovement = -1;

            #region oophell
            switch (ct)
            {
                case CardTemplate.Hero:
                {
                    cardType = CardType.Hero;
                    image = Properties.Resources.pepeman;
                    baseMovement = 1;
                    basePower = 1;
                    baseToughness = 10;
                } break;

                case CardTemplate.Jordan:
                {
                    image = Properties.Resources.jordanno;
                    baseMovement = 2;
                    basePower = 1;
                    baseToughness = 2;
                    cardType = CardType.Creature;
                } break;

                case CardTemplate.Zap:
                {
                    image = Properties.Resources.Zap;
                    cardType = CardType.Instant;
                    castRange = 3;
                    Effect e = new Effect(new TargetSet(new ResolveRule(ResolveRule.Rule.CastCard), new PryRule<Card>(c => true)), Doer.ZepDoer(2));
                    abilities.Add(new Ability(PileLocation.Hand, castRange, e));
                    breadText = "Deal 2 damage to target creature";
                } break;

                case CardTemplate.AlterTime:
                {
                    throw new NotImplementedException();
                } break;

                default:
                    throw new Exception();
            }

            #endregion

            power = new Modifiable<int>(basePower);
            toughness = new Modifiable<int>(baseToughness);
            movement = new Modifiable<int>(baseMovement);

            ms = new Modifiable[]
            {
                power,
                toughness,
                movement,
            };

            if (cardType == CardType.Creature)
            {
                Effect e = new Effect(new TargetSet(
                    new ResolveRule(ResolveRule.Rule.CastCard),
                    new CastRule(targetable => targetable is Tile && ((Tile)targetable).card == null)),
                    Doer.MoveToTileDoer());

                    Ability castAbility = new Ability(PileLocation.Hand, 2, e);
                    abilities.Add(castAbility);
            }
            
            breadText = breadText ?? "";
            this.owner = owner;
            name = ct.ToString();
        }

        public void moveTo(Pile p)
        {
            pile?.remove(this);
            pile = p;
            p.addTop(this);
        }

        public void moveTo(Tile t)
        {
            tile?.removeCard();
            tile = t;
            t.place(this);
        }

        public void reherp(GameEvent e)
        {
            foreach (var v in ms)
            {
                v.check(e);
            }
        }
    }

    enum CardTemplate
    {
        Hero,
        Jordan,
        AlterTime,
        Zap,
    }

    enum CardType
    {
        Hero,
        Creature,
        Instant,
    }
}
