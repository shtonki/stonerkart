#define testx
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
            List<Effect> additionalCastEffects = new List<Effect>();
            string castDescription = "";
            CastSpeed castSpeed;
            List<Effect> additionalCastCosts = new List<Effect>();
            keywordAbilities = new List<KeywordAbility>();


            #region oophell

            switch (ct)
            {
                    #region Illegal Goblin Laboratory

                case CardTemplate.Illegal_sGoblin_sLaboratory:
                {
                    cardType = CardType.Relic;
                    rarity = Rarity.Uncommon;

                    chaosCost = 2;
                    greyCost = 1;

                    addTriggeredAbility(
                        "At the end of your turn deal 1 damage to every enemy player.",
                        new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                            new CardResolveRule(CardResolveRule.Rule.VillainHeroes)),
                        new ZepperDoer(1),
                        new Foo(),
                        new TypedGameEventFilter<StartOfStepEvent>(
                            e => e.step == Steps.End && e.activePlayer == controller),
                        0,
                        PileLocation.Field,
                        false
                        );
                }
                    break;

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

                        addActivatedAbility(
                            String.Format("{0}: Give all your other white creatures +1/+0 until end of turn.", G.exhaustGhyph),
                            new TargetRuleSet(new AllCardsRule(c => c != this && c.controller == this.controller && c.isColour(ManaColour.Life))),
                            new ModifyDoer(ModifiableStats.Power, 1, LL.add, LL.endOfTurn),
                            LL.exhaustThis,
                            0,
                            PileLocation.Field,
                            CastSpeed.Instant 
                            );
                }
                    break;

                    #endregion

                    #region Kappa

                case CardTemplate.Kappa:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Uncommon;
                    race = Race.Beast;

                    baseMovement = 2;
                    basePower = 2;
                    baseToughness = 3;
                    orderCost = 3;
                    greyCost = 1;

                    addTriggeredAbility(
                        "Whenever this creature enters the battlefield under your control, draw two cards.",
                        new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController)),
                        new DrawCardsDoer(2),
                        new Foo(),
                        new TypedGameEventFilter<MoveToPileEvent>(
                            moveEvent => moveEvent.card == this && location.pile == PileLocation.Field),
                        0,
                        PileLocation.Field,
                        false,
                        TriggeredAbility.Timing.Post
                        );

                }
                    break;

                    #endregion

                    #region Yung Lich

                case CardTemplate.Yung_sLich:
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
                        new Foo(),
                        new TypedGameEventFilter<MoveToPileEvent>(
                            moveEvent =>
                                moveEvent.card == this && moveEvent.to.location.pile == PileLocation.Graveyard &&
                                location.pile == PileLocation.Field),
                        0,
                        PileLocation.Field,
                        false
                        );
                }
                    break;

                    #endregion

                    #region Cantrip

                case CardTemplate.Cantrip:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Common;

                    orderCost = 1;
                    castEffect =
                        new Effect(new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController)),
                            new DrawCardsDoer(1));
                    additionalCastEffects.Add(
                        new Effect(
                            new SelectCardRule(PileLocation.Hand, c => false,
                                new PryPlayerRule(p => true,
                                    new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController)),
                                SelectCardRule.Mode.Resolver),
                            new ModifyDoer(ModifiableStats.Movement, 0, LL.add, LL.clearAura)));
                    castDescription = "Look at target players hand. Draw a card.";

                }
                    break;

                    #endregion

                    #region Zap

                case CardTemplate.Zap:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Common;

                    castRange = 3;
                    chaosCost = 1;

                    castEffect =
                        new Effect(
                            new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                                new PryCardRule(c => true,
                                    new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController))),
                            new ZepperDoer(2));
                    castDescription = "Deal 2 damage to target creature.";
                }
                    break;

                    #endregion

                    #region Temple Healer

                case CardTemplate.Temple_sHealer:
                {
                    cardType = CardType.Creature;
                    race = Race.Human;
                    subtype = Subtype.Cleric;
                    rarity = Rarity.Uncommon;

                    basePower = 3;
                    baseToughness = 4;
                    baseMovement = 3;

                    lifeCost = 2;
                    greyCost = 2;

                    addTriggeredAbility(
                        "Whenever a creature enters the battlefield under your control, you may restore 1 toughness to your hero.",
                        new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                            new CardResolveRule(CardResolveRule.Rule.ResolveControllerCard)),
                        new ZepperDoer(-1),
                        new Foo(),
                        new TypedGameEventFilter<MoveToPileEvent>(moveEvent =>
                            moveEvent.card.controller == controller &&
                            moveEvent.to.location.pile == PileLocation.Field &&
                            location.pile == PileLocation.Field),
                        0,
                        PileLocation.Field,
                        true,
                        TriggeredAbility.Timing.Post
                        );
                }
                    break;

                    #endregion

                    #region Nature Heroman

                case CardTemplate.Chieftain_sZ_aloot_aboks:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Legendary;
                    race = Race.Human;
                    subtype = Subtype.Warrior;
                    isHeroic = true;
                    baseMovement = 3;
                    basePower = 1;
                    baseToughness = 20;
                    forceColour = ManaColour.Nature;

                    addActivatedAbility(
                        String.Format("{0}: Exhaust target creature.", G.exhaustGhyph),
                        new TargetRuleSet(new PryCardRule()),
                        new FatigueDoer(true),
                        LL.exhaustThis,
                        3,
                        PileLocation.Field,
                        CastSpeed.Instant
                        );
                }
                    break;

                    #endregion

                    #region Risen Abberation

                case CardTemplate.Risen_sAbberation:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Common;
                    race = Race.Zombie;

                    basePower = 2;
                    baseToughness = 2;
                    baseMovement = 2;

                    deathCost = 1;
                }
                    break;

                    #endregion

                    #region Shibby Shtank

                case CardTemplate.Shibby_sShtank:
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


                    addActivatedAbility(
                        String.Format("{1}{0}{0}: Draw a card", G.coloured(ManaColour.Order), G.colourless(2)),
                        new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController)),
                        new DrawCardsDoer(1),
                        fooFromManaCost(ManaColour.Order, ManaColour.Order, ManaColour.Colourless, ManaColour.Colourless),
                        0,
                        PileLocation.Field,
                        CastSpeed.Instant
                        );

                }
                    break;

                    #endregion

                    #region Unmake

                case CardTemplate.Unmake:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Common;

                    castRange = 4;
                    orderCost = 1;
                    greyCost = 1;

                    castEffect =
                        new Effect(
                            new TargetRuleSet(new PryCardRule(c => !c.isHeroic,
                                new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController))),
                            new MoveToPileDoer(PileLocation.Hand));
                    castDescription = "Return target non-heroic creature to its owner's hand.";
                }
                    break;

                    #endregion

                    #region Rockhand Ogre

                case CardTemplate.Rockhand_sEchion:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Uncommon;
                    race = Race.Giant;

                    baseMovement = 3;
                    basePower = 2;
                    baseToughness = 2;
                    mightCost = 1;
                }
                    break;

                    #endregion

                    #region Primeordial Chimera

                case CardTemplate.Primordial_sChimera:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Common;
                    race = Race.Beast;

                    baseMovement = 3;
                    basePower = 3;
                    baseToughness = 3;
                    mightCost = 2;
                }
                    break;

                    #endregion

                    #region Cleansing Fire

                case CardTemplate.Cleansing_sFire:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Rare;

                    chaosCost = 1;
                    lifeCost = 1;

                    castRange = 4;
                    castEffect =
                        new Effect(
                            new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard), new PryCardRule()),
                            new ZepperDoer(3));
                    additionalCastEffects.Add(
                        new Effect(
                            new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                                new CardResolveRule(CardResolveRule.Rule.ResolveControllerCard)),
                            new ZepperDoer(-3)));
                    castDescription = "Deal 3 damage to target creature. You gain 3 life.";


                }
                    break;

                    #endregion

                    #region Goblin Grenade

                case CardTemplate.Goblin_sGrenade:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Common;

                    chaosCost = 2;
                    greyCost = 1;

                    castRange = 5;
                    castEffect =
                        new Effect(
                            new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                                new AoeRule(t => true, 2, c => true)),
                            new ZepperDoer(1));
                    castDescription = "Deal 1 damage to all creatures within 2 tiles of target tile.";

                }
                    break;

                    #endregion

                    #region Teleport

                case CardTemplate.Teleport:
                {
                    cardType = CardType.Channel;
                    rarity = Rarity.Common;

                    orderCost = 2;
                    castRange = 5;
                    castEffect = new Effect(
                        new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveControllerCard),
                            new PryTileRule(f => f.passable,
                                new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController))), new MoveToTileDoer(true));
                    castDescription = "Move your hero to target tile.";
                }
                    break;

                    #endregion

                    #region One With Nature

                case CardTemplate.One_sWith_sNature:
                {
                    cardType = CardType.Channel;
                    rarity = Rarity.Rare;

                    natureCost = 1;

                    castEffect =
                        new Effect(new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController)),
                            new GainBonusManaDoer(ManaColour.Nature, ManaColour.Nature, ManaColour.Nature));
                    castDescription = String.Format("You gain {0}{0}{0} until the end of the step.",
                        G.coloured(ManaColour.Nature));
                }
                    break;

                    #endregion

                    #region Graverobber Syrdin

                case CardTemplate.Graverobber_sSyrdin:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Legendary;
                    race = Race.Human;
                    subtype = Subtype.Rogue;

                    baseMovement = 3;
                    basePower = 3;
                    baseToughness = 3;

                    deathCost = 1;
                    lifeCost = 1;
                    natureCost = 1;

                    addTriggeredAbility(
                        "Whenever this creature enters the battlefield under your control, you may return a card from your graveyard to your hand.",
                        new TargetRuleSet(new SelectCardRule(PileLocation.Graveyard)),
                        new MoveToPileDoer(PileLocation.Hand),
                        new Foo(),
                        new TypedGameEventFilter<MoveToPileEvent>(
                            moveEvent => moveEvent.card == this && moveEvent.to.location.pile == PileLocation.Field),
                        0,
                        PileLocation.Field,
                        true,
                        TriggeredAbility.Timing.Post
                        );

                }
                    break;

                    #endregion

                    #region Alter Fate

                case CardTemplate.Alter_sFate:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Common;

                    orderCost = 1;

                    castEffect =
                        new Effect(
                            new TargetRuleSet(new SelectCardRule(PileLocation.Deck)),
                            new MoveToPileDoer(PileLocation.Deck));
                    castDescription =
                        "Search your deck for a card. Shuffle your deck then put the selected card on top.";
                }
                    break;

                    #endregion

                    #region Fresh Fox

                case CardTemplate.Fresh_sFox:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Uncommon;
                    race = Race.Beast;

                    baseToughness = 2;
                    basePower = 2;
                    baseMovement = 1;

                    //natureCost = 2;

                    keywordAbilities.Add(KeywordAbility.Fervor);
                }
                    break;

                    #endregion

                    #region Damage Ward

                case CardTemplate.Damage_sWard:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Uncommon;

                    lifeCost = 1;
                    castRange = 4;
                    castEffect = new Effect(
                        new TargetRuleSet(new PryCardRule()),
                        new ModifyDoer(ModifiableStats.Toughness, 3, LL.add, LL.never));
                    castDescription = "Target creature gains 3 toughness.";

                }
                    break;

                    #endregion

                    #region Survival Instincts

                case CardTemplate.Survival_sInstincts:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Common;

                    castRange = 3;
                    natureCost = 2;
                    castEffect =
                        new Effect(
                            new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard), new PryCardRule()),
                            new ZepperDoer(-2));
                    additionalCastEffects.Add(new Effect(new CopyPreviousRule<Card>(1),
                        new ModifyDoer(ModifiableStats.Power, 2, LL.add, LL.endOfTurn)));
                    castDescription =
                        "Target creature is healed for 2 and gains 2 power until the end of this turn.";
                }
                    break;

                    #endregion

                    #region Baby Dragon

                case CardTemplate.Baby_sDragon:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Common;
                    race = Race.Dragon;

                    basePower = 2;
                    baseToughness = 1;
                    baseMovement = 2;

                    chaosCost = 2;
                    addTriggeredAbility(
                        "When this creature enters the battlefield you may have it deal 1 damage to target creature within 3 tiles.",
                        new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard), new PryCardRule()),
                        new ZepperDoer(1),
                        new Foo(),
                        LL.thisEnters(this, PileLocation.Field),
                        3,
                        PileLocation.Field,
                        true,
                        TriggeredAbility.Timing.Post
                        );
                }
                    break;

                    #endregion

                    #region Huntress of Nibememe

                case CardTemplate.Huntress_sOf_sNibememe:
                {
                    cardType = CardType.Creature;
                    race = Race.Human;
                    subtype = Subtype.Warrior;

                    natureCost = 1;
                    greyCost = 2;

                    baseToughness = 3;
                    basePower = 1;
                    baseMovement = 3;

                    addActivatedAbility(
                        String.Format("{0}: Deal 1 damage to target creature within 3 tiles.", G.exhaustGhyph),
                        new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard), new PryCardRule()),
                        new ZepperDoer(1),
                        new Foo(new Effect(new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard)),
                            new FatigueDoer(true))),
                        3,
                        PileLocation.Field,
                        CastSpeed.Instant
                        );
                }
                    break;

                    #endregion

                    #region Call to arms

                case CardTemplate.Call_sTo_sArms:
                {
                    cardType = CardType.Channel;
                    rarity = Rarity.Common;

                    lifeCost = 1;
                    castRange = 2;

                    castDescription = "Summon two 1/1 Squire tokens.";
                    castEffect = new Effect(new TargetRuleSet(
                        new CreateTokenRule(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController),
                            CardTemplate.Squire, CardTemplate.Squire),
                        new PryTileRule(t => t.card == null && !t.isEdgy,
                            new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), true, 2, false)),
                        new MoveToTileDoer(),
                        true
                        );

                }
                    break;

                    #endregion

                    #region Wilt

                case CardTemplate.Wilt:
                {
                    cardType = CardType.Channel;
                    rarity = Rarity.Uncommon;

                    deathCost = 2;

                    castDescription = "Look at target players hand and select a creature. Selected card is discarded.";
                    castEffect =
                        new Effect(
                            new SelectCardRule(PileLocation.Hand,
                                c => c.cardType == CardType.Creature,
                                new PryPlayerRule(),
                                SelectCardRule.Mode.Resolver),
                            new MoveToPileDoer(PileLocation.Graveyard));
                }
                    break;

                    #endregion

                    #region Price Ila

                case CardTemplate.Prince_sIla:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Legendary;
                    race = Race.Undead;
                    subtype = Subtype.Rogue;
                    isHeroic = true;
                    forceColour = ManaColour.Death;

                    baseMovement = 2;
                    basePower = 2;
                    baseToughness = 20;

                    addActivatedAbility(
                        String.Format("{0}: Each player discards a card.", G.exhaustGhyph),
                        new TargetRuleSet(new SelectCardRule(PileLocation.Hand,
                            c => true,
                            new PlayerResolveRule(PlayerResolveRule.Rule.AllPlayers))),
                        new MoveToPileDoer(PileLocation.Graveyard),
                        new Foo(new Effect(new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard)),
                            new FatigueDoer(true))),
                        0,
                        PileLocation.Field,
                        CastSpeed.Slow
                        );
                }
                    break;

                    #endregion

                    #region Kraken

                case CardTemplate.Kraken:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Rare;
                    race = Race.Beast;

                    baseMovement = 2;
                    basePower = 0;
                    baseToughness = 5;

                    orderCost = 2;
                    greyCost = 3;

                    auras.Add(new Aura(
                        "This creature gets +1/+0 for each card in its controllers hand.",
                        () => controller.hand.Count,
                        LL.add,
                        ModifiableStats.Power,
                        c => c == this
                        ));
                }
                    break;

                    #endregion

                    #region Ilas Gravekeeper

                case CardTemplate.Ilas_sGravekeeper:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Uncommon;
                    race = Race.Undead;

                    baseMovement = 3;
                    basePower = 0;
                    baseToughness = 4;

                    deathCost = 2;

                    auras.Add(new Aura(
                        "This creature gets +1/+0 for each Zombie in its controllers graveyard.",
                        () => controller.graveyard.Count(c => c.race == Race.Zombie),
                        LL.add,
                        ModifiableStats.Power,
                        c => c == this
                        ));
                }
                    break;

                    #endregion

                    #region Frenzied Pirhana

                case CardTemplate.Frenzied_sPirhana:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Common;
                    race = Race.Beast;

                    baseMovement = 3;
                    baseToughness = 1;
                    basePower = 1;

                    natureCost = 1;

                    keywordAbilities.Add(KeywordAbility.Elusion);

                    auras.Add(new Aura(
                        "",
                        () => controller.graveyard.Count >= 7 ? 2 : 0,
                        LL.add,
                        ModifiableStats.Power,
                        c => c == this
                        ));

                    auras.Add(new Aura(
                        "This creature has gets +2/+2 as long as its controllers graveyard contains seven or more cards.",
                        () => controller.graveyard.Count >= 7 ? 2 : 0,
                        LL.add,
                        ModifiableStats.Toughness,
                        c => c == this
                        ));
                }
                    break;

                    #endregion

                    #region Ilatian Haunter

                case CardTemplate.Ilatian_sHaunter:
                {
                    cardType = CardType.Creature;
                    race = Race.Zombie;
                    rarity = Rarity.Common;

                    baseMovement = 2;
                    baseToughness = 1;
                    basePower = 1;

                    deathCost = 1;
                    greyCost = 1;

                    addActivatedAbility(
                        "You may cast this card from the graveyard.",
                        new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                            new PryTileRule(t => t.card == null && !t.isEdgy,
                                new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), true)),
                        new MoveToTileDoer(),
                        new Foo(LL.manaCost(new ManaSet(ManaColour.Colourless, ManaColour.Death))),
                        2,
                        PileLocation.Graveyard,
                        CastSpeed.Slow,
                        true
                        );
                }
                    break;

                    #endregion

                    #region Invigorate

                case CardTemplate.Invigorate:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Common;

                    natureCost = 1;

                    castDescription = "Return all movement to target creature.";
                    castEffect = new Effect(new PryCardRule(), new FatigueDoer(false));
                    castRange = 4;
                }
                    break;

                    #endregion

                    #region Counterspell

                case CardTemplate.Counterspell:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Common;

                    orderCost = 2;
                    greyCost = 1;

                    castDescription = "Counter target spell.";
                    castEffect = new Effect(new ClickCardRule(c => c.location.pile == PileLocation.Stack && !c.isDummy),
                        new MoveToPileDoer(PileLocation.Graveyard));
                }
                    break;

                    #endregion

                    #region Gleeful Duty

                case CardTemplate.Gleeful_sDuty:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Uncommon;

                    deathCost = 3;

                    castDescription = "Destroy target non-heroic creature.";
                    castEffect =
                        new Effect(
                            new PryCardRule(c => !c.isHeroic,
                                new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController)),
                            new MoveToPileDoer(PileLocation.Graveyard));
                    castRange = 4;
                }
                    break;

                    #endregion

                    #region Overgrow

                case CardTemplate.Overgrow:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Common;

                    natureCost = 2;

                    castDescription = "Destroy target relic.";
                    castEffect =
                        new Effect(
                            new PryCardRule(c => c.cardType == CardType.Relic,
                                new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController)),
                            new MoveToPileDoer(PileLocation.Graveyard));
                    castRange = 6;
                }
                    break;

                #endregion

                #region Gotterdammerun

                case CardTemplate.Gotterdammerung:
                {
                    cardType = CardType.Channel;
                    rarity = Rarity.Rare;

                    lifeCost = 3;
                    greyCost = 4;

                    castDescription = "Destroy all non-heroic creatures";
                    castEffect =
                        new Effect(
                            new AllCardsRule(c => !c.isHeroic), 
                            new MoveToPileDoer(PileLocation.Graveyard));
                    } break;

                #endregion

                #region Rider of Death

                case CardTemplate.Rider_sof_sDeath:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Legendary;

                    basePower = 3;
                    baseToughness = 7;
                    baseMovement = 4;

                    deathCost = 4;
                    greyCost = 3;

                    addTriggeredAbility(
                        "Whenever Rider of Death enters the battlefield you may destroy target non-heroic creature.",
                        new TargetRuleSet(new PryCardRule(c => !c.isHeroic)),
                        new MoveToPileDoer(PileLocation.Graveyard),
                        new Foo(),
                        new TypedGameEventFilter<MoveToPileEvent>(
                            e => e.card == this && e.to.location.pile == PileLocation.Field),
                        -1,
                        PileLocation.Field,
                        true,
                        TriggeredAbility.Timing.Post
                        );
                } break;

                #endregion

                #region Rider of War

                case CardTemplate.Rider_sof_sWar:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Legendary;

                    basePower = 3;
                    baseToughness = 7;
                    baseMovement = 4;

                    mightCost = 4;
                    greyCost = 3;

                    keywordAbilities.Add(KeywordAbility.Kingslayer);
                    keywordAbilities.Add(KeywordAbility.Fervor);
                } break;

                #endregion

                #region Rider of Famine
                case CardTemplate.Rider_sof_sPestilence:
                    {
                        cardType = CardType.Creature;
                    rarity = Rarity.Legendary;

                    basePower = 3;
                    baseToughness = 7;
                    baseMovement = 4;

                    chaosCost = 4;
                    greyCost = 3;
                        
                    addTriggeredAbility(
                        "Whenever Rider of Famine deals damage to a player that player discards a card.",
                        new TargetRuleSet(new SelectCardRule(PileLocation.Hand, c => true,
                            new TriggeredTargetRule<DamageEvent, Player>(e => e.target.controller))),
                        new MoveToPileDoer(PileLocation.Graveyard),
                        new Foo(),
                        new TypedGameEventFilter<DamageEvent>(e => e.source == this && e.target.isHeroic),
                        0,
                        PileLocation.Field,
                        false
                        );
                } break;
                #endregion

                #region Rider of Pestilence
                case CardTemplate.Rider_sof_sFamine:
                    {
                        cardType = CardType.Creature;
                        rarity = Rarity.Legendary;

                        basePower = 3;
                        baseToughness = 7;
                        baseMovement = 4;

                        natureCost = 4;
                        greyCost = 3;

                        addTriggeredAbility(
                            "Whenever Rider of Pestilence deals damage to a player that player sacrifices a non-heroic creature.",
                            new TargetRuleSet(new SelectCardRule(PileLocation.Field,
                                c => !c.isHeroic && c.cardType == CardType.Creature,
                                new TriggeredTargetRule<DamageEvent, Player>(e => e.target.controller))),
                            new MoveToPileDoer(PileLocation.Graveyard),
                            new Foo(),
                            new TypedGameEventFilter<DamageEvent>(e => e.source == this && e.target.isHeroic),
                            0,
                            PileLocation.Field,
                            false
                            );
                    }
                    break;
                #endregion

                #region Magma Vents
                case CardTemplate.Magma_sVents:
                {
                    cardType = CardType.Relic;
                    rarity = Rarity.Common;

                    chaosCost = 1;
                    orderCost = 1;
                    greyCost = 2;

                    addTriggeredAbility(
                        "Whenever a player draws a card Magma Vents deals 1 damage to that player.",
                        new TargetRuleSet(
                            new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                            new TriggeredTargetRule<DrawEvent, Card>(g => g.player.heroCard)),
                        new ZepperDoer(1),
                        new Foo(),
                        new TypedGameEventFilter<DrawEvent>(),
                        0,
                        PileLocation.Field,
                        false 
                        );
                } break;
                #endregion

                #region Chains of Gold
                    case CardTemplate.Chains_sof_sVirtue:
                {
                    cardType = CardType.Channel;
                    rarity = Rarity.Common;

                    lifeCost = 2;

                    castDescription = "Set target non-life creatures movement to 1.";
                    castEffect = new Effect(new PryCardRule(c => !c.isColour(ManaColour.Life)),
                        new ModifyDoer(ModifiableStats.Movement, 1, LL.set, LL.never));
                    castRange = 4;
                } break;
                #endregion

                #region Chains of Iron
                case CardTemplate.Chains_sof_sSin:
                    {
                        cardType = CardType.Channel;
                        rarity = Rarity.Common;

                        deathCost = 2;

                        castDescription = "Reduce target non-death creatures movement by 2.";
                        castEffect = new Effect(new PryCardRule(c => !c.isColour(ManaColour.Death)),
                            new ModifyDoer(ModifiableStats.Movement, -2, LL.add, LL.never));
                        castRange = 4;
                    }
                    break;
                #endregion

                #region Abolish

                case CardTemplate.Abolish:
                {
                    cardType = CardType.Channel;
                    rarity = Rarity.Common;

                    lifeCost = 2;
                    greyCost = 1;

                        castDescription = "Destroy target relic.";
                        castEffect =
                            new Effect(
                                new PryCardRule(c => c.cardType == CardType.Relic,
                                    new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController)),
                                new MoveToPileDoer(PileLocation.Graveyard));
                        castRange = 6;
                    } break;

                #endregion

                case CardTemplate.Deep_sFry:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Uncommon;

                    chaosCost = 2;
                    greyCost = 1;

                    castDescription = "Deal 1 damage to and exhaust target creature.";
                    castEffect =
                        new Effect(
                            new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard), new PryCardRule()),
                            new ZepperDoer(1));
                    additionalCastEffects.Add(new Effect(new CopyPreviousRule<Card>(1), new FatigueDoer(true)));
                    castRange = 5;

                } break;

                #region tokens
                #region Squire
                case CardTemplate.Squire:
                {
                    forceColour = ManaColour.Life;
                    basePower = 1;
                    baseToughness = 1;
                    baseMovement = 2;
                    race = Race.Human;
                    subtype = Subtype.Warrior;
                    isToken = true;
                } break;
                #endregion
                #endregion

                default:
                {
                    throw new Exception("missing cardtemplate in switch");
                }
            }

            #endregion

            int[] x = new int[ManaSet.size];
