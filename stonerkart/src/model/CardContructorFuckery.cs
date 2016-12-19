﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    partial class Card
    {
        public Card(CardTemplate ct, Player owner = null, bool isDummy = false)
        {
            template = ct;
            this.isDummy = isDummy;


            int basePower = -1;
            int baseToughness = -1;
            int baseMovement = -1;

            int chaosCost = 0,
                deathCost = 0,
                lifeCost = 0,
                mightCost = 0,
                natureCost = 0,
                orderCost = 0,
                greyCost = 0;

            int castRange = -1;
            Effect castEffect = null;
            string castDescription = "";
            CastSpeed castSpeed;


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


                    Effect e = new Effect(new TargetRuleSet(new ResolveRule(ResolveRule.Rule.ResolveController)), new DrawCardsDoer(2));
                    Cost c = new Cost();
                    GameEventFilter t =
                        new TypedGameEventFilter<MoveToPileEvent>(
                            moveEvent =>
                            {
                                if (moveEvent.card == this && location.pile == PileLocation.Field)
                                {
                                    return true;
                                }
                                return false;
                            });
                    TriggeredAbility ta = new TriggeredAbility(
                        this, PileLocation.Field, new []{e}, 0, 
                        c, t, TriggeredAbility.Timing.Post, 
                        "Whenever Kappa enters the battlefield under your control you draw two cards.");
                    triggeredAbilities.Add(ta);

                        cardType = CardType.Creature;
                } break;

                case CardTemplate.Cantrip:
                {
                    cardType = CardType.Sorcery;
                    rarity = Rarity.Common;

                    orderCost = 1;
                    castEffect = new Effect(new TargetRuleSet(new ResolveRule(ResolveRule.Rule.ResolveController)),
                        new DrawCardsDoer(1));
                    castDescription = "Draw a card.";

                } break;

                case CardTemplate.Zap:
                {
                    cardType = CardType.Instant;
                    rarity = Rarity.Common;

                    castRange = 3;
                    chaosCost = 1;
                    castEffect = new Effect(new TargetRuleSet(new ResolveRule(ResolveRule.Rule.ResolveCard), new PryCardRule(c => true)), new ZepperDoer(2));
                    castDescription = "Deal 2 damage to target creature.";
                } break;

                case CardTemplate.Temple_Healer:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Uncommon;

                    basePower = 3;
                    baseToughness = 4;
                    baseMovement = 2;

                    lifeCost = 2;
                    greyCost = 2;
                }
                    break;

                default:
                    throw new Exception();
            }


            #endregion

            ManaCost cmc = new ManaCost(chaosCost, deathCost, lifeCost, mightCost, natureCost, orderCost, greyCost);

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


            if (cardType == CardType.Instant)
            {
                castSpeed = CastSpeed.Instant;
            }
            else if (cardType == CardType.Sorcery)
            {
                castSpeed = CastSpeed.Slow;
            }
            else if (cardType == CardType.Creature)
            {
                castSpeed = CastSpeed.Slow;
                castRange = 2;
                castEffect = new Effect(new TargetRuleSet(
                    new ResolveRule(ResolveRule.Rule.ResolveCard),
                    new PryTileRule(t => t.card == null)),
                    new MoveToTileDoer(null));
            }
            else throw new Exception();

            if (castEffect == null) throw new Exception();

            castAbility = new ActivatedAbility(PileLocation.Hand, castRange, new Cost(cmc), castSpeed, castDescription, castEffect);
            activatedAbilities.Add(castAbility);

            this.owner = owner;
            controller = owner;


            name = ct.ToString().Replace('_', ' ');
        }
    }
}
