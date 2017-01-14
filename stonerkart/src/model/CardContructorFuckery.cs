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
                #region Illegal Goblin Laboratory
                case CardTemplate.Illegal_Goblin_Laboratory:
                {
                    cardType = CardType.Creature;

                    basePower = 0;
                    baseToughness = 4;
                    baseMovement = 0;

                    chaosCost = 1;
                    greyCost = 2;

                    addTriggeredAbility(
                        "At the end of your turn deal 1 damage to every enemy player.",
                        new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard), new CardResolveRule(CardResolveRule.Rule.VillainHeroes)),
                        new ZepperDoer(1),
                        new Cost(),
                        new TypedGameEventFilter<StartOfStepEvent>(e => e.step == Steps.End && e.activePlayer == controller),
                        0,
                        PileLocation.Field
                        );
                } break;
                #endregion
                #region Belwas
                case CardTemplate.Belwas:
                {
                    cardType = CardType.Creature;
                    race = Race.Human;
                    subtype = Subtype.Warrior;
                    rarity = Rarity.Legendary;
                    isHeroic = true;
                    forceColour = ManaColour.Life;

                    baseMovement = 2;
                    basePower = 2;
                    baseToughness = 25;
                } break;
                #endregion
                #region Kappa
                case CardTemplate.Kappa:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Uncommon;
                    race = Race.Lizard;

                    baseMovement = 1;
                    basePower = 2;
                    baseToughness = 3;
                    orderCost = 3;
                    greyCost = 1;

                    addTriggeredAbility(
                        "Whenever this creature enters the battlefield under your control, draw two cards.",
                        new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController)),
                        new DrawCardsDoer(2),
                        new Cost(),
                        new TypedGameEventFilter<MoveToPileEvent>(moveEvent => moveEvent.card == this && location.pile == PileLocation.Field),
                        0, 
                        PileLocation.Field,
                        TriggeredAbility.Timing.Post
                        );

                } break;
                #endregion
                #region Yung Lich
                case CardTemplate.Yung_Lich:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Rare;
                    race = Race.Undead;
                    subtype = Subtype.Wizard;

                    baseMovement = 2;
                    basePower = 2;
                    baseToughness = 2;
                    orderCost = 1;
                    deathCost = 1;
                    addTriggeredAbility(
                        "Whenever this creature enters the graveyard from the battlefield under your control, draw a card.",
                        new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController)),
                        new DrawCardsDoer(1),
                        new Cost(),
                        new TypedGameEventFilter<MoveToPileEvent>(moveEvent => moveEvent.card == this && moveEvent.to.location.pile == PileLocation.Graveyard && location.pile == PileLocation.Field),
                        0,
                        PileLocation.Field
                        );
                } break;
                #endregion
                #region Cantrip
                case CardTemplate.Cantrip:
                {
                    cardType = CardType.Instant;
                    rarity = Rarity.Common;

                    orderCost = 1;
                    castEffect = new Effect(new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController)),
                        new DrawCardsDoer(1));
                    castDescription = "Draw a card.";

                } break;
                #endregion
                #region Zap
                case CardTemplate.Zap:
                {
                    cardType = CardType.Instant;
                    rarity = Rarity.Common;

                    castRange = 3;
                    chaosCost = 1;

                    castEffect = new Effect(new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard), new PryCardRule(c => true)), 
                        new ZepperDoer(2));
                    castDescription = "Deal 2 damage to target creature.";
                } break;
                #endregion
                #region Temple Healer
                case CardTemplate.Temple_Healer:
                {
                    cardType = CardType.Creature;
                    race = Race.Human;
                    subtype = Subtype.Cleric;
                    rarity = Rarity.Uncommon;

                    basePower = 3;
                    baseToughness = 4;
                    baseMovement = 2;

                    lifeCost = 2;
                    greyCost = 2;

                    addTriggeredAbility(
                        "Whenever a creature enters the battlefield under your control, gain 1 life.",
                        new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard),new CardResolveRule(CardResolveRule.Rule.ResolveControllerCard)),
                        new ZepperDoer(-1),
                        new Cost(),
                        new TypedGameEventFilter<MoveToPileEvent>(moveEvent =>
                           moveEvent.card.controller == this.controller &&
                           moveEvent.to.location.pile == PileLocation.Field &&
                           this.location.pile == PileLocation.Field),
                        0,
                        PileLocation.Field
                        );
                    } break;
                #endregion
                #region Nature Heroman
                case CardTemplate.Nature_Heroman:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Legendary;
                    isHeroic = true;
                    baseMovement = 3;
                    basePower = 1;
                    baseToughness = 20;
                    forceColour = ManaColour.Nature;
                } break;
                #endregion
                #region Risen Abberation
                case CardTemplate.Risen_Abberation:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Common;

                    basePower = 2;
                    baseToughness = 2;
                    baseMovement = 1;

                    deathCost = 1;
                    } break;
                #endregion
                #region Shibby Shtank
                case CardTemplate.Shibby_Shtank:
                {
                    cardType = CardType.Creature;
                    race = Race.Human;
                    subtype = Subtype.Wizard;
                    rarity = Rarity.Legendary;
                    isHeroic = true;
                    forceColour = ManaColour.Order;

                    baseMovement = 1;
                    basePower = 1;
                    baseToughness = 20;

                    ActivatedAbility a = new ActivatedAbility(PileLocation.Field, 0, 
                        new Cost(new ManaCost(0, 0, 0, 0, 0, 2, 2)),
                        CastSpeed.Instant, 
                        String.Format("{1}{0}{0}: Draw a card", G.coloured(ManaColour.Order), G.colourless(2)),
                        new Effect(new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController)), new DrawCardsDoer(1)));
                    activatedAbilities.Add(a);
                } break;
                #endregion
                #region Unmake
                case CardTemplate.Unmake:
                {
                    cardType = CardType.Instant;
                    rarity = Rarity.Common;

                    castRange = 4;
                    orderCost = 1;
                    greyCost = 1;

                    castEffect = new Effect(new TargetRuleSet(new PryCardRule(c => !c.isHeroic)), 
                        new ToOwners(PileLocation.Hand));
                    castDescription = "Return target non-heroic creature to its owner's hand.";
                } break;
                #endregion
                #region Frothing Goblin
                case CardTemplate.Frothing_Goblin:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Uncommon;
                    race = Race.Goblin;

                    baseMovement = 2;
                    basePower = 2;
                    baseToughness = 2;
                    mightCost = 1;
                } break;
                #endregion
                #region Bear Cavalary
                case CardTemplate.Bear_Cavalary:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Common;

                    baseMovement = 3;
                    basePower = 3;
                    baseToughness = 3;
                    mightCost = 2;
                } break;
                #endregion 

                case CardTemplate.missingno:
                {
                    cardType = CardType.Creature;

                    baseToughness = 1;

                    addTriggeredAbility(
                        "x",
                        new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard), new PryCardRule()),
                        new ZepperDoer(1),
                        new Cost(),
                        new TypedGameEventFilter<PlaceOnTileEvent>(e => e.card == this),
                        2,
                        PileLocation.Field,
                        TriggeredAbility.Timing.Post
                        );
                    } break;

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
                    new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                    new PryTileRule(t => t.card == null)),
                    new MoveToTileDoer());
            }
            else throw new Exception();

            if (castEffect == null) throw new Exception();

            castAbility = new ActivatedAbility(PileLocation.Hand, castRange, new Cost(cmc), castSpeed, castDescription, castEffect);
            activatedAbilities.Add(castAbility);

            this.owner = owner;
            controller = owner;


            name = ct.ToString().Replace("_a", "'").Replace('_', ' ');
        }

        private void addTriggeredAbility(string description, TargetRuleSet trs, Doer doer, Cost cost, GameEventFilter filter, int castRange, PileLocation activeIn, TriggeredAbility.Timing timing = TriggeredAbility.Timing.Pre)
        {
            Effect e = new Effect(trs, doer);
            TriggeredAbility ta = new TriggeredAbility(this, activeIn, new[] { e }, castRange, cost, filter, timing, description);
            triggeredAbilities.Add(ta);
        }

    }
}
