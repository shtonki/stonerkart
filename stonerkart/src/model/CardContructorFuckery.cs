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
                    String.Format("{2}{1}{1}, {0}: Your other white creatures get +1/+0 until end of turn. {3}",
                        G.exhaustGhyph, G.colouredGlyph(ManaColour.Life), G.colourlessGlyph(1), G.channelOnly),
                    new TargetRuleSet(
                        new AllCardsRule(
                            c => c != this && c.controller == this.controller && c.isColour(ManaColour.Life))),
                    new ModifyDoer(ModifiableStats.Power, LL.add(1), LL.endOfTurn),
                    new Foo(LL.exhaustThis, LL.manaCost(ManaColour.Life, ManaColour.Life, ManaColour.Colourless)),
                    0,
                    PileLocation.Field,
                    CastSpeed.Channel
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

                orderCost = 2;
                greyCost = 2;

                addTriggeredAbility(
                    "Whenever Kappa enters the battlefield under your control, draw two cards.",
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

                diesLambda(
                    "Whenever Yung Lich enters the graveyard from the battlefield under your control, draw a card.",
                    new Effect(new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController)),
                        new DrawCardsDoer(1))
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
                        new SelectCardRule(new PryPlayerRule(p => true,
                            new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController)),
                            PileLocation.Hand, c => false, SelectCardRule.Mode.Resolver),
                        new ModifyDoer(ModifiableStats.Movement, LL.add(0), LL.clearAura))); //ugliest hack i've seen in a while
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

                castEffect = zepLambda(2);
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
                        moveEvent.to.location.pile == PileLocation.Field),
                    0,
                    PileLocation.Field,
                    true,
                    TriggeredAbility.Timing.Post
                    );
            }
                break;

                #endregion
                #region Chieftain Slootboks

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
                    String.Format("{1}, {0}: Exhaust another target creature within 3 tiles.", G.exhaustGhyph, G.colouredGlyph(ManaColour.Nature)),
                    new TargetRuleSet(LL.creature(c => c != this)),
                    new FatigueDoer(true),
                    new Foo(LL.exhaustThis, LL.manaCost(ManaColour.Nature)),
                    3,
                    PileLocation.Field,
                    CastSpeed.Interrupt
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
                    String.Format("{1}{0}{0}, {2}: Draw a card. {3}", G.colouredGlyph(ManaColour.Order),
                        G.colourlessGlyph(2), G.exhaustGhyph, G.channelOnly),
                    new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController)),
                    new DrawCardsDoer(1),
                    fooFromManaCost(ManaColour.Order, ManaColour.Order, ManaColour.Colourless, ManaColour.Colourless),
                    0,
                    PileLocation.Field,
                    CastSpeed.Interrupt
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
                    new Effect(LL.nonheroicCreature(),
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
                rarity = Rarity.Uncommon;

                chaosCost = 1;
                lifeCost = 1;

                castRange = 4;
                castEffect = zepLambda(3);
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

                orderCost = 1;
                greyCost = 1;

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
                    new Effect
                    (new TargetRuleSet(
                        new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController),
                        new StaticManaRule(ManaColour.Nature, ManaColour.Nature, ManaColour.Nature)),
                        new GainBonusManaDoer());
                castDescription = String.Format("You gain {0}{0}{0} until the end of the step.",
                    G.colouredGlyph(ManaColour.Nature));
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
                    "Whenever Graverobber Syrdin creature enters the battlefield under your control, you may return a card from your graveyard to your hand.",
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
                baseMovement = 4;

                natureCost = 2;

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
                    new ModifyDoer(ModifiableStats.Toughness, LL.add(3), LL.never));
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
                natureCost = 1;
                greyCost = 1;

                castEffect =
                    new Effect(
                        new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard), new PryCardRule()),
                        new ZepperDoer(-2));
                additionalCastEffects.Add(new Effect(new CopyPreviousRule<Card>(1),
                    new ModifyDoer(ModifiableStats.Power, LL.add(2), LL.endOfTurn)));
                castDescription =
                    "Target creature is healed for 2 and gains 2 power until the end of this turn.";
            } break;

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

                chaosCost = 1;
                greyCost = 1;


                addTriggeredAbility(
                    "When Baby Dragon enters the battlefield you may have it deal 1 damage to target creature within 3 tiles.",
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
                #region Huntress of Nibemem

            case CardTemplate.Huntress_sOf_sNibemem:
            {
                cardType = CardType.Creature;
                race = Race.Human;
                subtype = Subtype.Warrior;
                rarity = Rarity.Common;

                natureCost = 1;
                greyCost = 2;

                baseToughness = 3;
                basePower = 1;
                baseMovement = 3;

                addActivatedAbility(
                    String.Format("{0}: Deal 1 damage to target creature within 3 tiles.", G.exhaustGhyph),
                    zepLambda(1),
                    new Foo(new Effect(new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard)),
                        new FatigueDoer(true))),
                    3,
                    PileLocation.Field,
                    CastSpeed.Interrupt
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
                castEffect = Effect.summonTokensEffect(CardTemplate.Squire, CardTemplate.Squire);

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
                        new SelectCardRule(new PryPlayerRule(),
                            PileLocation.Hand, c => c.cardType == CardType.Creature, SelectCardRule.Mode.Resolver),
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
                basePower = 1;
                baseToughness = 20;

                addActivatedAbility(
                    String.Format("{1}{1}, {0}: Each player discards a card. {2}", G.exhaustGhyph, G.colouredGlyph(ManaColour.Death), G.channelOnly),
                    new TargetRuleSet(new SelectCardRule(new PlayerResolveRule(PlayerResolveRule.Rule.AllPlayers), PileLocation.Hand, c => true, SelectCardRule.Mode.Reflective)),
                    new MoveToPileDoer(PileLocation.Graveyard),
                    new Foo(LL.exhaustThis, LL.manaCost(ManaColour.Death, ManaColour.Death)),
                    0,
                    PileLocation.Field,
                    CastSpeed.Channel
                    );
            } break;

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
                    v => v + controller.hand.Count,
                    ModifiableStats.Power, 
                    c => c == this, 
                    PileLocation.Field));
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
                    "Ilas Gravekeeper gets +1/+0 for each Zombie in its controllers graveyard.",
                    v => v + controller.graveyard.Count(c => c.race == Race.Zombie),
                    ModifiableStats.Power,
                    c => c == this,
                    PileLocation.Field
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
                    v => v + (controller.graveyard.Count >= 5 ? 2 : 0),
                    ModifiableStats.Power,
                    c => c == this,
                    PileLocation.Field
                    ));

                auras.Add(new Aura(
                    "This creature has gets +2/+2 as long as its controllers graveyard contains five or more cards.",
                    v => v + (controller.graveyard.Count >= 5 ? 2 : 0),
                    ModifiableStats.Toughness,
                    c => c == this,
                    PileLocation.Field
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
                    "You may cast Ilatian Haunter from the graveyard.",
                    new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                        new PryTileRule(t => t.card == null && !t.isEdgy,
                            new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), true)),
                    new SummonToTileDoer(),
                    new Foo(LL.manaCost(new ManaSet(ManaColour.Colourless, ManaColour.Death))),
                    2,
                    PileLocation.Graveyard,
                    CastSpeed.Channel,
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
                    new Effect(LL.nonheroicCreature(),
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
                    new Effect(LL.relic,
                        new MoveToPileDoer(PileLocation.Graveyard));
                castRange = 6;
            }
                break;

            #endregion
                #region Gotterdammerun

                case CardTemplate.Gotterdammerung:
                {
                    cardType = CardType.Channel;
                    rarity = Rarity.Uncommon;

                    lifeCost = 3;
                    greyCost = 3;

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
                        new TargetRuleSet(LL.nonheroicCreature()),
                        new MoveToPileDoer(PileLocation.Graveyard),
                        new Foo(),
                        new TypedGameEventFilter<MoveToPileEvent>(
                            e =>
                            {
                                return e.card == this && e.to.location.pile == PileLocation.Field;
                            }),
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
                        "Whenever Rider of Pestilence deals damage to a player that player discards a card.",
                        new TargetRuleSet(new SelectCardRule(new TriggeredTargetRule<DamageEvent, Player>(e => e.target.controller), PileLocation.Hand, c => true)),
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
                            "Whenever Rider of Famine deals damage to a player that player sacrifices a non-heroic creature.",
                            new TargetRuleSet(new SelectCardRule(new TriggeredTargetRule<DamageEvent, Player>(e => e.target.controller), PileLocation.Field, c => !c.isHeroic && c.cardType == CardType.Creature)),
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
                #region Chains of Virtue
                    case CardTemplate.Chains_sof_sVirtue:
                {
                    cardType = CardType.Channel;
                    rarity = Rarity.Common;

                    lifeCost = 1;
                    greyCost = 2;

                    castDescription = "Set target non-life creatures movement to 1.";
                    castEffect = new Effect(LL.nonColouredCreature(ManaColour.Life),
                        new ModifyDoer(ModifiableStats.Movement, LL.set(1), LL.never));
                    castRange = 4;
                } break;
                #endregion
                #region Chains of Sin
                case CardTemplate.Chains_sof_sSin:
                    {
                        cardType = CardType.Channel;
                        rarity = Rarity.Common;

                        deathCost = 1;
                        greyCost = 1;

                        castDescription = "Reduce target non-death creatures movement by 2. This cannot reduce the targets movement below 1.";
                        castEffect = new Effect(LL.nonColouredCreature(ManaColour.Death),
                            new ModifyDoer(ModifiableStats.Movement, v => Math.Max(Math.Min(v, 1), v - 2), LL.never));
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
                            new Effect(LL.relic,
                                new MoveToPileDoer(PileLocation.Graveyard));
                        castRange = 6;
                    } break;

                #endregion
                #region Deep Fry

                case CardTemplate.Deep_sFry:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Uncommon;

                    chaosCost = 1;
                    greyCost = 1;

                    castDescription = "Deal 1 damage to target creature then exhaust it.";
                    castEffect = zepLambda(1);
                    additionalCastEffects.Add(new Effect(new CopyPreviousRule<Card>(1), new FatigueDoer(true)));
                    castRange = 5;

                } break;
                #endregion
                #region Reanimate Dead
                case CardTemplate.Raise_sDead:
                {
                    cardType = CardType.Channel;
                    rarity = Rarity.Uncommon;

                    deathCost = 2;
                    greyCost = 2;
                    castRange = 2;

                    castDescription = "Return a creature from your graveyard to the battlefield under your control.";
                    castEffect = new Effect(new TargetRuleSet(
                        new SelectCardRule(PileLocation.Graveyard, c => c.cardType == CardType.Creature),
                        new PryTileRule(t => t.card == null && !t.isEdgy,
                            new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), true)),
                        new SummonToTileDoer());
                } break;
                #endregion
                #region Ancient Druid
                case CardTemplate.Ancient_sDruid:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Common;

                    natureCost = 1;

                    baseToughness = 1;
                    baseMovement = 1;
                    basePower = 1;

                    addActivatedAbility(
                        String.Format("{0}: Gain {1} until end of step.", G.exhaustGhyph, G.colouredGlyph(ManaColour.Nature)),
                        new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), new StaticManaRule(ManaColour.Nature)),
                        new GainBonusManaDoer(),
                        new Foo(LL.exhaustThis),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );
                }break;
                #endregion
                #region Suspicious Vortex

                case CardTemplate.Suspicious_sVortex:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Uncommon;

                    orderCost = 4;
                    
                    castRange = 5;
                    castDescription = "Place target non-heroic permanent on top of its owners deck.";
                    castEffect = new Effect(new PryCardRule(c => !c.isHeroic), new MoveToPileDoer(PileLocation.Deck));
                } break;

                #endregion
                #region Rapture
                case CardTemplate.Rapture:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Uncommon;

                    lifeCost = 2;
                    greyCost = 2;

                    castDescription = "Displace target exhausted non-heroic creature.";
                    castEffect =
                        new Effect(
                            new PryCardRule(c => c.cardType == CardType.Creature && !c.isHeroic && c.isExhausted),
                            new MoveToPileDoer(PileLocation.Displaced));
                    castRange = 4;
                } break;
                #endregion
                #region Seething Rage
                case CardTemplate.Seething_sRage:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Uncommon;

                    chaosCost = 2;

                    castDescription = "Return all movement to target creature and give it +2/+0 until end of turn.";
                    castEffect =
                        new Effect(
                            new TargetRuleSet(LL.creature()),
                            new FatigueDoer(false));
                    additionalCastEffects.Add(new Effect(new CopyPreviousRule<Card>(0),
                        new ModifyDoer(ModifiableStats.Power, LL.add(2), LL.endOfTurn)));
                    castRange = 3;
                } break;
                #endregion
                #region Ilas Bargain
                case CardTemplate.Ilas_sBargain:
                {
                    cardType = CardType.Channel;
                    rarity = Rarity.Uncommon;

                    deathCost = 2;

                    castDescription =
                        "As an additional cost to casting this card sacrifice a non-heroic creature. Draw two cards.";
                    castEffect = new Effect(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController),
                        new DrawCardsDoer(2));
                    additionalCastCosts.Add(sacCostLambda);
                } break;
                #endregion
                #region Marilith
                case CardTemplate.Marilith:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Rare;

                    baseMovement = 4;
                    baseToughness = 3;
                    basePower = 3;

                    mightCost = 2;
                    greyCost = 1;

                    keywordAbilities.Add(KeywordAbility.Ambush);
                } break;
                #endregion
                #region Sanguine Artisan
                case CardTemplate.Sanguine_sArtisan:
                    {
                        cardType = CardType.Creature;
                        race = Race.Vampire;
                        rarity = Rarity.Common;

                        basePower = 0;
                        baseToughness = 2;
                        baseMovement = 2;

                        deathCost = 1;
                        greyCost = 1;


                        addTriggeredAbility(
                            "Whenever a creature enters the graveyard from the battlefield under your control, Sanguine Artisan deals 1 damage to target heroic creature.",
                            new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                                LL.player),
                            new ZepperDoer(1),
                            new Foo(),
                            new TypedGameEventFilter<MoveToPileEvent>(moveEvent =>
                                moveEvent.card.controller == controller &&
                                moveEvent.to.location.pile == PileLocation.Graveyard &&
                                moveEvent.card.location.pile == PileLocation.Field),
                            -1,
                            PileLocation.Field,
                            false
                            );
                    } break;
                #endregion
                #region Houndmaster
                case CardTemplate.Houndmaster:
                {
                    cardType = CardType.Creature;
                    race = Race.Human;
                    rarity = Rarity.Common;

                    natureCost = 2;
                    greyCost = 2;

                    basePower = 3;
                    baseToughness = 5;
                    baseMovement = 3;

                    addTriggeredAbility(
                        "Whenever Houndmaster deals damage you may summon a 2/2 Wolf token.",
                        Effect.summonTokensEffect(CardTemplate.Wolf),
                        new Foo(),
                        new TypedGameEventFilter<DamageEvent>(damageEvent => damageEvent.source == this),
                        2,
                        PileLocation.Field,
                        true,
                        TriggeredAbility.Timing.Post
                        );
                } break;
                #endregion
                #region Shotty Contruct
                    
                case CardTemplate.Shotty_sContruct:
                {
                    cardType = CardType.Creature;
                    race = Race.Mecha;
                    subtype = Subtype.Warrior;
                    rarity = Rarity.Common;

                    greyCost = 0;

                    basePower = 1;
                    baseToughness = 1;
                    baseMovement = 2;
                } break;

                #endregion
                #region Feral Imp
                case CardTemplate.Feral_sImp:
                {
                    cardType = CardType.Creature;
                    race = Race.Demon;
                    rarity = Rarity.Common;

                    mightCost = 1;
                    greyCost = 1;

                    basePower = 2;
                    baseToughness = 1;
                    baseMovement = 3;

                    keywordAbilities.Add(KeywordAbility.Fervor);
                    keywordAbilities.Add(KeywordAbility.Flying);
                } break;
                #endregion
                #region Resounding Blast
                case CardTemplate.Resounding_sBlast:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Uncommon;

                    castRange = 3;
                    chaosCost = 2;
                    greyCost = 1;

                    castEffect = zepNonHeroicLambda(3);
                    additionalCastEffects.Add(new Effect(
                        new TargetRuleSet(
                            new CopyPreviousRule<Card>(0),
                            new ModifyPreviousRule<Card, Card>(1, c => c.controller.heroCard)),
                        new ZepperDoer(3)
                        ));
                    castDescription = "Deal 3 damage to target non-heroic creature and 3 damage to that creatures controller.";

                } break;
                #endregion
                #region Solemn Lotus
                case CardTemplate.Solemn_sLotus:
                {
                    cardType = CardType.Relic;
                    rarity = Rarity.Uncommon;
                    forceColour = ManaColour.Death;

                    greyCost = 1;

                    baseMovement = 1;

                    addActivatedAbility(
                        String.Format("{0}, {1}: Gain one Death mana until end of step.",
                            G.colourlessGlyph(1), G.exhaustGhyph),
                        new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), new StaticManaRule(ManaColour.Death)),
                        new GainBonusManaDoer(),
                        new Foo(LL.exhaustThis, LL.manaCost(ManaColour.Colourless)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );
                        
                    addActivatedAbility(
                        String.Format("{0}{0}, {1}, Sacrifice Solemn Lotus: Target player sacrifices a non-heroic creature.", G.colouredGlyph(ManaColour.Death), G.exhaustGhyph),
                        playerSacLambda(new PryPlayerRule()),
                        new Foo(LL.exhaustThis, LL.manaCost(ManaColour.Death, ManaColour.Death), sacThisLambda),
                        -1,
                        PileLocation.Field, 
                        CastSpeed.Channel
                        );

                } break;
                #endregion
                #region Mysterious Lilac
                case CardTemplate.Mysterious_sLilac:
                {
                    cardType = CardType.Relic;
                    rarity = Rarity.Uncommon;
                    forceColour = ManaColour.Order;

                    greyCost = 1;

                    baseMovement = 1;

                    addActivatedAbility(
                        String.Format("{0}, {1}: Gain one Order mana until end of step.",
                            G.colourlessGlyph(1), G.exhaustGhyph),
                        new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController),
                            new StaticManaRule(ManaColour.Order)),
                        new GainBonusManaDoer(),
                        new Foo(LL.exhaustThis, LL.manaCost(ManaColour.Colourless)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );

                        addActivatedAbility(
                        String.Format("{0}{0}, {1}, Sacrifice Mysterious Lilac: Draw a card.", G.colouredGlyph(ManaColour.Order), G.exhaustGhyph),
                        new Effect(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), new DrawCardsDoer(1)),
                        new Foo(LL.exhaustThis, LL.manaCost(ManaColour.Order, ManaColour.Order), sacThisLambda),
                        -1,
                        PileLocation.Field,
                        CastSpeed.Channel
                        );

                } break;
                #endregion
                #region Daring Poppy
                case CardTemplate.Daring_sPoppy:
                    {
                        cardType = CardType.Relic;
                        rarity = Rarity.Uncommon;
                        forceColour = ManaColour.Chaos;

                        greyCost = 1;

                        baseMovement = 1;

                        addActivatedAbility(
                        String.Format("{0}, {1}: Gain one Chaos mana until end of step.",
                            G.colourlessGlyph(1), G.exhaustGhyph),
                        new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), new StaticManaRule(ManaColour.Chaos)),
                        new GainBonusManaDoer(),
                        new Foo(LL.exhaustThis, LL.manaCost(ManaColour.Colourless)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );

                        addActivatedAbility(
                            String.Format("{0}{0}, {1}, Sacrifice Daring Poppy: Deal 2 damage to target creature.", G.colouredGlyph(ManaColour.Chaos), G.exhaustGhyph),
                            zepLambda(2),
                            new Foo(LL.exhaustThis, LL.manaCost(ManaColour.Chaos, ManaColour.Chaos), sacThisLambda),
                            -1,
                            PileLocation.Field,
                            CastSpeed.Channel
                            );

                    }
                    break;
                #endregion
                #region Serene Dandelion
                case CardTemplate.Serene_sDandelion:
                    {
                        cardType = CardType.Relic;
                        rarity = Rarity.Uncommon;
                        forceColour = ManaColour.Life;

                        greyCost = 1;

                        baseMovement = 1;

                        addActivatedAbility(
                        String.Format("{0}, {1}: Gain one Life mana until end of step.",
                            G.colourlessGlyph(1), G.exhaustGhyph),
                        new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), new StaticManaRule(ManaColour.Life)),
                        new GainBonusManaDoer(),
                        new Foo(LL.exhaustThis, LL.manaCost(ManaColour.Colourless)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );

                        addActivatedAbility(
                            String.Format("{0}{0}, {1}, Sacrifice Serene Dandelion: Restore 4 toughness to target creature.", G.colouredGlyph(ManaColour.Life), G.exhaustGhyph),
                            zepLambda(-4),
                            new Foo(LL.exhaustThis, LL.manaCost(ManaColour.Life, ManaColour.Life), sacThisLambda),
                            -1,
                            PileLocation.Field,
                            CastSpeed.Channel
                            );

                    }
                    break;
                #endregion
                #region Stark Lily
                case CardTemplate.Stark_sLily:
                {
                    cardType = CardType.Relic;
                        rarity = Rarity.Uncommon;
                        forceColour = ManaColour.Might;

                    greyCost = 1;

                    baseMovement = 1;

                    addActivatedAbility(
                        String.Format("{0}, {1}: Gain one Might mana until end of step.",
                            G.colourlessGlyph(1), G.exhaustGhyph),
                        new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController),
                            new StaticManaRule(ManaColour.Might)),
                        new GainBonusManaDoer(),
                        new Foo(LL.exhaustThis, LL.manaCost(ManaColour.Colourless)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );

                        addActivatedAbility(
                        String.Format("{0}{0}, {1}, Sacrifice Stark Lily: Summon a 2/2 Gryphon token with Flying.", G.colouredGlyph(ManaColour.Might), G.exhaustGhyph),
                        Effect.summonTokensEffect(CardTemplate.Gryphon),
                        new Foo(LL.exhaustThis, LL.manaCost(ManaColour.Might, ManaColour.Might), sacThisLambda),
                        2,
                        PileLocation.Field,
                        CastSpeed.Channel
                        );

                } break;
                #endregion
                #region Vibrant Zinnia
                case CardTemplate.Vibrant_sZinnia:
                    {
                        cardType = CardType.Relic;
                        rarity = Rarity.Uncommon;
                        forceColour = ManaColour.Nature;

                        greyCost = 1;

                        baseMovement = 1;

                        addActivatedAbility(
                        String.Format("{0}, {1}: Gain one Nature mana until end of step.",
                            G.colourlessGlyph(1), G.exhaustGhyph),
                        new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), new StaticManaRule(ManaColour.Nature)),
                        new GainBonusManaDoer(),
                        new Foo(LL.exhaustThis, LL.manaCost(ManaColour.Colourless)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );


                        Effect e1 = new Effect(LL.nonheroicCreature(),
                            new ModifyDoer(ModifiableStats.Power, LL.add(2), LL.never));
                        Effect e2 = new Effect(new CopyPreviousRule<Card>(0),
                            new ModifyDoer(ModifiableStats.Toughness, LL.add(2), LL.never));

                        addActivatedAbility(
                            String.Format("{0}{0}, {1}, Sacrifice Vibrant Zinnia: Target non-heroic creature gets +2/+2", G.colouredGlyph(ManaColour.Nature), G.exhaustGhyph),
                            new Effect[] {e1, e2}, 
                            new Foo(LL.exhaustThis, LL.manaCost(ManaColour.Nature, ManaColour.Nature), sacThisLambda),
                            -1,
                            PileLocation.Field,
                            CastSpeed.Channel
                            );

                    }
                    break;
                #endregion
                #region Primal Chopter
                case CardTemplate.Primal_sChopter:
                {
                    cardType = CardType.Creature;
                    race = Race.Mecha;
                    rarity = Rarity.Common;

                    greyCost = 2;

                    basePower = 1;
                    baseToughness = 2;
                    baseMovement = 4;

                    keywordAbilities.Add(KeywordAbility.Flying);
                } break;
                #endregion
                #region Famished Tarantula
                case CardTemplate.Famished_sTarantula:
                {
                    cardType = CardType.Creature;
                    race = Race.Beast;
                    rarity = Rarity.Uncommon;

                    basePower = 1;
                    baseToughness = 3;
                    baseMovement = 3;

                    natureCost = 2;
                    greyCost = 1;

                    deathtouchLambda();

                    keywordAbilities.Add(KeywordAbility.Wingclipper);

                } break;
                #endregion
                #region Morenian Medic
                case CardTemplate.Morenian_sMedic:
                {
                    cardType = CardType.Creature;
                    race = Race.Human;
                    subtype = Subtype.Warrior;
                    rarity = Rarity.Uncommon;

                    basePower = 2;
                    baseToughness = 2;
                    baseMovement = 2;

                    lifeCost = 2;

                    etbLambda(
                        "When Morenian Medic enters the battlefield restore 2 toughness to target creature within 3 tiles.",
                        zepLambda(-2), 3);

                } break;
                #endregion
                #region Bubastis
                case CardTemplate.Bubastis:
                {
                    cardType = CardType.Creature;
                    race = Race.Beast;
                    rarity = Rarity.Legendary;

                    basePower = 4;
                    baseToughness = 5;
                    baseMovement = 3;

                    orderCost = 3;
                    greyCost = 3;

                    etbLambda(
                        "When Bubastis enters the battlefield you may return another target non-heroic creature within 5 tiles to it's owners hand.",
                        new Effect(LL.nonheroicCreature(c => c != this), new MoveToPileDoer(PileLocation.Hand)),
                        5,
                        true
                        );
                } break;
                #endregion
                #region Unyeilding Stalwart
                case CardTemplate.Unyeilding_sStalwart:
                {
                    cardType = CardType.Creature;
                    race = Race.Human;
                    subtype = Subtype.Warrior;
                    rarity = Rarity.Common;

                    lifeCost = 1;

                    basePower = 1;
                    baseToughness = 1;
                    baseMovement = 3;

                    diesLambda(
                        "Whenever Unyeilding Stalwart enters the graveyard from the battlefield under your control, summon a 1/1 Spirit token with Flying.",
                        Effect.summonTokensEffect(CardTemplate.Spirit),
                        2);
                } break;
                #endregion
                #region Haunted Chapel
                case CardTemplate.Haunted_sChapel:
                {
                    cardType = CardType.Relic;
                    rarity = Rarity.Uncommon;

                    lifeCost = 1;
                    deathCost = 1;
                    greyCost = 2;

                    addActivatedAbility(
                        String.Format(
                            "{0}{1}, Displace a Creature card from your Graveyard: Summon a 1/1 Spirit token with Flying. {2}",
                            G.colouredGlyph(ManaColour.Life), G.colouredGlyph(ManaColour.Death), G.channelOnly),
                        Effect.summonTokensEffect(CardTemplate.Spirit),
                        new Foo(
                            LL.manaCost(ManaColour.Death, ManaColour.Life),
                            displaceFromGraveyard(c => c.cardType == CardType.Creature)),
                        2,
                        PileLocation.Field,
                        CastSpeed.Channel);
                } break;
                #endregion
                #region Enraged Dragon
                case CardTemplate.Enraged_sDragon:
                {
                    cardType = CardType.Creature;
                    race = Race.Dragon;
                    rarity = Rarity.Common;

                    basePower = 1;
                    baseToughness = 4;
                    baseMovement = 4;

                    chaosCost = 1;
                    greyCost = 2;

                    addActivatedAbility(
                        String.Format("{0}: Enraged Dragon gets +1/+0 until end of turn.", G.colouredGlyph(ManaColour.Chaos)),
                        new Effect(new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                            new ModifyDoer(ModifiableStats.Power, LL.add(1), LL.endOfTurn)),
                        new Foo(LL.manaCost(ManaColour.Chaos)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );

                    keywordAbilities.Add(KeywordAbility.Flying);
                } break;
                #endregion
                #region Chromatic Unicorn
                case CardTemplate.Chromatic_sUnicorn:
                {
                    cardType = CardType.Creature;
                    race = Race.Beast;
                    rarity = Rarity.Uncommon;

                    natureCost = 2;

                    basePower = 1;
                    baseToughness = 2;
                    baseMovement = 3;

                    auras.Add(new Aura(
                        "",
                        v => v + (owner.game.allPlayers.SelectMany(p => p.graveyard.cards).SelectMany(c => c.colours).Where(c => c != ManaColour.Colourless).Distinct().Count()),
                        ModifiableStats.Power,
                        c => c == this,
                        PileLocation.Field
                        ));

                    auras.Add(new Aura(
                        "Chromatic Unicorn gets +1/+1 for each colour among cards in Graveyards.",
                        v => v + (owner.game.allPlayers.SelectMany(p => p.graveyard.cards).SelectMany(c => c.colours).Where(c => c != ManaColour.Colourless).Distinct().Count()),
                        ModifiableStats.Toughness,
                        c => c == this,
                        PileLocation.Field
                        ));

                } break;
                #endregion
                #region Seraph
                case CardTemplate.Seraph:
                {
                    cardType = CardType.Creature;
                    race = Race.Angel;
                    subtype = Subtype.Guardian;
                    rarity = Rarity.Common;

                    baseToughness = 2;
                    basePower = 1;
                    baseMovement = 4;

                    lifeCost = 3;
                    greyCost = 2;

                    Func<Func<int, int>> grtr = () =>
                    {
                        int i = controller.field.cards.Where(c => c.cardType == CardType.Creature && !c.isHeroic).Count();
                        return v => v + i;
                    };

                    Effect e1 = new Effect(
                        new CardResolveRule(CardResolveRule.Rule.ResolveCard), new ForceStaticModifyDoer(ModifiableStats.Power, 
                        grtr, 
                        LL.never)
                        );

                    Effect e2 = new Effect(
                        new CardResolveRule(CardResolveRule.Rule.ResolveCard), new ForceStaticModifyDoer(ModifiableStats.Toughness,
                        grtr,
                        LL.never)
                        );

                    etbLambda("When Seraph enters the battlefield it gets +1/+1 for every non-heroic you control.",
                        new Effect[] {e1, e2});

                    keywordAbilities.Add(KeywordAbility.Flying);

                } break;
                #endregion
                #region Moratian Battle Standard
                case CardTemplate.Moratian_sBattle_sStandard:
                {
                    cardType = CardType.Relic;
                    rarity = Rarity.Uncommon;

                    lifeCost = 1;
                    greyCost = 2;

                    auras.Add(new Aura("",
                        LL.add(1),
                        ModifiableStats.Toughness,
                        c => c.controller == this.controller && !c.isHeroic && c.isColour(ManaColour.Life),
                        PileLocation.Field
                        ));

                    auras.Add(new Aura("Your non-heroic Life creatures get +1/+1.",
                        LL.add(1),
                        ModifiableStats.Power,
                        c => c.controller == this.controller && !c.isHeroic && c.isColour(ManaColour.Life),
                        PileLocation.Field
                        ));
                } break;
                #endregion
                #region Flamekindler
                case CardTemplate.Flamekindler:
                {
                    cardType = CardType.Creature;
                    race = Race.Human;
                    subtype = Subtype.Wizard;
                    rarity = Rarity.Rare;

                    chaosCost = 2;

                    basePower = 0;
                    baseToughness = 3;
                    baseMovement = 2;

                    addActivatedAbility(
                        String.Format(
                            "{0}, {1}, Displace a Channel or Interrupt card from your Graveyard: Deal 2 damage to target Creature within 5 tiles.",
                            G.colouredGlyph(ManaColour.Chaos), G.exhaustGhyph),
                        zepLambda(2),
                        new Foo(
                            LL.manaCost(ManaColour.Chaos),
                            displaceFromGraveyard(c => c.cardType == CardType.Channel || c.cardType == CardType.Interrupt),
                            LL.exhaustThis),
                        5,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );
                } break;
                #endregion
                #region Sparryz
                case CardTemplate.Sparryz:
                {
                    cardType = CardType.Creature;
                    race = Race.Demon;
                    subtype = Subtype.Wizard;
                    rarity = Rarity.Legendary;
                    isHeroic = true;
                    forceColour = ManaColour.Chaos;

                    baseMovement = 2;
                    basePower = 1;
                    baseToughness = 20;

                    addActivatedAbility(
                        String.Format("{1}{1}, {0}: Deal 2 damage to all heroic creatures.",
                            G.exhaustGhyph, G.colouredGlyph(ManaColour.Chaos)),
                        new TargetRuleSet(
                            new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                            new CardResolveRule(CardResolveRule.Rule.AllHeroes)),
                        new ZepperDoer(2),
                        new Foo(LL.exhaustThis, LL.manaCost(ManaColour.Chaos, ManaColour.Chaos)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );
                } break;
                #endregion
                case CardTemplate.missingo:
                {
                    cardType = CardType.Interrupt;

                    castEffect = new Effect(
                        new TargetRuleSet(
                            new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                            new AllCardsRule(c => c.cardType == CardType.Creature && !c.isHeroic)), 
                        new ZepperDoer(1));

                    additionalCastEffects.Add(new Effect(
                        new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController),
                        new DrawCardsDoer(1)
                        ));
                } break;

                #region tokens
                #region Spirit
                case CardTemplate.Spirit:
                {
                    forceColour = ManaColour.Life;
                    basePower = 1;
                    baseToughness = 1;
                    baseMovement = 2;
                    race = Race.Undead;
                    isToken = true;
                    keywordAbilities.Add(KeywordAbility.Flying);
                } break;
                #endregion
                #region Gryphon
                case CardTemplate.Gryphon:
                {
                    forceColour = ManaColour.Might;
                    basePower = 2;
                    baseToughness = 2;
                    baseMovement = 3;
                    race = Race.Beast;
                    isToken = true;
                    keywordAbilities.Add(KeywordAbility.Flying);
                } break;
                #endregion
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
                #region Wolf
                case CardTemplate.Wolf:
                    {
                        forceColour = ManaColour.Nature;
                        basePower = 2;
                        baseToughness = 2;
                        baseMovement = 3;
                        race = Race.Beast;
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
                castSpeed = CastSpeed.Interrupt;
            }
            else if (cardType == CardType.Channel)
            {
                castSpeed = CastSpeed.Channel;
            }
            else if (cardType == CardType.Creature || cardType == CardType.Relic)
            {
                castSpeed = CastSpeed.Channel;
                castRange = 2;
                castEffect = new Effect(new TargetRuleSet(
                    new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                    new PryTileRule(t => t.card == null && !t.isEdgy, new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), true)),
                    new SummonToTileDoer());
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
            addTriggeredAbility(description, e, cost, filter, castRange, activeIn, optional, timing);
        }

        private void addTriggeredAbility(string description, Effect e, Foo cost, GameEventFilter filter, int castRange, PileLocation activeIn, bool optional, TriggeredAbility.Timing timing = TriggeredAbility.Timing.Pre)
        {
            TriggeredAbility ta = new TriggeredAbility(this, activeIn, new[] { e }, castRange, cost, filter, optional, timing, description);
            abilities.Add(ta);
        }

        private void addTriggeredAbility(string description, Effect[] es, Foo cost, GameEventFilter filter, int castRange, PileLocation activeIn, bool optional, TriggeredAbility.Timing timing = TriggeredAbility.Timing.Pre)
        {
            TriggeredAbility ta = new TriggeredAbility(this, activeIn, es, castRange, cost, filter, optional, timing, description);
            abilities.Add(ta);
        }

        private void addActivatedAbility(string description, TargetRuleSet trs, Doer doer, Foo cost, int castRange, PileLocation activeIn, CastSpeed castSpeed, bool alternateCast = false)
        {
            Effect e = new Effect(trs, doer);
            addActivatedAbility(description, e, cost, castRange, activeIn, castSpeed, alternateCast);
        }

        private void addActivatedAbility(string description, Effect e, Foo cost, int castRange, PileLocation activeIn, CastSpeed castSpeed, bool alternateCast = false)
        {
            ActivatedAbility aa = new ActivatedAbility(this, activeIn, castRange, cost, castSpeed, description, e);
            abilities.Add(aa);
            if (alternateCast) alternateCasts.Add(aa);
        }

        private void addActivatedAbility(string description, Effect[] es, Foo cost, int castRange, PileLocation activeIn, CastSpeed castSpeed, bool alternateCast = false)
        {
            ActivatedAbility aa = new ActivatedAbility(this, activeIn, castRange, cost, castSpeed, description, es);
            abilities.Add(aa);
            if (alternateCast) alternateCasts.Add(aa);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] //yagni said no one ever
        private static Foo fooFromManaCost(params ManaColour[] cs)
        {
            return new Foo(new Effect(new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController),
                new ManaCostRule(cs)), new PayManaDoer()));
        }

        public Effect zepLambda(int damage)
        {
            return
                new Effect(new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard), LL.creature()),
                    new ZepperDoer(damage));
        }

        public Effect zepNonHeroicLambda(int damage)
        {
            return
                new Effect(new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard), LL.nonheroicCreature()),
                    new ZepperDoer(damage));
        }

        public void etbLambda(String description, Effect e, int range = -1, bool optional = false)
        {
            addTriggeredAbility(
                        description, 
                        e,
                        new Foo(),
                        new TypedGameEventFilter<MoveToPileEvent>(
                            moveEvent => moveEvent.card == this && location.pile == PileLocation.Field),
                        range,
                        PileLocation.Field,
                        optional,
                        TriggeredAbility.Timing.Post
                        );
        }

        public void etbLambda(String description, Effect[] es, int range = -1, bool optional = false)
        {
            addTriggeredAbility(
                        description,
                        es,
                        new Foo(),
                        new TypedGameEventFilter<MoveToPileEvent>(
                            moveEvent => moveEvent.card == this && location.pile == PileLocation.Field),
                        range,
                        PileLocation.Field,
                        optional,
                        TriggeredAbility.Timing.Post
                        );
        }

        public void diesLambda(String description, Effect e, int range = -1, bool optional = false)
        {
            addTriggeredAbility(
                description,
                e,
                new Foo(),
                new TypedGameEventFilter<MoveToPileEvent>(
                    moveEvent =>
                        moveEvent.card == this && moveEvent.to.location.pile == PileLocation.Graveyard &&
                        location.pile == PileLocation.Field),
                range,
                PileLocation.Field,
                optional
                );
        }

        public void deathtouchLambda()
        {
            addTriggeredAbility(
                        "Whenever this creature deals damage to a non-heroic creature destroy it.",
                        new TargetRuleSet(new TriggeredTargetRule<DamageEvent, Card>(de => de.target)),
                        new MoveToPileDoer(PileLocation.Graveyard),
                        new Foo(),
                        new TypedGameEventFilter<DamageEvent>(de => de.source == this && !de.target.isHeroic),
                        0,
                        PileLocation.Field,
                        false
                        );
        }

        public Effect sacThisLambda =>
            new Effect(new CardResolveRule(CardResolveRule.Rule.ResolveCard), new MoveToPileDoer(PileLocation.Graveyard));

        public Effect sacCostLambda
            =>
                new Effect(
                    new PryCardRule(
                        c => !c.isHeroic && c.controller == this.controller && c.cardType == CardType.Creature),
                    new MoveToPileDoer(PileLocation.Graveyard));

        public static Effect displaceFromGraveyard(Func<Card, bool> filter = null)
        {
            filter = filter ?? (c => true);
            return new Effect(new SelectCardRule(PileLocation.Graveyard, filter), new MoveToPileDoer(PileLocation.Displaced));
        }

        public Effect playerSacLambda(TargetRule sacrificer)
        {
            return new Effect(new SelectCardRule(sacrificer, PileLocation.Field, c => c.cardType == CardType.Creature && !c.isHeroic, SelectCardRule.Mode.Reflective),
                new MoveToPileDoer(PileLocation.Graveyard));
        }
        
    }
}
