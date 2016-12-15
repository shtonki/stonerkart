using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Card : Observable<CardChangedMessage>, Stuff, Targetable, Observer<int>
    {
        public string name { get; }
        public Pile pile { get; private set; }
        public Tile tile { get; set; }
        public Player owner { get; }
        public Player controller { get; }
        public CardTemplate template { get; }
        public CardType cardType { get; }
        public Rarity rarity { get; }
        public string breadText { get; }

        public Location location => pile.location;
        public ActivatedAbility castAbility { get; }
        public int castRange => castAbility.castRange;
        public ManaSet castManaCost => castAbility.cost.getSubCost<ManaCost>().cost;

        public string typeText => typeTextEx();

        public List<Ability> abilities = new List<Ability>();
        public ActivatedAbility[] usableHere => abilities.Where(a => a is ActivatedAbility && a.activeIn == location.pile).Cast<ActivatedAbility>().ToArray();
        /// <summary>
        /// Returns a list containing the unique colours of the card. If the card has no mana cost it returns an 
        /// array containing only ManaColour.Colourless.
        /// </summary>
        public List<ManaColour> colours => coloursEx();


        public bool isHeroic { get; }
        public Modifiable<int> power { get; }
        public Modifiable<int> toughness { get; }
        public Modifiable<int> movement { get; }


        private ManaColour? forceColour;
        private Modifiable[] modifiables;

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
                natureCost = 0,
                orderCost = 0,
                colourlessCost = 0;
            int castRange = -1;
            Effect castEffect = null;
            Rarity rarity;


            #region oophell
            switch (ct)
            {
                case CardTemplate.Belwas:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Rare;
                    isHeroic = true;
                    baseMovement = 4;
                    basePower = 1;
                    baseToughness = 10;
                    forceColour = ManaColour.Life;
                } break;

                case CardTemplate.Kappa:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Common;
                    baseMovement = 2;
                    basePower = 1;
                    baseToughness = 1;
                    orderCost = 1;
                    cardType = CardType.Creature;
                } break;

                case CardTemplate.Zap:
                {
                    cardType = CardType.Instant;
                    rarity = Rarity.Common;

                    castEffect = new Effect(new TargetRuleSet(new ResolveRule(ResolveRule.Rule.ResolveCard), new PryCardRule(c => true)), new ZepperDoer(2));
                    castRange = 3;
                    chaosCost = 1;

                    breadText = "Deal 2 damage to target creature.";
                } break;

                default:
                    throw new Exception();
            }

            #endregion
            
            ManaCost cmc = new ManaCost(chaosCost, deathCost, lifeCost, mightCost, natureCost, orderCost, colourlessCost);

            power = new Modifiable<int>(basePower);
            power.addObserver(this);
            toughness = new Modifiable<int>(baseToughness);
            toughness.addObserver(this);
            movement = new Modifiable<int>(baseMovement);
            movement.addObserver(this);

            modifiables = new Modifiable[]
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
            }

            else if (castAbility == null) throw new Exception();
            abilities.Add(castAbility);

            breadText = breadText ?? "";

            this.owner = owner;
            controller = owner;

            this.rarity = rarity;

            name = ct.ToString().Replace('_', ' ');
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

        public void exhaust()
        {
            movement.modify(-movement, ModifiableSchmoo.intAdd, ModifiableSchmoo.startOfOwnersTurn(this));
        }

        private List<ManaColour> coloursEx()
        {
            if (forceColour.HasValue) return new List<ManaColour>(new ManaColour[] { forceColour.Value });
            HashSet<ManaColour> hs = new HashSet<ManaColour>(castManaCost.orbs.Where(x => x != ManaColour.Colourless));
            if (hs.Count == 0) return new List<ManaColour>(new ManaColour[] {ManaColour.Colourless,});
            
            return hs.ToList();
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
            foreach (Modifiable modifiable in modifiables)
            {
                modifiable.check(e);
            }
        }

        private string typeTextEx()
        {
            string s = isHeroic ? "Heroic " : "";
            s += cardType.ToString(); 

            return s;
        }

        public void notify(int t)
        {
            notify(new CardChangedMessage());
        }

        public override string ToString()
        {
            return name;
        }
    }

    enum CardTemplate
    {
        Belwas,
        Zap,
        Kappa,
    }

    enum CardType
    {
        Creature,
        Instant,
    }

    enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary,
    }
}