#if !test
            x[(int)ManaColour.Chaos] = chaosCost;
            x[(int)ManaColour.Colourless] = greyCost;
            x[(int)ManaColour.Death] = deathCost;
            x[(int)ManaColour.Life] = lifeCost;
            x[(int)ManaColour.Might] = mightCost;
            x[(int)ManaColour.Nature] = natureCost;
            x[(int)ManaColour.Order] = orderCost;
#else
            baseMovement = 10;
            castRange = 10;
#endif
            castManaCost = new ManaSet(x);

            Power = new Modifiable(basePower, 0);
            Toughness = new Modifiable(baseToughness, 0);
            Movement = new Modifiable(baseMovement, 0);

            modifiables = new Modifiable[]
            {
                Power,
                Toughness,
                Movement,
            };


            if (cardType == CardType.Interrupt)
            {
                castSpeed = CastSpeed.Instant;
            }
            else if (cardType == CardType.Channel)
            {
                castSpeed = CastSpeed.Slow;
            }
            else if (cardType == CardType.Creature || cardType == CardType.Relic)
            {
                castSpeed = CastSpeed.Slow;
                castRange = 2;
                castEffect = new Effect(new TargetRuleSet(
                    new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                    new PryTileRule(t => t.card == null && !t.isEdgy, new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), true)),
                    new MoveToTileDoer());
            }
            else throw new Exception();
            
            if (castEffect == null) throw new Exception("these don't show up anyway");
            List<Effect> es = new List<Effect>();

            es.Add(castEffect);
            es.AddRange(additionalCastEffects);

            additionalCastCosts.Add(LL.manaCost(castManaCost));

            castAbility = new ActivatedAbility(this, PileLocation.Hand, castRange, new Foo(additionalCastCosts.ToArray()), castSpeed, castDescription, es.ToArray());
            abilities.Add(castAbility);

            this.owner = owner;
            controller = owner;

            name = ct.ToString().Replace("_a", "'").Replace("_s", " ");

            eventHandler = generatedlft();
        }

        private void addTriggeredAbility(string description, TargetRuleSet trs, Doer doer, Foo cost, GameEventFilter filter, int castRange, PileLocation activeIn, bool optional, TriggeredAbility.Timing timing = TriggeredAbility.Timing.Pre)
        {
            Effect e = new Effect(trs, doer);
            TriggeredAbility ta = new TriggeredAbility(this, activeIn, new[] { e }, castRange, cost, filter, optional, timing, description);
            abilities.Add(ta);
        }

        private void addActivatedAbility(string description, TargetRuleSet trs, Doer doer, Foo cost, int castRange, PileLocation activeIn, CastSpeed castSpeed, bool alternateCast = false)
        {
            Effect e = new Effect(trs, doer);
            ActivatedAbility aa = new ActivatedAbility(this, activeIn, castRange, cost, castSpeed, description, e);
            abilities.Add(aa);
            if (alternateCast) alternateCasts.Add(aa);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] //yagni said no one ever
        private static Foo fooFromManaCost(params ManaColour[] cs)
        {
            return new Foo(new Effect(new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController),
                new ManaCostRule(cs)), new PayManaDoer()));
        }

    }
}
