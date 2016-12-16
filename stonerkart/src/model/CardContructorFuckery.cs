using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    partial class Card
    {
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
                    }
                    break;

                case CardTemplate.Kappa:
                    {
                        cardType = CardType.Creature;
                        rarity = Rarity.Common;
                        baseMovement = 2;
                        basePower = 1;
                        baseToughness = 1;
                        orderCost = 1;
                        cardType = CardType.Creature;
                    }
                    break;

                case CardTemplate.Zap:
                    {
                        cardType = CardType.Instant;
                        rarity = Rarity.Common;

                        castEffect = new Effect(new TargetRuleSet(new ResolveRule(ResolveRule.Rule.ResolveCard), new PryCardRule(c => true)), new ZepperDoer(2));
                        castRange = 3;
                        chaosCost = 1;

                        breadText = "Deal 2 damage to target creature.";
                    }
                    break;

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

            CastSpeed castSpeed;

            if (castEffect != null)
            {

                if (cardType == CardType.Instant)
                {
                    castSpeed = CastSpeed.Instant;
                }
                else if (cardType == CardType.Sorcery)
                {
                    castSpeed = CastSpeed.Slow;
                }
                else throw new Exception();
                castAbility = new ActivatedAbility(PileLocation.Hand, castRange, new Cost(cmc), castSpeed, castEffect);
            }

            if (cardType == CardType.Creature)
            {


                Effect e = new Effect(new TargetRuleSet(
                    new ResolveRule(ResolveRule.Rule.ResolveCard),
                    new PryTileRule(t => t.card == null)),
                    new MoveToTileDoer());

                castAbility = new ActivatedAbility(PileLocation.Hand, 2, new Cost(cmc), CastSpeed.Slow, e);
            }

            else if (castAbility == null) throw new Exception();
            abilities.Add(castAbility);

            breadText = breadText ?? "";

            this.owner = owner;
            controller = owner;

            this.rarity = rarity;

            name = ct.ToString().Replace('_', ' ');
        }
    }
}
