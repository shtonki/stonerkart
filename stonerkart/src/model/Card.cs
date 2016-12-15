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
        public Player owner { get; }
        public CardTemplate template { get; }
        public CardType cardType { get; }
        public string breadText { get; }

        public Location location => pile.location;
        public ActivatedAbility castAbility;
        public int castRange => castAbility.castRange;
        public ManaSet castManaCost => castAbility.cost.getSubCost<ManaCost>().cost;


        public List<Ability> abilities = new List<Ability>();
        public ActivatedAbility[] usableHere => abilities.Where(a => a is ActivatedAbility && a.activeIn == location.pile).Cast<ActivatedAbility>().ToArray();
        public List<ManaColour> colours => coloursEx();


        public bool isHeroic { get; }
        public Modifiable<int> power { get; }
        public Modifiable<int> toughness { get; }
        public Modifiable<int> movement { get; }



        private Modifiable[] ms;

        public Card(CardTemplate ct, Player owner = null)
        {
            template = ct;

            int basePower = -1;
            int baseToughness = -1;
            int baseMovement = -1;

            int chaosCost = 0,
                deathCost = 0,
                lifeCost = 0,
                mightCost = 0,
                natureCost = 0 ,
                orderCost = 0;
            int castRange = -1;
            Effect castEffect = null;


            #region oophell
            switch (ct)
            {
                case CardTemplate.Hero:
                {
                    cardType = CardType.Creature;
                    isHeroic = true;
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
                    orderCost = 1;
                    cardType = CardType.Creature;
                } break;

                case CardTemplate.Zap:
                {
                    image = Properties.Resources.Zap;
                    cardType = CardType.Instant;

                    castEffect = new Effect(new TargetRuleSet(new ResolveRule(ResolveRule.Rule.ResolveCard), new PryCardRule(c => true)), new ZepperDoer(2));
                    castRange = 3;
                    chaosCost = 1;

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
            
            ManaCost cmc = new ManaCost(chaosCost, deathCost, lifeCost, mightCost, natureCost, orderCost);

            power = new Modifiable<int>(basePower);
            toughness = new Modifiable<int>(baseToughness);
            movement = new Modifiable<int>(baseMovement);

            ms = new Modifiable[]
            {
                power,
                toughness,
                movement,
            };

            if (castEffect != null)
            {
                castAbility = new ActivatedAbility(PileLocation.Hand, castRange, new Cost(cmc), castEffect);
            }

            if (cardType == CardType.Creature)
            {
                Effect e = new Effect(new TargetRuleSet(
                    new ResolveRule(ResolveRule.Rule.ResolveCard),
                    new PryTileRule(t => t.card == null)),
                    new MoveToTileDoer());

                    castAbility = new ActivatedAbility(PileLocation.Hand, 2, new Cost(cmc), e);
                    abilities.Add(castAbility);
            }
            else if (castAbility == null) throw new Exception();
            
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

        public List<ManaColour> coloursEx()
        {
            return castManaCost.orbs.ToList();
        }

        public int abilityOrd(Ability a)
        {
            return abilities.IndexOf(a);
        }

        public Ability abilityFromOrd(int i)
        {
            return abilities[i];
        }

        public void reherp(GameEvent e)
        {
            foreach (var v in ms)
            {
                v.check(e);
            }
        }

        public override string ToString()
        {
            return name;
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
        Creature,
        Instant,
    }
}
