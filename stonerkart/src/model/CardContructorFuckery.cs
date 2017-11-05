﻿#define testx
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
            flavourText = "";
            CastSpeed castSpeed;
            List<Effect> additionalCastCosts = new List<Effect>();
            keywordAbilities = new List<KeywordAbility>();


            #region oophell

            switch (ct)
            {
                #region Confuse
                case CardTemplate.Confuse:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Common;

                    orderCost = 1;

                    castRange = 4;
                    castDescription = "Apply 1 stack of stunned to target creature.";
                    castEffect = new ApplyStacksEffect(new ChooseHexCard(c => c.IsCreature), sg(Counter.Stunned));
                } break;
                #endregion
                #region Big Monkey
                case CardTemplate.Big_sMonkey:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Common;

                    deathCost = 3;
                    greyCost = 1;

                    basePower = 3;
                    baseToughness = 2;
                    baseMovement = 3;

                    keywordAbilities.Add(KeywordAbility.Flying);

                    AddActivatedAbility(
                        String.Format("{0}{0}, {1}: Give another target creature +1/+1.", G.colouredGlyph(ManaColour.Life), G.exhaustGhyph),
                        new Foo(new ModifyEffect(Add(1), Forever, new ChooseHexCard(c => c.IsCreature && !c.IsHeroic), sg(ModifiableStats.Power, ModifiableStats.Toughness))),
                        new Foo(ResolverPaysManaEffect(ManaColour.Life, ManaColour.Life)),
                        3,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );

                    flavourText = "Big monkey -- Big money.";

                } break;
                #endregion
                #region Great White Buffalo
                case CardTemplate.Great_sWhite_sBuffalo:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Rare;

                    lifeCost = 2;
                    natureCost = 2;
                    greyCost = 2;

                    basePower = 6;
                    baseToughness = 7;
                    baseMovement = 3;

                    AddEtBLambda(
                        "When Great White Buffalo enters the battlefield restore 5 toughness to your heroic creatures.",
                        new Foo(new PingEffect(-5, ResolveCard, ResolveControllerCard)));

                    flavourText = "They only took what they needed, millions of buffalo were the proof.";
                } break;
                #endregion
                #region Alter Fate
                case CardTemplate.Alter_sFate:
                {
                    cardType = CardType.Channel;
                    rarity = Rarity.Common;

                    orderCost = 1;

                    castDescription =
                        "Look at the top three cards of your deck then put them back in any order. You may shuffle your library. Draw a card.";
                    castEffect = new MoveToPileEffect(
                        PileLocation.Deck,
                        new ChooseCardsFromCards(
                            c => true,
                            ResolveController,
                            3,
                            true,
                            ChooseCardsFromCards.Mode.PlayerLooksAtPlayer,
                            p => p.deck.Reverse().Take(3)));
                    additionalCastEffects.Add(new ShuffleEffect(new OptionRule<Player>("Do you wish to shuffle your deck?", ResolveController)));
                    additionalCastEffects.Add(new DrawCardsEffect(1, ResolveController));

                } break;
                #endregion
                #region Count Fera II
                case CardTemplate.Count_sFera_sII:
                    {
                        cardType = CardType.Creature;
                        rarity = Rarity.Legendary;
                        baseRace = Race.Undead;

                        baseMovement = 3;
                        basePower = 3;
                        baseToughness = 3;

                        deathCost = 4;
                        greyCost = 1;
                        /*
                        Effect e1 = new Effect(new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard)),
                           new ModifyEffect(add(1), Forever, ModifiableStats.Power, ModifiableStats.Toughness));
                        Effect e2 = new Effect(new ChooseRule<Card>(ChooseRule<Card>.ChooseAt.Cast, c => !c.isHeroic),
                            new ModifyEffect(add(-1), Forever, ModifiableStats.Power, ModifiableStats.Toughness));

                        //should be better considering its legendary, maybe create vampire instead of sacrificing? 
                        AddActivatedAbility(String.Format("{0}, Exhaust: Target non-heroic creature becomes undead and gets -1/-1. Count Fera II gets +1/+1.", G.colouredGlyph(ManaColour.Death)),
                            new Effect[] {e1, e2,},
                            new Foo(exhaustThis, ResolverPaysManaEffect(ManaColour.Death)),//new Foo(new Effect(new TargetRuleSet(new ChooseRule<Card>(c => !c.isHeroic)), new MoveToPileDoer(PileLocation.Graveyard))),
                            -1,
                            PileLocation.Field,
                            CastSpeed.Interrupt);

                        Effect e3 = new Effect(new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard)),
                           new ModifyEffect(add(-3), Forever, ModifiableStats.Power, ModifiableStats.Toughness));
                        Effect e4 = new Effect(new ChooseRule<Card>(ChooseRule<Card>.ChooseAt.Cast, c => !c.isHeroic),
                            new ModifyEffect(add(-3), Forever, ModifiableStats.Power, ModifiableStats.Toughness));

                        AddActivatedAbility(
                            String.Format(
                                "{0}{0}, Exhaust: Target non-undead, non-heroic creature gets -3/-3. Count Fera II gets -3/-3.",
                                G.colouredGlyph(ManaColour.Death)),
                            new Effect[] {e3, e4,},
                            new Foo(ExhaustThis, ResolverPaysManaEffect(ManaColour.Death, ManaColour.Death)),//manaCostFoo(ManaColour.Chaos), 
                            -1,
                            PileLocation.Field,
                            CastSpeed.Interrupt);
                            */
                    }
                    break;
                #endregion
                #region Arachosa
                case CardTemplate.Arachosa:
                    {
                        cardType = CardType.Creature;
                        rarity = Rarity.Legendary;
                        baseRace = Race.Beast;
                        
                        natureCost = 4;

                        basePower = 4;
                        baseToughness = 2;
                        baseMovement = 2;

                        AddDiesLambda(
                        "When Arachosa enters the graveyard from the battlefield summon two green 1/1/3 Spiderling tokens.",
                        new Foo(SummonTokensEffect(ResolveController, CardTemplate.Spiderling, CardTemplate.Spiderling)));
                    }
                    break;

                #endregion
                #region Paralyzing_sSpider
                case CardTemplate.Paralyzing_sSpider:
                    {
                        cardType = CardType.Creature;
                        rarity = Rarity.Common;
                        baseRace = Race.Beast;

                        natureCost = 3;
                        greyCost = 2;

                        basePower = 1;
                        baseToughness = 5;
                        baseMovement = 3;

                        AddTriggeredAbility(
                            "Whenever Paralyzing Spider deals damage to a non-heroic creature reduce that creature's movement speed by 2. This cannot reduce the creature's movement below 1.",
                            new Foo(new ApplyStacksEffect(new TriggeredRule<DamageEvent, Card>(g => g.target), sg(Counter.Crippled))), 
                            new Foo(),
                            new TypedGameEventFilter<DamageEvent>(damageEvent => damageEvent.source == this),
                            -1,
                            PileLocation.Field,
                            false,
                            TriggeredAbility.Timing.Post
                        );

                    }
                    break;
                #endregion
                #region Seblastian
                case CardTemplate.Seblastian:
                    {
                        cardType = CardType.Creature;
                        rarity = Rarity.Rare;
                        baseRace = Race.Human;

                        baseMovement = 3;
                        basePower = 0;
                        baseToughness = 3;

                        mightCost = 1;

                        AddActivatedAbility(
                        String.Format("Sacrifice this creature: deal 3 damage to another target creature within 1 tile."),
                        new Foo(new PingEffect(3, ResolveCard, new ChooseHexCard(c => c.IsCreature))),
                        new Foo(new MoveToPileEffect(PileLocation.Graveyard, ResolveCard)),
                        1,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );
                    }
                    break;
                #endregion
                #region Warp
                case CardTemplate.Warp:
                    {
                        cardType = CardType.Channel;
                        rarity = Rarity.Common;

                        orderCost = 2;

                        castRange = 5;
                        castEffect = new MoveToTileEffect(new ChooseHexCard(c => !c.IsHeroic), new ChooseHexTile(t => t.Passable));
                        castDescription = "Move target non-heroic creature to target tile.";
                    }
                    break;
                #endregion    
                #region Hosro
                case CardTemplate.Hosro:
                    {
                        cardType = CardType.Creature;
                        rarity = Rarity.Uncommon;
                        baseRace = Race.Human;

                        baseMovement = 3;
                        basePower = 4;
                        baseToughness = 2;

                        chaosCost = 4;
                        greyCost = 1;

                        keywordAbilities.Add(KeywordAbility.Fervor);
                        keywordAbilities.Add(KeywordAbility.Kingslayer);
                    }
                    break;
                #endregion
                #region Iradj
                case CardTemplate.Iradj:
                    {
                        cardType = CardType.Creature;
                        rarity = Rarity.Uncommon;
                        baseRace = Race.Beast;

                        baseMovement = 2;
                        basePower = 3;
                        baseToughness = 4;

                        mightCost = 4;
                        greyCost = 1;

                        keywordAbilities.Add(KeywordAbility.Elusion);
                        keywordAbilities.Add(KeywordAbility.Ambush);
                    }
                    break;
                #endregion
                #region Jabroncho
                case CardTemplate.Jabroncho:
                    {
                        cardType = CardType.Creature;
                        rarity = Rarity.Legendary;
                        baseRace = Race.Human;

                        baseMovement = 3;
                        basePower = 3;
                        baseToughness = 3;

                        natureCost = 4;
                        AddEtBLambda(
                            "Whenever Jabroni enters the battlefield from the battlefield under your control, summon a green 2/2/2 Makaroni token with flying.",
                            new Foo(SummonTokensEffect(ResolveController, CardTemplate.Makaroni)), 2);
                    }
                    break;
                #endregion
                #region Archfather
                case CardTemplate.Archfather:
                    {
                        cardType = CardType.Creature;
                        rarity = Rarity.Rare;
                        baseRace = Race.Vampire;

                        baseMovement = 2;
                        basePower = 2;
                        baseToughness = 2;

                        deathCost = 3;
                        greyCost = 1;

                        AddTriggeredAbility(String.Format("Whenever another {0} creature enters the battlefield under your control Archfather gets +2/+2.", G.colouredGlyph(ManaColour.Death)),
                            new Foo(new ModifyEffect(Add(2), Forever, ResolveCard, sg(ModifiableStats.Power, ModifiableStats.Toughness))), 
                            new Foo(),
                            new TypedGameEventFilter<MoveToPileEvent>(e => e.card.colours.Contains(ManaColour.Death) && e.card.Controller.IsHero && e.to.location.pile == PileLocation.Field && e.card != this),
                            -1,
                            PileLocation.Field,
                            false, 
                            TriggeredAbility.Timing.Post);
                    }
                    break;
                #endregion
                #region Hungry Felhound
                case CardTemplate.Hungry_sFelhound:
                    {
                        cardType = CardType.Creature;
                        rarity = Rarity.Uncommon;
                        baseRace = Race.Demon;

                        deathCost = 1;

                        baseMovement = 3;
                        basePower = 1;
                        baseToughness = 1;

                        keywordAbilities.Add(KeywordAbility.Fervor);
                    }
                    break;
                #endregion
                #region Vincennes
                case CardTemplate.Vincennes:
                    {
                        cardType = CardType.Creature;
                        rarity = Rarity.Rare;
                        baseRace = Race.Mecha;

                        greyCost = 4;

                        baseMovement = 1;
                        basePower = 2;
                        baseToughness = 3;

                        AddActivatedAbility(String.Format(
                                "{1}, {0}: Vincennes deals 2 damage to target flying creature within 3 tiles.",
                                G.exhaustGhyph, G.colourlessGlyph(2)),
                                new Foo(new PingEffect(2, ResolveCard, new ChooseHexCard(c => c.HasKeyword(KeywordAbility.Flying)))),
                                new Foo(ExhaustThis, ResolverPaysManaEffect(ManaColour.Colourless, ManaColour.Colourless)),
                                3,
                                PileLocation.Field,
                                CastSpeed.Channel);
                    }
                    break;
                #endregion
                #region Ilatian Ghoul
                case CardTemplate.Ilatian_sGhoul:
                    {
                        cardType = CardType.Creature;
                        rarity = Rarity.Uncommon;
                        baseRace = Race.Undead;

                        deathCost = 2;
                        greyCost = 2;

                        baseMovement = 2;
                        basePower = 2;
                        baseToughness = 3;

                        AddActivatedAbility(
                        "You may cast Ilatian Ghoul from the graveyard.",
                        new Foo(new SummonToTileEffect(ResolveCard, new ChooseHexTile(true, t => t.Summonable))),
                        new Foo(ResolverPaysManaEffect(ManaColour.Colourless, ManaColour.Death)),
                        2,
                        PileLocation.Graveyard,
                        CastSpeed.Channel,
                        true
                        );
                    }
                    break;
                #endregion
                #region Illegal Goblin Laboratory
                case CardTemplate.Illegal_sGoblin_sLaboratory:
                {
                    cardType = CardType.Relic;
                    rarity = Rarity.Uncommon;

                    chaosCost = 2;
                    greyCost = 2;

                    AddTriggeredAbility(
                        "At the end of your turn deal 1 damage to every enemy heroic creature.",
                        new Foo(new PingEffect(1, ResolveCard, VillainsCard)),
                        new Foo(),
                        StartOfHeros(Steps.End),
                        0,
                        PileLocation.Field,
                        false
                        );
                } break;

                #endregion
                #region Bhewas

                case CardTemplate.Bhewas:
                {
                    cardType = CardType.Creature;
                    baseRace = Race.Human;
                    subtype = Subtype.Warrior;
                    rarity = Rarity.Legendary;
                    isHeroic = true;
                    forceColour = ManaColour.Life;

                    baseMovement = 2;
                    basePower = 1;
                    baseToughness = 25;

                    AddActivatedAbility(
                        String.Format("{2}{1}{1}, {0}: Your other white creatures get +1/+0 until end of turn. {3}",
                            G.exhaustGhyph, G.colouredGlyph(ManaColour.Life), G.colourlessGlyph(1), G.channelOnly),
                        new Foo(new ModifyEffect(Add(1), EndOfTurn, new AllCardsRule(c => c.Controller.IsHero && c.Is(ManaColour.Life)), sg(ModifiableStats.Power) )),
                        new Foo(ExhaustThis, ResolverPaysManaEffect(ManaColour.Life, ManaColour.Life, ManaColour.Colourless)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Channel
                        );
                } break;
                    #endregion
                #region Kappa
                case CardTemplate.Kappa:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Uncommon;
                    baseRace = Race.Beast;

                    baseMovement = 2;
                    basePower = 2;
                    baseToughness = 3;

                    orderCost = 3;
                    greyCost = 2;

                    AddEtBLambda(
                        "When Kappa enters the battlefield under your control, draw two cards.",
                        new Foo(new DrawCardsEffect(2, ResolveController)));

                } break;

                #endregion
                #region Yung Lich
                case CardTemplate.Yung_sLich:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Rare;
                    baseRace = Race.Undead;
                    subtype = Subtype.Wizard;

                    baseMovement = 2;
                    basePower = 2;
                    baseToughness = 2;

                    orderCost = 1;
                    deathCost = 1;

                    AddDiesLambda(
                        "Whenever Yung Lich enters your graveyard from the field draw a card.",
                        new Foo(new DrawCardsEffect(1, new ResolvePlayerRule(ResolvePlayerRule.Rule.ResolveController))));
                } break;

                #endregion
                #region Cantrip

            case CardTemplate.Cantrip:
            {
                cardType = CardType.Interrupt;
                rarity = Rarity.Common;

                orderCost = 1;
                castEffect = new DrawCardsEffect(1, ResolveController);
                additionalCastEffects.Add(new DisplayEffect(new PlayersCardsRule(new ChooseHexPlayer(), PileLocation.Hand), ResolveController));
                castDescription = "Look at target players hand. Draw a card.";

            }
                break;

                #endregion
                #region Zap
                case CardTemplate.Zap:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Common;

                    chaosCost = 1;

                    castRange = 3;
                    castEffect = new PingEffect(2, ResolveCard, new ChooseHexCard(c => c.IsCreature));
                    castDescription = "Deal 2 damage to target creature.";
                } break;

                #endregion
                #region Temple Healer

            case CardTemplate.Temple_sHealer:
            {
                cardType = CardType.Creature;
                baseRace = Race.Human;
                subtype = Subtype.Cleric;
                rarity = Rarity.Uncommon;

                basePower = 3;
                baseToughness = 4;
                baseMovement = 3;

                lifeCost = 2;
                greyCost = 2;

                AddTriggeredAbility(
                    "Whenever a creature enters the battlefield under your control restore 1 toughness to your heroic creatures.",
                    new Foo(new PingEffect(-1, ResolveCard, ResolveControllerCard)),
                    new Foo(),
                    new TypedGameEventFilter<MoveToPileEvent>(moveEvent =>
                        moveEvent.card.Controller == Controller &&
                        moveEvent.to.location.pile == PileLocation.Field),
                    0,
                    PileLocation.Field,
                    false,
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
                baseRace = Race.Human;
                subtype = Subtype.Warrior;
                isHeroic = true;

                baseMovement = 3;
                basePower = 1;
                baseToughness = 20;
                forceColour = ManaColour.Nature;

                AddActivatedAbility(
                    String.Format("{1}, {0}: Exhaust another target creature within 3 tiles.", G.exhaustGhyph, G.colouredGlyph(ManaColour.Nature)),
                    new Foo (new FatigueEffect(new ChooseHexCard(c => c.IsCreature && c != this), c => c.Movement)),
                    new Foo(ExhaustThis, ResolverPaysManaEffect(ManaColour.Nature)),
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
                baseRace = Race.Zombie;

                basePower = 2;
                baseToughness = 1;
                baseMovement = 2;

                deathCost = 1;
            }
                break;

                #endregion
                #region Shibby Shtank

            case CardTemplate.Shibby_sShtank:
            {
                cardType = CardType.Creature;
                baseRace = Race.Human;
                subtype = Subtype.Wizard;
                rarity = Rarity.Legendary;
                isHeroic = true;
                forceColour = ManaColour.Order;

                baseMovement = 1;
                basePower = 1;
                baseToughness = 20;


                AddActivatedAbility(
                    String.Format("{0}{0}{1}, {2}: Draw a card. {3}", G.colouredGlyph(ManaColour.Order),
                        G.colourlessGlyph(2), G.exhaustGhyph, G.channelOnly),
                    new Foo(new DrawCardsEffect(1, ResolveController)),
                    new Foo(ResolverPaysManaEffect(ManaColour.Order, ManaColour.Order, ManaColour.Colourless, ManaColour.Colourless)),
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

                orderCost = 1;
                greyCost = 1;


                castRange = 4;

                castEffect = new MoveToPileEffect(PileLocation.Hand, new ChooseHexCard(c => c.IsCreature && !c.IsHeroic));
                castDescription = "Return target non-heroic creature to its owner's hand.";
            }
                break;

                #endregion
                #region Rockhand Ogre

            case CardTemplate.Rockhand_sEchion:
            {
                cardType = CardType.Creature;
                rarity = Rarity.Uncommon;
                baseRace = Race.Giant;

                baseMovement = 3;
                basePower = 2;
                baseToughness = 1;

                mightCost = 1;
            }
                break;

                #endregion
                #region Primeordial Chimera

            case CardTemplate.Primordial_sChimera:
            {
                cardType = CardType.Creature;
                rarity = Rarity.Common;
                baseRace = Race.Beast;

                baseMovement = 3;
                basePower = 2;
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
                castEffect = new PingEffect(3, ResolveCard, new ChooseHexCard(c => c.IsCreature));
                additionalCastEffects.Add(new PingEffect(-3, ResolveCard, ResolveControllerCard));
                castDescription = "Deal 3 damage to target creature. You gain 3 life.";
            }
                break;

                #endregion
                #region Goblin Grenade

            case CardTemplate.Goblin_sGrenade:
            {
                cardType = CardType.Interrupt;
                rarity = Rarity.Common;

                chaosCost = 1;
                greyCost = 1;

                castRange = 3;
                castEffect = new PingEffect(1, ResolveCard, new AoERule(1));
                castDescription = "Deal 1 damage to all creatures within radius 1 of target tile.";
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

                castEffect = new MoveToTileEffect(ResolveControllerCard, new ChooseHexTile(t => t.Passable));
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

                castEffect = new GainBonusManaEffect(ResolveController, sg(ManaColour.Nature, ManaColour.Nature, ManaColour.Nature));
                castDescription = String.Format("Gain {0}{0}{0} until end of step.",G.colouredGlyph(ManaColour.Nature));
            }
                break;

                #endregion
                #region Graverobber Syrdin

            case CardTemplate.Graverobber_sSyrdin:
            {
                cardType = CardType.Creature;
                rarity = Rarity.Legendary;
                baseRace = Race.Human;
                subtype = Subtype.Rogue;

                baseMovement = 3;
                basePower = 3;
                baseToughness = 3;

                deathCost = 1;
                lifeCost = 1;
                natureCost = 1;

                AddTriggeredAbility(
                    "Whenever Graverobber Syrdin creature enters the battlefield, you may return a channel or interrupt from your graveyard to your hand.",
                    new Foo(new MoveToPileEffect(PileLocation.Hand,
                        new ChooseCardsFromPile(ResolveController, PileLocation.Graveyard, false, 1,
                            c => c.Is(CardType.Interrupt) || c.Is(CardType.Channel), ChooseCardsFromCards.Mode.PlayerLooksAtPlayer))),
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
                #region Sinister Pact

            case CardTemplate.Sinister_sPact:
            {
                cardType = CardType.Channel;
                rarity = Rarity.Common;

                deathCost = 1;

                castDescription = "Search your deck for a card. Shuffle your deck then put the selected card on top.";
                castEffect = new MoveToPileEffect(PileLocation.Deck, new PlayersCardsRule(ResolveController, PileLocation.Deck));
                additionalCastEffects.Add(new ShuffleEffect(ResolveController));
                additionalCastEffects.Add(new MoveToPileEffect(PileLocation.Deck, new ModifyRule<Card, Card>(0, 0, c => c)));
            }
                break;

                #endregion
                #region Fresh Fox

            case CardTemplate.Fresh_sFox:
            {
                cardType = CardType.Creature;
                rarity = Rarity.Uncommon;
                baseRace = Race.Beast;

                baseToughness = 2;
                basePower = 3;
                baseMovement = 4;

                natureCost = 2;
                greyCost = 1;

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
                castEffect = new ModifyEffect(Add(3), Forever, new ChooseHexCard(c => c.IsCreature), sg(ModifiableStats.Toughness));
                castDescription = "Target creature gains 3 toughness.";
            }
                break;

                #endregion
                #region Survival Instincts

            case CardTemplate.Survival_sInstincts:
            {
                cardType = CardType.Interrupt;
                rarity = Rarity.Common;

                castRange = 4;

                natureCost = 1;

                castDescription = "Target creature gets +2/+2 until end of turn.";

                castEffect = new ModifyEffect(Add(2), EndOfTurn, new ChooseHexCard(c => c.IsCreature),
                sg(ModifiableStats.Power, ModifiableStats.Toughness));
            } break;

                #endregion
                #region Baby Dragon

            case CardTemplate.Baby_sDragon:
            {
                cardType = CardType.Creature;
                rarity = Rarity.Common;
                baseRace = Race.Dragon;

                basePower = 1;
                baseToughness = 1;
                baseMovement = 2;

                chaosCost = 1;


                AddEtBLambda(
                    "When Baby Dragon enters the battlefield deal 1 damage to target creature within 3 tiles.",
                    new Foo(new PingEffect(1, ResolveCard, new ChooseHexCard(c => c.IsCreature && c != this))),
                    3
                    );
            }
                break;

                #endregion
                #region Huntress of Nibemem

            case CardTemplate.Huntress_sOf_sNibemem:
            {
                cardType = CardType.Creature;
                baseRace = Race.Human;
                subtype = Subtype.Hunter;
                rarity = Rarity.Common;

                natureCost = 1;
                greyCost = 2;

                baseToughness = 3;
                basePower = 2;
                baseMovement = 3;

                AddActivatedAbility(
                    String.Format("{0}: Deal 1 damage to target creature within 4 tiles.", G.exhaustGhyph),
                    new Foo(new PingEffect(1, ResolveCard, new ChooseHexCard(c => c.IsCreature))),
                    new Foo(new FatigueEffect(ResolveCard, c => c.Movement)),
                    4,
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
                castEffect = SummonTokensEffect(ResolveController, CardTemplate.Squire, CardTemplate.Squire);

            }
                break;

                #endregion
                #region Wilt

            case CardTemplate.Wilt:
            {
                cardType = CardType.Channel;
                rarity = Rarity.Uncommon;

                deathCost = 2;

                castDescription = "Look at target players hand and select a creature card. Selected card is discarded.";
                castEffect = new MoveToPileEffect(PileLocation.Graveyard,
                    new ChooseCardsFromPile(new ChooseHexPlayer(), PileLocation.Hand, true, 1, ChooseCardsFromCards.Mode.ResolverLooksAtPlayer));
            }
                break;

                #endregion
                #region Price Ila

            case CardTemplate.Prince_sIla:
            {
                cardType = CardType.Creature;
                rarity = Rarity.Legendary;
                baseRace = Race.Undead;
                subtype = Subtype.Rogue;
                isHeroic = true;
                forceColour = ManaColour.Death;

                baseMovement = 2;
                basePower = 1;
                baseToughness = 20;

                AddActivatedAbility(
                    String.Format("{1}{1}, {0}: Each player discards a card. {2}", G.exhaustGhyph,
                        G.colouredGlyph(ManaColour.Death), G.channelOnly),
                    new Foo(new MoveToPileEffect(PileLocation.Graveyard,
                        new ChooseCardsFromPile(new ResolvePlayerRule(ResolvePlayerRule.Rule.AllPlayers),
                            PileLocation.Hand, true, 1, ChooseCardsFromCards.Mode.PlayerLooksAtPlayer))),
                    new Foo(ExhaustThis, ResolverPaysManaEffect(ManaColour.Death, ManaColour.Death)),
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
                baseRace = Race.Beast;

                baseMovement = 2;
                basePower = 1;
                baseToughness = 5;

                orderCost = 2;
                greyCost = 4;

                auras.Add(new Aura(
                    "Kraken gets +1/+0 for each card in your hand.",
                    v => v + Controller.hand.Count,
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
                baseRace = Race.Undead;

                baseMovement = 3;
                basePower = 0;
                baseToughness = 4;

                deathCost = 2;

                auras.Add(new Aura(
                    "Ilas Gravekeeper gets +1/+0 for each zombie in your graveyard.",
                    v => v + Controller.graveyard.Count(c => c.race == Race.Zombie),
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
                baseRace = Race.Beast;

                baseMovement = 3;
                baseToughness = 1;
                basePower = 1;

                natureCost = 1;

                keywordAbilities.Add(KeywordAbility.Elusion);

                auras.Add(new Aura(
                    "Frenzied Pirhana gets +2/+2 as long as your graveyard contains five or more cards.",
                    v => v + (Controller.graveyard.Count >= 5 ? 2 : 0),
                    ModifiableStats.Power,
                    c => c == this,
                    PileLocation.Field
                    ));

                auras.Add(new Aura(
                    "",
                    v => v + (Controller.graveyard.Count >= 5 ? 2 : 0),
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
                baseRace = Race.Zombie;
                rarity = Rarity.Common;

                baseMovement = 2;
                baseToughness = 1;
                basePower = 1;

                deathCost = 1;
                greyCost = 1;

                AddActivatedAbility(
                    "You may cast Ilatian Haunter from the graveyard.",
                    new Foo(new SummonToTileEffect(ResolveCard, new ChooseHexTile(true, t => t.Summonable))),
                    new Foo(ResolverPaysManaEffect(ManaColour.Colourless, ManaColour.Death)),
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
                castEffect = new FatigueEffect(new ChooseHexCard(c => c.IsCreature), c => -c.fatigue);
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
                castEffect = new MoveToPileEffect(PileLocation.Graveyard,
                    new ChooseAnyCard(c => c.location.pile == PileLocation.Stack && !c.isDummy));
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
                castEffect = new MoveToPileEffect(PileLocation.Graveyard, new ChooseHexCard(c => c.IsCreature && !c.IsHeroic));
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
                castEffect = new MoveToPileEffect(PileLocation.Graveyard, new ChooseHexCard(c => c.Is(CardType.Relic)));
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
                    castEffect = new MoveToPileEffect(PileLocation.Graveyard, new AllCardsRule(c => !c.isHeroic)); 
                    } break;

                #endregion
                #region Rider of Death

                case CardTemplate.Rider_sof_sDeath:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Legendary;

                    basePower = 6;
                    baseToughness = 7;
                    baseMovement = 4;

                    deathCost = 5;
                    greyCost = 4;

                        AddEtBLambda(
                        "Whenever Rider of Death enters the battlefield, destroy target non-heroic creature.",
                        new Foo(new MoveToPileEffect(PileLocation.Graveyard, new ChooseHexCard(c => c.IsCreature && !c.IsHeroic))),
                        -1
                        );
                } break;

                #endregion
                #region Rider of War

                case CardTemplate.Rider_sof_sWar:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Legendary;

                    basePower = 5;
                    baseToughness = 7;
                    baseMovement = 4;

                    mightCost = 5;
                    greyCost = 4;

                    keywordAbilities.Add(KeywordAbility.Kingslayer);
                    keywordAbilities.Add(KeywordAbility.Ambush);


                } break;

                #endregion
                #region Rider of Famine
                case CardTemplate.Rider_sof_sFamine:
                    {
                        cardType = CardType.Creature;
                    rarity = Rarity.Legendary;

                    basePower = 3;
                    baseToughness = 8;
                    baseMovement = 4;

                    chaosCost = 5;
                    greyCost = 4;

                        AddTriggeredAbility(
                            "Whenever Rider of Famine deals damage to a player that player discards a card.",
                            new Foo(new MoveToPileEffect(PileLocation.Graveyard,
                                new ChooseCardsFromPile(
                                    new TriggeredRule<DamageEvent, Player>(de => de.target.Controller),
                                    PileLocation.Hand, true, 1, ChooseCardsFromCards.Mode.PlayerLooksAtPlayer))),
                            new Foo(),
                            new TypedGameEventFilter<DamageEvent>(e => e.source == this && e.target.IsHeroic),
                            0,
                            PileLocation.Field,
                            false
                            );
                    } break;
                #endregion
                #region Rider of Pestilence
                case CardTemplate.Rider_sof_sPestilence:
                    {
                        cardType = CardType.Creature;
                        rarity = Rarity.Legendary;

                        basePower = 2;
                        baseToughness = 10;
                        baseMovement = 4;

                        natureCost = 5;
                        greyCost = 4;

                        AddTriggeredAbility(
                            "Whenever Rider of Pestilence deals damage to a player that player sacrifices a non-heroic creature.",
                            new Foo(new MoveToPileEffect(PileLocation.Graveyard,
                                new ChooseCardsFromPile(
                                    new TriggeredRule<DamageEvent, Player>(de => de.target.Controller),
                                    PileLocation.Field, true, 1, ChooseCardsFromCards.Mode.PlayerLooksAtPlayer))),
                            new Foo(),
                            new TypedGameEventFilter<DamageEvent>(e => e.source == this && e.target.IsHeroic),
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

                    AddTriggeredAbility(
                        "Whenever a player draws a card deal 1 damage to that player.",
                        new Foo(new PingEffect(1, ResolveCard, new TriggeredRule<DrawEvent, Card>(de => de.player.heroCard))),
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

                    castDescription = "Apply 3 stacks of crippled to target non-white creature.";
                    castEffect = new ApplyStacksEffect(new ChooseHexCard(c => c.IsCreature && !c.Is(ManaColour.Life)), sg(Counter.Crippled, Counter.Crippled, Counter.Crippled));
                    castRange = 4;
                } break;
                #endregion
                #region Chains of Sin
                case CardTemplate.Chains_sof_sSin:
                    {
                        cardType = CardType.Channel;
                        rarity = Rarity.Common;

                        deathCost = 1;

                        castDescription = "Apply 3 stacks of crippled to target non-black creature.";
                        castEffect = new ApplyStacksEffect(new ChooseHexCard(c => c.IsCreature && !c.Is(ManaColour.Death)), sg(Counter.Crippled, Counter.Crippled, Counter.Crippled));
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
                    castEffect = new MoveToPileEffect(PileLocation.Graveyard, new ChooseHexCard(c => c.Is(CardType.Relic)));
                    castRange = 6;
                } break;

                #endregion
                #region Deep Fry

                case CardTemplate.Deep_sFry:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Uncommon;

                    chaosCost = 2;
                    greyCost = 2;

                    castDescription = "Deal 4 damage to target creature then exhaust it.";
                    castEffect = new PingEffect(4, ResolveCard, new ChooseHexCard());
                    additionalCastEffects.Add(new FatigueEffect(new ModifyRule<Card, Card>(0, 1, c => c), c => c.movement));
                    castRange = 4;

                } break;
                #endregion
                #region Reanimate Dead
                case CardTemplate.Raise_sDead:
                {
                    cardType = CardType.Channel;
                    rarity = Rarity.Uncommon;

                    deathCost = 2;
                    greyCost = 3;

                    castRange = 2;
                    castDescription = "Return a creature from your graveyard to the battlefield under your control.";
                    castEffect =
                        new SummonToTileEffect(
                            new ChooseCardsFromPile(ResolveController, PileLocation.Graveyard, false, 1, ChooseCardsFromCards.Mode.PlayerLooksAtPlayer), 
                            new ChooseHexTile(c => c.Summonable));
                } break;
                #endregion
                #region Ancient Druid
                case CardTemplate.Ancient_sDruid:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Common;
                    baseRace = Race.Elf;
                    subtype = Subtype.Cleric;

                    natureCost = 1;

                    baseToughness = 1;
                    baseMovement = 1;
                    basePower = 1;

                    AddActivatedAbility(
                        String.Format("{0}: Gain {1} until end of step.", G.exhaustGhyph,
                            G.colouredGlyph(ManaColour.Nature)),
                        new Foo(new GainBonusManaEffect(ResolveController, sg(ManaColour.Nature))),
                        new Foo(ExhaustThis),
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
                    castEffect = new MoveToPileEffect(PileLocation.Deck, new ChooseHexCard(c => !c.IsHeroic));
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
                    castEffect = new MoveToPileEffect(PileLocation.Displaced, new ChooseHexCard(c => c.IsCreature && c.IsExhausted));
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
                    castEffect = new FatigueEffect(new ChooseHexCard(c => c.IsCreature), c => -c.fatigue);
                    additionalCastEffects.Add(new ModifyEffect(Add(2), EndOfTurn, new ModifyRule<Card, Card>(0, 0, c => c), sg(ModifiableStats.Power)));
                    castRange = 3;
                } break;
                #endregion
                #region Ilas Bargain
                case CardTemplate.Ilas_sBargain:
                {
                    cardType = CardType.Channel;
                    rarity = Rarity.Uncommon;

                    deathCost = 2;

                    castDescription = "As an additional cost to casting this card sacrifice a non-heroic creature. Draw two cards.";
                    castEffect = new DrawCardsEffect(2, ResolveController);
                    additionalCastCosts.Add(new MoveToPileEffect(PileLocation.Graveyard, new ChooseHexCard(c => c.IsCreature && !c.IsHeroic)));
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

                    mightCost = 3;

                    keywordAbilities.Add(KeywordAbility.Ambush);
                } break;
                #endregion
                #region Sanguine Artisan
                case CardTemplate.Sanguine_sArtisan:
                    {
                        cardType = CardType.Creature;
                        baseRace = Race.Vampire;
                        rarity = Rarity.Common;

                        basePower = 0;
                        baseToughness = 2;
                        baseMovement = 2;

                        deathCost = 2;
                        greyCost = 1;


                        AddTriggeredAbility(
                            "Whenever a creature enters the graveyard from the battlefield under your control, Sanguine Artisan deals 1 damage to target heroic creature.",
                            new Foo(new PingEffect(1, ResolveCard, new ChooseHexCard(c => c.IsHeroic))),
                            new Foo(),
                            new TypedGameEventFilter<MoveToPileEvent>(moveEvent =>
                                moveEvent.card.Controller == Controller &&
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
                    baseRace = Race.Human;
                    rarity = Rarity.Common;

                    natureCost = 4;
                    greyCost = 2;

                    basePower = 3;
                    baseToughness = 5;
                    baseMovement = 3;

                    AddTriggeredAbility(
                        "Whenever Houndmaster deals damage you may summon a green 2/2/2 Wolf token.",
                        new Foo(SummonTokensEffect(ResolveController, CardTemplate.Wolf)),
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
                    baseRace = Race.Mecha;
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
                    baseRace = Race.Demon;
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

                    chaosCost = 2;
                    greyCost = 1;

                    castRange = 3;

                    castEffect = new PingEffect(3, ResolveCard, new ChooseHexCard(c => c.IsCreature && !c.IsHeroic));
                    additionalCastEffects.Add(new PingEffect(3, ResolveCard, new ModifyRule<Card, Card>(0, 1, c => c.Controller.heroCard)));
                    castDescription = "Deal 3 damage to target non-heroic creature and 3 damage to that creatures controller.";

                } break;
                #endregion
                #region Solemn Lotus
                case CardTemplate.Solemn_sLotus:
                {
                    cardType = CardType.Relic;
                    rarity = Rarity.Uncommon;

                    greyCost = 1;

                    baseMovement = 1;

                    AddActivatedAbility(
                        String.Format("{0}, {1}: Gain {2} until end of step.",
                            G.colourlessGlyph(1), G.exhaustGhyph, G.colouredGlyph(ManaColour.Death)),
                        new Foo(new GainBonusManaEffect(ResolveController, sg(ManaColour.Death))),
                        new Foo(ExhaustThis, ResolverPaysManaEffect(ManaColour.Colourless)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );
                        
                    AddActivatedAbility(
                        String.Format("{0}{0}, {1}, Sacrifice Solemn Lotus: Target player sacrifices a non-heroic creature.", G.colouredGlyph(ManaColour.Death), G.exhaustGhyph),
                        new Foo(new MoveToPileEffect(PileLocation.Graveyard, new ChooseCardsFromPile(new ChooseHexPlayer(), PileLocation.Field, true, 1, c => c.IsCreature && !c.IsHeroic, ChooseCardsFromCards.Mode.PlayerLooksAtPlayer))),
                        new Foo(ExhaustThis, ResolverPaysManaEffect(ManaColour.Death, ManaColour.Death), SacThis),
                        -1,
                        PileLocation.Field, 
                        CastSpeed.Interrupt
                        );

                } break;
                #endregion
                #region Mysterious Lilac
                case CardTemplate.Mysterious_sLilac:
                {
                    cardType = CardType.Relic;
                    rarity = Rarity.Uncommon;

                    greyCost = 1;

                    baseMovement = 1;

                    AddActivatedAbility(
                        String.Format("{0}, {1}: Gain {2} until end of step.",
                            G.colourlessGlyph(1), G.exhaustGhyph, G.colouredGlyph(ManaColour.Order)),
                        new Foo(new GainBonusManaEffect(ResolveController, sg(ManaColour.Order))),
                        new Foo(ExhaustThis, ResolverPaysManaEffect(ManaColour.Colourless)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );

                        AddActivatedAbility(
                        String.Format("{0}{0}, {1}, Sacrifice Mysterious Lilac: Draw a card.", G.colouredGlyph(ManaColour.Order), G.exhaustGhyph),
                        new Foo(new DrawCardsEffect(1, ResolveController)),
                        new Foo(ExhaustThis, ResolverPaysManaEffect(ManaColour.Order, ManaColour.Order), SacThis),
                        -1,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );

                } break;
                #endregion
                #region Daring Poppy
                case CardTemplate.Daring_sPoppy:
                    {
                        cardType = CardType.Relic;
                        rarity = Rarity.Uncommon;

                        greyCost = 1;

                        baseMovement = 1;

                        AddActivatedAbility(
                        String.Format("{0}, {1}: Gain {2} until end of step.",
                            G.colourlessGlyph(1), G.exhaustGhyph, G.colouredGlyph(ManaColour.Chaos)),
                        new Foo(new GainBonusManaEffect(ResolveController, sg(ManaColour.Chaos))),
                        new Foo(ExhaustThis, ResolverPaysManaEffect(ManaColour.Colourless)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );

                        AddActivatedAbility(
                            String.Format("{0}{0}, {1}, Sacrifice Daring Poppy: Deal 2 damage to target creature.", G.colouredGlyph(ManaColour.Chaos), G.exhaustGhyph),
                            new Foo(new PingEffect(2, ResolveCard, new ChooseHexCard(c => c.IsCreature))),
                            new Foo(ExhaustThis, ResolverPaysManaEffect(ManaColour.Chaos, ManaColour.Chaos), SacThis),
                            -1,
                            PileLocation.Field,
                            CastSpeed.Interrupt
                            );

                    }
                    break;
                #endregion
                #region Serene Dandelion
                case CardTemplate.Serene_sDandelion:
                    {
                        cardType = CardType.Relic;
                        rarity = Rarity.Uncommon;

                        greyCost = 1;

                        baseMovement = 1;

                        AddActivatedAbility(
                        String.Format("{0}, {1}: Gain {2} until end of step.",
                            G.colourlessGlyph(1), G.exhaustGhyph, G.colouredGlyph(ManaColour.Life)),
                        new Foo(new GainBonusManaEffect(ResolveController, sg(ManaColour.Life))),
                        new Foo(ExhaustThis, ResolverPaysManaEffect(ManaColour.Colourless)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );

                        AddActivatedAbility(
                            String.Format("{0}{0}, {1}, Sacrifice Serene Dandelion: Restore 4 toughness to target creature.", G.colouredGlyph(ManaColour.Life), G.exhaustGhyph),
                            new Foo(new PingEffect(-4, ResolveCard, new ChooseHexCard(c => c.IsCreature))),
                            new Foo(ExhaustThis, ResolverPaysManaEffect(ManaColour.Life, ManaColour.Life), SacThis),
                            -1,
                            PileLocation.Field,
                            CastSpeed.Interrupt
                            );

                    }
                    break;
                #endregion
                #region Stark Lily
                case CardTemplate.Stark_sLily:
                {
                    cardType = CardType.Relic;
                    rarity = Rarity.Uncommon;

                    greyCost = 1;

                    baseMovement = 1;

                    AddActivatedAbility(
                        String.Format("{0}, {1}: Gain {2} until end of step.",
                            G.colourlessGlyph(1), G.exhaustGhyph, G.colouredGlyph(ManaColour.Might)),
                        new Foo(new GainBonusManaEffect(ResolveController, sg(ManaColour.Might))),
                        new Foo(ExhaustThis, ResolverPaysManaEffect(ManaColour.Colourless)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );

                        AddActivatedAbility(
                        String.Format("{0}{0}, {1}, Sacrifice Stark Lily: Summon a purple 2/2 Gryphon token with Flying.", G.colouredGlyph(ManaColour.Might), G.exhaustGhyph),
                        new Foo(SummonTokensEffect(ResolveController, CardTemplate.Gryphon)),
                        new Foo(ExhaustThis, ResolverPaysManaEffect(ManaColour.Might, ManaColour.Might), SacThis),
                        2,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );

                } break;
                #endregion
                #region Vibrant Zinnia
                case CardTemplate.Vibrant_sZinnia:
                    {
                        cardType = CardType.Relic;
                        rarity = Rarity.Uncommon;

                        greyCost = 1;

                        baseMovement = 1;

                        AddActivatedAbility(
                        String.Format("{0}, {1}: Gain {2} until end of step.",
                            G.colourlessGlyph(1), G.exhaustGhyph, G.colouredGlyph(ManaColour.Nature)),
                        new Foo(new GainBonusManaEffect(ResolveController, sg(ManaColour.Nature))),
                        new Foo(ExhaustThis, ResolverPaysManaEffect(ManaColour.Colourless)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );

                        AddActivatedAbility(
                            String.Format("{0}{0}, {1}, Sacrifice Vibrant Zinnia: Target non-heroic creature gets +2/+2", G.colouredGlyph(ManaColour.Nature), G.exhaustGhyph),
                            new Foo(new ModifyEffect(Add(2), Forever, new ChooseHexCard(c => c.IsCreature), sg(ModifiableStats.Power, ModifiableStats.Toughness))),
                            new Foo(ExhaustThis, ResolverPaysManaEffect(ManaColour.Nature, ManaColour.Nature), SacThis),
                            -1,
                            PileLocation.Field,
                            CastSpeed.Interrupt
                            );

                    }
                    break;
                #endregion
                #region Primal Chopter
                case CardTemplate.Primal_sChopter:
                {
                    cardType = CardType.Creature;
                    baseRace = Race.Mecha;
                    rarity = Rarity.Common;

                    greyCost = 2;

                    basePower = 1;
                    baseToughness = 2;
                    baseMovement = 3;

                    keywordAbilities.Add(KeywordAbility.Flying);
                } break;
                #endregion
                #region Famished Tarantula
                case CardTemplate.Famished_sTarantula:
                {
                    cardType = CardType.Creature;
                    baseRace = Race.Beast;
                    rarity = Rarity.Uncommon;

                    basePower = 1;
                    baseToughness = 3;
                    baseMovement = 3;

                    natureCost = 2;
                    greyCost = 1;

                    AddDeathtouchLambda();

                    keywordAbilities.Add(KeywordAbility.Wingclipper);

                } break;
                #endregion
                #region Morenian Medic
                case CardTemplate.Morenian_sMedic:
                {
                    cardType = CardType.Creature;
                    baseRace = Race.Human;
                    subtype = Subtype.Warrior;
                    rarity = Rarity.Uncommon;

                    basePower = 2;
                    baseToughness = 2;
                    baseMovement = 2;

                    lifeCost = 2;

                    AddEtBLambda(
                        "When Morenian Medic enters the battlefield, restore 2 toughness to target creature within 3 tiles.",
                        new Foo(new PingEffect(-2, ResolveCard, new ChooseHexCard(c => c.IsCreature))),
                        3);

                } break;
                #endregion
                #region Bubastis
                case CardTemplate.Bubastis:
                {
                    cardType = CardType.Creature;
                    baseRace = Race.Beast;
                    rarity = Rarity.Legendary;

                    basePower = 4;
                    baseToughness = 5;
                    baseMovement = 3;

                    orderCost = 4;
                    greyCost = 3;

                    AddEtBLambda(
                        "When Bubastis enters the battlefield, you may return another target non-heroic permanent within 3 tiles to it's owners hand.",
                        new Foo(new MoveToPileEffect(PileLocation.Hand, new ChooseHexCard(c => !c.IsHeroic))),
                        3,
                        true);
                } break;
                #endregion
                #region Unyeilding Stalwart
                case CardTemplate.Unyeilding_sStalwart:
                {
                    cardType = CardType.Creature;
                    baseRace = Race.Human;
                    subtype = Subtype.Warrior;
                    rarity = Rarity.Common;

                    lifeCost = 1;

                    basePower = 1;
                    baseToughness = 1;
                    baseMovement = 3;

                    AddDiesLambda(
                        "When Unyeilding Stalwart enters the graveyard from the battlefield under your control, summon a 1/1 Spirit token with Flying.",
                        new Foo(SummonTokensEffect(ResolveController, CardTemplate.Spirit)),
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

                    baseMovement = 1;

                    AddActivatedAbility(
                        String.Format(
                            "{0}{1}, {2}, Displace a creature card from your graveyard: Summon a white 1/1 Spirit token with flying. {3}",
                            G.colouredGlyph(ManaColour.Life), G.colouredGlyph(ManaColour.Death), G.exhaustGhyph,
                            G.channelOnly),
                        new Foo(SummonTokensEffect(ResolveController, CardTemplate.Spirit)),
                        new Foo(
                            ExhaustThis,
                            ResolverPaysManaEffect(ManaColour.Death, ManaColour.Life),
                            new MoveToPileEffect(
                                PileLocation.Displaced,
                                new ChooseCardsFromPile(ResolveController, PileLocation.Graveyard, false, 1,
                                    c => c.IsCreature, ChooseCardsFromCards.Mode.PlayerLooksAtPlayer))
                            ),
                        2,
                        PileLocation.Field,
                        CastSpeed.Channel);

                } break;
                #endregion
                #region Enraged Dragon
                case CardTemplate.Enraged_sDragon:
                {
                    cardType = CardType.Creature;
                    baseRace = Race.Dragon;
                    rarity = Rarity.Common;

                    basePower = 1;
                    baseToughness = 4;
                    baseMovement = 4;

                    chaosCost = 2;
                    greyCost = 2;

                    AddActivatedAbility(
                        String.Format("{0}: Enraged Dragon gets +1/+0 until end of turn.", G.colouredGlyph(ManaColour.Chaos)),
                        new Foo(new ModifyEffect(Add(1), EndOfTurn, ResolveCard, sg(ModifiableStats.Power))),
                        new Foo(ResolverPaysManaEffect(ManaColour.Chaos)),
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
                    baseRace = Race.Beast;
                    rarity = Rarity.Uncommon;

                    natureCost = 2;

                    basePower = 1;
                    baseToughness = 2;
                    baseMovement = 3;

                    auras.Add(new Aura(
                        "Chromatic Unicorn gets +1/+1 for each colour among cards in graveyards.",
                        v => v + (owner.game.allPlayers.SelectMany(p => p.graveyard.cards).SelectMany(c => c.colours).Where(c => c != ManaColour.Colourless).Distinct().Count()),
                        ModifiableStats.Power,
                        c => c == this,
                        PileLocation.Field
                        ));

                    auras.Add(new Aura(
                        "",
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
                    baseRace = Race.Angel;
                    subtype = Subtype.Guardian;
                    rarity = Rarity.Common;

                    baseToughness = 2;
                    basePower = 1;
                    baseMovement = 4;

                    lifeCost = 3;
                    greyCost = 2;

                    AddEtBLambda(
                        "When Seraph enters the battlefield it gets +1/+1 for every non-heroic you control.",
                        new Foo(new ModifyEffect(
                            v =>
                                v +
                                Controller.field.cards.Where(c => c.cardType == CardType.Creature && !c.IsHeroic)
                                    .Count(),
                            Forever,
                            ResolveCard,
                            sg(ModifiableStats.Power, ModifiableStats.Toughness))));

                    keywordAbilities.Add(KeywordAbility.Flying);

                } break;
                #endregion
                #region Moratian Battle Standard
                case CardTemplate.Moratian_sBattle_sStandard:
                {
                    cardType = CardType.Relic;
                    rarity = Rarity.Uncommon;

                    lifeCost = 3;
                    greyCost = 1;

                    auras.Add(new Aura("",
                        Add(1),
                        ModifiableStats.Toughness,
                        c => c.Controller == this.Controller && !c.IsHeroic && c.isColour(ManaColour.Life),
                        PileLocation.Field
                        ));

                    auras.Add(new Aura("Your non-heroic white creatures get +1/+1.",
                        Add(1),
                        ModifiableStats.Power,
                        c => c.Controller == this.Controller && !c.IsHeroic && c.isColour(ManaColour.Life),
                        PileLocation.Field
                        ));
                } break;
                #endregion
                #region Flamekindler
                case CardTemplate.Flamekindler:
                {
                    cardType = CardType.Creature;
                    baseRace = Race.Human;
                    subtype = Subtype.Wizard;
                    rarity = Rarity.Rare;

                    chaosCost = 2;

                    basePower = 0;
                    baseToughness = 3;
                    baseMovement = 2;

                    AddActivatedAbility(
                        String.Format(
                            "{0}, {1}, Displace a channel or interrupt card from your graveyard: Deal 2 damage to target creature within 5 tiles.",
                            G.colouredGlyph(ManaColour.Chaos), G.exhaustGhyph),
                        new Foo(new PingEffect(2, ResolveCard, new ChooseHexCard(c => c.IsCreature))),
                        new Foo(
                            ExhaustThis,
                            new MoveToPileEffect(
                                PileLocation.Displaced,
                                new ChooseCardsFromPile(
                                    ResolveController,
                                    PileLocation.Graveyard,
                                    false,
                                    1,
                                    c => c.Is(CardType.Interrupt) || c.Is(CardType.Channel),
                                    ChooseCardsFromCards.Mode.PlayerLooksAtPlayer)),
                            ResolverPaysManaEffect(ManaColour.Chaos)),
                        5,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );
                } break;
                #endregion
                #region Commander Commander_sSparryz
                case CardTemplate.Commander_sSparryz:
                {
                    cardType = CardType.Creature;
                    baseRace = Race.Demon;
                    subtype = Subtype.Wizard;
                    rarity = Rarity.Legendary;
                    isHeroic = true;
                    forceColour = ManaColour.Chaos;

                    baseMovement = 2;
                    basePower = 1;
                    baseToughness = 20;

                    AddActivatedAbility(
                        String.Format("{1}{1}, {0}: Deal 2 damage to all heroic creatures.",
                            G.exhaustGhyph, G.colouredGlyph(ManaColour.Chaos)),
                        new Foo(new PingEffect(2, ResolveCard, new AllCardsRule(c => c.IsHeroic))),
                        new Foo(ExhaustThis, ResolverPaysManaEffect(ManaColour.Chaos, ManaColour.Chaos)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );
                } break;
                #endregion
                #region Shimmering Koi
                case CardTemplate.Shimmering_sKoi:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Common;
                    baseRace = Race.Beast;

                    baseToughness = 4;
                    basePower = 3;
                    baseMovement = 3;

                    natureCost = 2;
                    greyCost = 1;

                    keywordAbilities.Add(KeywordAbility.Elusion);
                } break;
                #endregion
                #region Decaying Horror
                case CardTemplate.Decaying_sHorror:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Rare;
                    baseRace = Race.Zombie;

                    basePower = 5;
                    baseToughness = 6;
                    baseMovement = 3;

                    greyCost = 1;
                    deathCost = 2;

                    AddTriggeredAbility(
                        "At the start of your turn, Decaying Horror gets -1/-1.",
                        new Foo(new ModifyEffect(Add(-1), Forever, ResolveCard, sg(ModifiableStats.Power, ModifiableStats.Toughness))),
                        new Foo(),
                        StartOfHeros(Steps.Replenish)
                        );

                    } break;


                #endregion
                #region Relentless Consriptor
                case CardTemplate.Relentless_sConsriptor:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Common;
                    baseRace = Race.Human;
                    subtype = Subtype.Warrior;

                    lifeCost = 3;
                    greyCost = 2;

                    basePower = 3;
                    baseToughness = 4;
                    baseMovement = 3;

                    AddTriggeredAbility(
                        "At the end of your turn you may summon a white 1/1 Squire token.",
                        new Foo(SummonTokensEffect(ResolveController, CardTemplate.Squire)),
                        new Foo(),
                        StartOfHeros(Steps.End),
                        2,
                        PileLocation.Field,
                        true
                    );

                    } break;
                #endregion
                #region Terminate
                case CardTemplate.Terminate:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Uncommon;

                    mightCost = 2;
                    greyCost = 1;

                    castDescription = "Destroy target damaged non-heroic creature.";
                    castEffect = new MoveToPileEffect(PileLocation.Graveyard, new ChooseHexCard(c => c.IsCreature && c.damageTaken > 0));
                    castRange = 4;
                    } break;
                #endregion
                #region Spirit of Salvation
                case CardTemplate.Spirit_sof_sSalvation:
                {
                    cardType = CardType.Creature;
                    baseRace = Race.Spirit;
                    subtype = Subtype.Cleric;
                    rarity = Rarity.Uncommon;

                    basePower = 2;
                    baseToughness = 6;
                    baseMovement = 3;

                    lifeCost = 3;
                    greyCost = 3;

                    keywordAbilities.Add(KeywordAbility.Reinforcement);

                    Effect e1 = new MoveToPileEffect(PileLocation.Displaced, new ChooseHexCard(c => c.Controller == this.Controller && !c.isHeroic && c.race != Race.Spirit));
                    Effect e2 = new SummonToTileEffect(new ModifyRule<Card, Card>(0, 0, c => c), new ChooseHexTile(true, t => t.Summonable));

                        AddEtBLambda(
                            "Whenever Spirit of Salvation enters the battlefield you may displace target non-spirit, non-heroic creature you control then summon it to the battlefield to another target tile within 2 tiles.",
                            new Foo(e1, e2),
                            2, 
                            true);
                        
                    } break;
                #endregion
                #region Benedictor
                case CardTemplate.Benedictor:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Common;
                    baseRace = Race.Human;
                    subtype = Subtype.Guardian;

                    lifeCost = 2;

                    baseToughness = 1;
                    basePower = 1;
                    baseMovement = 3;

                    AddEtBLambda(
                        "When Benedictor enters the battlefield, dispace all cards in players graveyards.",
                        new Foo(new MoveToPileEffect(PileLocation.Displaced, new AllCardsRule(c => c.location.pile == PileLocation.Graveyard))));
                } break;
                #endregion
                #region Pyrostorm
                case CardTemplate.Pyrostorm:
                {
                    cardType = CardType.Channel;
                    rarity = Rarity.Common;

                    chaosCost = 3;
                    greyCost = 1;

                    castDescription = "Deal 2 damage to all creatures.";
                    castEffect = new PingEffect(2, ResolveCard, new AllCardsRule(c => c.location.pile == PileLocation.Field));
                } break;
                #endregion
                #region Elven Cultivator
                case CardTemplate.Elven_sCultivator:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Common;
                    baseRace = Race.Elf;

                    basePower = 3;
                    baseToughness = 2;
                    baseMovement = 3;

                    natureCost = 2;
                    greyCost = 1;

                    AddEtBLambda(
                        "When Elven Cultivator enters the battlefield give another non-heroic creature you control +1/+1.",
                        new Foo(new ModifyEffect(Add(1), Forever, new ChooseHexCard(c => c.IsCreature && !c.IsHeroic), sg(ModifiableStats.Power, ModifiableStats.Toughness))),
                        3);

                } break;
                #endregion
                #region Faceless Sphinx
                case CardTemplate.Faceless_sSphinx:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Rare;
                    baseRace = Race.Beast;
                    subtype = Subtype.Guardian;

                    basePower = 4;
                    baseToughness = 5;
                    baseMovement = 3;

                    orderCost = 4;
                    greyCost = 3;

                    keywordAbilities.Add(KeywordAbility.Flying);

                    AddTriggeredAbility(
                        "Whenever an opponent draws a card you may draw a card.",
                        new Foo(new DrawCardsEffect(1, ResolveController)),
                        new Foo(),
                        new TypedGameEventFilter<DrawEvent>(e => e.player != Controller),
                        0,
                        PileLocation.Field, 
                        true
                        );
                } break;
                #endregion
                #region Cerberus
                case CardTemplate.Cerberus:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Common;
                    baseRace = Race.Beast;

                    chaosCost = 4;
                    greyCost = 1;

                    basePower = 6;
                    baseToughness = 4;
                    baseMovement = 3;

                    keywordAbilities.Add(KeywordAbility.Ambush);

                } break;
                #endregion
                #region Heroic Might
                case CardTemplate.Heroic_sMight:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Common;

                    mightCost = 2;

                    castRange = 5;
                    castDescription = "Give target damaged non-heroic creature +2/+2.";
                    castEffect = new ModifyEffect(Add(2), Forever,
                        new ChooseHexCard(c => c.IsCreature && !c.IsHeroic && c.IsDamaged),
                        sg(ModifiableStats.Power, ModifiableStats.Toughness));
                } break;
                #endregion
                #region Taouy Ruins
                case CardTemplate.Taouy_sRuins:
                {
                    cardType = CardType.Relic;
                    rarity = Rarity.Rare;

                    greyCost = 2;

                    baseMovement = 1;
                        /*
                    AddActivatedAbility(
                        String.Format("{0}, {1}: Gain one mana of any colour until end of step.",
                        G.colourlessGlyph(1), G.exhaustGhyph),
                        new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController),
                        new ChooseRule<ManaOrb>(ChooseRule<ManaOrb>.ChooseAt.Resolve)),
                        new GainBonusManaEffect(),
                        new Foo(ExhaustThis, ResolverPaysManaEffect(ManaColour.Colourless)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );
                        */
                    } break;
                #endregion
                #region Shibby's Saboteur
                case CardTemplate.Shibby_as_sSaboteur:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Uncommon;
                    baseRace = Race.Human;
                    subtype = Subtype.Rogue;

                    orderCost = 2;
                    greyCost = 2;

                    basePower = 2;
                    baseToughness = 2;
                    baseMovement = 3;

                    keywordAbilities.Add(KeywordAbility.Reinforcement);

                    AddEtBLambda(
                        "When Shibby's Saboteur enters the battlefield, you set target other non-heroic creature's power to 1.",
                        new Foo(new ModifyEffect(Set(1), Forever, new ChooseHexCard(c => c.IsCreature), sg(ModifiableStats.Power)))
                        );
                } break;
                #endregion
                #region Brute Force
                case CardTemplate.Brute_sForce:
                {
                    cardType = CardType.Channel;
                    rarity = Rarity.Common;

                    mightCost = 1;
                    greyCost = 1;

                    castDescription = "Your heroic creatures get +3/+0 until end of turn.";
                    castEffect = new ModifyEffect(Add(3), EndOfTurn, ResolveControllerCard, sg(ModifiableStats.Power));

                } break;
                #endregion
                #region Scroll of Earth
                case CardTemplate.Scroll_sof_sEarth:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Common;

                    natureCost = 1;

                    castRange = 2;

                    castDescription = "Summon two 0/4 Rock tokens.";
                    castEffect = SummonTokensEffect(ResolveController, CardTemplate.Rock, CardTemplate.Rock);
                } break;
                #endregion
                #region Malificent Spirit
                case CardTemplate.Maleficent_sSpirit:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Common;
                    baseRace = Race.Spirit;

                    deathCost = 3;
                    greyCost = 2;

                    basePower = 2;
                    baseToughness = 4;
                    baseMovement = 2;

                    keywordAbilities.Add(KeywordAbility.Flying);

                    AddEtBLambda(
                        "When Malificent Spirit enters the battlefield you may look at target players hand and select a card from it. The selected card is discarded.",
                        new Foo(
                            new MoveToPileEffect(
                                PileLocation.Graveyard,
                                new ChooseCardsFromPile(
                                    new ChooseHexPlayer(),
                                    PileLocation.Hand,
                                    true,
                                    1,
                                    c => true,
                                    ChooseCardsFromCards.Mode.ResolverLooksAtPlayer))));
                } break;
                #endregion
                #region Thirstclaw
                case CardTemplate.Thirstclaw:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Common;
                    baseRace = Race.Beast;

                    mightCost = 2;
                    greyCost = 2;

                    basePower = 2;
                    baseToughness = 5;
                    baseMovement = 4;

                    keywordAbilities.Add(KeywordAbility.Fervor);
                } break;
                #endregion
                #region Flameheart Pheonix
                case CardTemplate.Flameheart_sPhoenix:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Rare;
                    baseRace = Race.Elemental;
                    subtype = Subtype.Guardian;

                    lifeCost = 2;
                    chaosCost = 2;
                    greyCost = 1;

                    basePower = 3;
                    baseToughness = 3;
                    baseMovement = 3;

                    keywordAbilities.Add(KeywordAbility.Flying);

                    AddDiesLambda(
                        "Whenever Flameheart Pheonix enters the graveyard from the battlefield, you may return it to your hand.",
                        new Foo(new MoveToPileEffect(PileLocation.Hand, ResolveCard)),
                        -1,
                        true
                        );

                } break;
                #endregion
                #region Lord Plevin
                case CardTemplate.Lord_sPlombie:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Legendary;
                    isHeroic = true;
                    forceColour = ManaColour.Might;
                    baseRace = Race.Human;
                    subtype = Subtype.Warrior;

                    basePower = 2;
                    baseToughness = 25;
                    baseMovement = 2;

                    AddActivatedAbility(
                        String.Format("{1}, {0}: Deal 1 damage to target creature within 1 tile.", G.exhaustGhyph, G.colouredGlyph(ManaColour.Might)),
                        new Foo(new PingEffect(1, ResolveCard, new ChooseHexCard(c => c.IsCreature))),
                        new Foo(ExhaustThis, ResolverPaysManaEffect(ManaColour.Might)),
                        1,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );

                    flavourText =
                        "\"What he lacks in brains he makes up for in... You see where I'm going with this?\" -- Shibby Shtank";
                } break;
                #endregion
                #region Gryphon Rider
                case CardTemplate.Gryphon_sRider:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Uncommon;
                    baseRace = Race.Human;

                    basePower = 3;
                    baseToughness = 3;
                    baseMovement = 4;

                    mightCost = 3;
                    greyCost = 1;

                    keywordAbilities.Add(KeywordAbility.Flying);

                    AddDiesLambda(
                        "Whenever Gryphon Rider enters the graveyard from the battlefield, summon a 2/2/2 Gryphon token with flying.",
                        new Foo (SummonTokensEffect(ResolveController, CardTemplate.Gryphon)),
                        2);
                } break;
                #endregion
                #region Ravaging Greed
                case CardTemplate.Ravaging_sGreed:
                {
                    cardType = CardType.Channel;
                    rarity = Rarity.Uncommon;

                    chaosCost = 2;

                    castDescription = "Discard a card. Draw two cards.";
                    castEffect = new MoveToPileEffect(PileLocation.Graveyard,
                        new ChooseCardsFromPile(ResolveController, PileLocation.Hand, false, 2,
                            ChooseCardsFromCards.Mode.PlayerLooksAtPlayer));
                    additionalCastEffects.Add(new DrawCardsEffect(2, ResolveController));
                } break;
                #endregion
                #region Water Bolt
                case CardTemplate.Water_sBolt:
                {
                    cardType = CardType.Interrupt;
                    rarity = Rarity.Common;

                    chaosCost = 1;
                    orderCost = 1;

                    castDescription = "Deal 1 damage to target creature. Draw a card.";
                    castRange = 4;
                    castEffect = new PingEffect(1, ResolveCard, new ChooseHexCard(c => c.IsCreature));
                    additionalCastEffects.Add(new DrawCardsEffect(1, ResolveController));

                } break;
                #endregion
                #region Lone Ranger
                case CardTemplate.Lone_sRanger:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Uncommon;
                    baseRace = Race.Human;
                    subtype = Subtype.Hunter;

                    natureCost = 2;

                    basePower = 1;
                    baseToughness = 2;
                    baseMovement = 3;

                    auras.Add(new Aura(
                        "Lone Ranger gets +2/+2 as long as you control no other non-heroic creatures.",
                        v => v + (Controller.field.Count(c => c != this && c.cardType == CardType.Creature && !c.IsHeroic) == 0 ? 2 : 0),
                        ModifiableStats.Power,
                        c => c == this,
                        PileLocation.Field
                        ));

                    auras.Add(new Aura(
                        "",
                        v => v + (Controller.field.Count(c => c != this && c.cardType == CardType.Creature && !c.IsHeroic) == 0 ? 2 : 0),
                        ModifiableStats.Toughness,
                        c => c == this,
                        PileLocation.Field
                        ));
                    } break;
                #endregion
                #region Charging Bull
                case CardTemplate.Charging_sBull:
                    {
                        cardType = CardType.Creature;
                        rarity = Rarity.Uncommon;
                        baseRace = Race.Beast;

                        natureCost = 2;
                        basePower = 4;
                        baseToughness = 3;
                        baseMovement = 0;

                        keywordAbilities.Add(KeywordAbility.Fervor);

                        AddTriggeredAbility(
                        "Whenever Charging Bull takes damage set Charging Bull's movement to 0.",
                        new Foo(new ModifyEffect(Set(0), Forever, ResolveCard, sg(ModifiableStats.Movement))),
                        new Foo(),
                        new TypedGameEventFilter<DamageEvent>(e => e.source != this && e.target == this),
                        0
                        );

                        AddActivatedAbility(
                            String.Format("{0}: set base movement to 5.", G.exhaustGhyph),
                            new Foo(new ModifyEffect(Set(5), Forever, ResolveCard, sg(ModifiableStats.Movement))),
                            new Foo(ExhaustThis),
                            0,
                            PileLocation.Field,
                            CastSpeed.Channel);          
                    }
                    break;
                #endregion
                #region Ent
                case CardTemplate.Ent:
                    {
                        cardType = CardType.Creature;
                        rarity = Rarity.Uncommon;
                        baseRace = Race.Elemental;

                        natureCost = 0;
                        basePower = 1;
                        baseToughness = 1;
                        baseMovement = 0;

   
                        keywordAbilities.Add(KeywordAbility.Fervor);
                        AddActivatedAbility(
                        String.Format("{0}: Ent gets +1/+1.", G.colouredGlyph(ManaColour.Nature)),
                        new Foo(new ModifyEffect(Add(1), Forever, ResolveCard, sg(ModifiableStats.Power, ModifiableStats.Toughness))),
                        new Foo(ResolverPaysManaEffect(ManaColour.Nature)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );

                        AddActivatedAbility(
                            String.Format("{0}: set base movement to 2.", G.exhaustGhyph),
                            new Foo(new ModifyEffect(Set(2), Forever, ResolveCard, sg(ModifiableStats.Movement))),
                            new Foo(ExhaustThis),
                            0,
                            PileLocation.Field,
                            CastSpeed.Channel
                            );
                    }
                    break;
                #endregion
                #region AOE_EXHAUST
                case CardTemplate.AOE_sEXHAUST:
                    {
                        /*
                        cardType = CardType.Creature;
                        rarity = Rarity.Uncommon;
                        baseRace = Race.Human;
                        subtype = Subtype.Hunter;
                        natureCost = 1;

                        basePower = 0;
                        baseToughness = 3;
                        baseMovement = 0;

                        etbLambda("Deal 2 damage to self.", new Effect(new TargetRuleSet(resolveCard, resolveCard), new PingDoer(2)), 0);

                        addTriggeredAbility(
                            "Exhaust: Heal AOE EXHAUST one hp at the end of your turn. "//{0} Whenever a creature enters the graveyard from the battlefield under your control, Sanguine Artisan deals 1 damage to target heroic creature.",
                            new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                                player),
                            new PingDoer(1),
                            new Foo(),
                            new TypedGameEventFilter<MoveToPileEvent>(moveEvent =>
                                moveEvent.card.controller == controller &&
                                moveEvent.to.location.pile == PileLocation.Graveyard &&
                                moveEvent.card.location.pile == PileLocation.Field),
                            -1,
                            PileLocation.Field,
                            false
                            );
                        addActivatedAbility(String.Format("{0}, Exhaust: Target non-Heroic creature becomes Undead and gets -1/-1. Count Fera II gets +1/+1.", G.colouredGlyph(ManaColour.Death)),
                            new Effect[] { e1, e2, e3, e4 },
                            new Foo(exhaustThis, manaCostEffect(ManaColour.Death)),//new Foo(new Effect(new TargetRuleSet(new ChooseRule<Card>(c => !c.isHeroic)), new MoveToPileDoer(PileLocation.Graveyard))),
                            -1,
                            PileLocation.Field,
                            CastSpeed.Interrupt);*/
                    }
                    break;
                #endregion

                #region tokens

                #region Spirit
                case CardTemplate.Spirit:
                    {
                        forceColour = ManaColour.Life;
                        basePower = 1;
                        baseToughness = 1;
                        baseMovement = 2;
                        baseRace = Race.Spirit;
                        isToken = true;
                        keywordAbilities.Add(KeywordAbility.Flying);
                    }
                    break;
                #endregion
                #region Gryphon
                case CardTemplate.Gryphon:
                    {
                        forceColour = ManaColour.Might;
                        basePower = 2;
                        baseToughness = 2;
                        baseMovement = 3;
                        baseRace = Race.Beast;
                        isToken = true;
                        keywordAbilities.Add(KeywordAbility.Flying);
                    }
                    break;
                #endregion
                #region Squire
                case CardTemplate.Squire:
                    {
                        forceColour = ManaColour.Life;
                        basePower = 1;
                        baseToughness = 1;
                        baseMovement = 2;
                        baseRace = Race.Human;
                        subtype = Subtype.Warrior;
                        isToken = true;
                    }
                    break;
                #endregion
                #region Wolf
                case CardTemplate.Wolf:
                    {
                        forceColour = ManaColour.Nature;
                        basePower = 2;
                        baseToughness = 2;
                        baseMovement = 3;
                        baseRace = Race.Beast;
                        isToken = true;
                    }
                    break;
                #endregion
                #region Zombie
                case CardTemplate.Zombie:
                {
                    forceColour = ManaColour.Death;
                    basePower = 1;
                    baseToughness = 1;
                    baseMovement = 2;
                    baseRace = Race.Zombie;
                    isToken = true;
                } break;
                #endregion
                #region Rock
                case CardTemplate.Rock:
                {
                    basePower = 0;
                    baseToughness = 4;
                    baseMovement = 0;
                    baseRace = Race.Elemental;
                    isToken = true;
                } break;
                #endregion
                    
                #region Makaroni
                case CardTemplate.Makaroni:
                    {
                        forceColour = ManaColour.Nature;
                        cardType = CardType.Creature;
                        baseRace = Race.Beast;
                        isToken = true;
                        baseMovement = 2;
                        basePower = 2;
                        baseToughness = 2;
                        keywordAbilities.Add(KeywordAbility.Flying);
                    }
                    break;
                #endregion
                    
                #region Spiderling
                case CardTemplate.Spiderling:
                {
                    forceColour = ManaColour.Nature;
                    basePower = 1;
                    baseToughness = 1;
                    baseMovement = 3;
                    baseRace = Race.Beast;
                    isToken = true;
                }
                break;
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
            forceRace = new Modifiable(-1);

            modifiables = new Modifiable[]
            {
                Power,
                Toughness,
                Movement,
                forceRace,
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
                castSpeed = HasKeyword(KeywordAbility.Reinforcement) ? CastSpeed.Interrupt : CastSpeed.Channel;
                castRange = 2;
                castEffect = new SummonToTileEffect(ResolveCard, new ChooseHexTile(true, t => t.Summonable));
            }
            else throw new Exception();
            
            if (castEffect == null) throw new Exception("these don't show up anyway");
            List<Effect> es = new List<Effect>();

            es.Add(castEffect);
            es.AddRange(additionalCastEffects);

            additionalCastCosts.Add(ResolverPaysManaEffect(castManaCost));

            castAbility = new ActivatedAbility(
                this, 
                PileLocation.Hand, 
                castRange, 
                new Foo(es.ToArray()),
                new Foo(additionalCastCosts.ToArray()), 
                castSpeed, 
                castDescription
                );
            activatedAbilities.Add(castAbility);

            this.owner = owner;
            Controller = owner;

            name = G.replaceUnderscoresAndShit(ct.ToString());

            eventHandler = generateDefaultEventHandlers();
        }


        private void AddTriggeredAbility(string description, Foo effect, Foo cost, GameEventFilter filter, int castRange = -1, PileLocation activeIn = PileLocation.Field, bool optional = false, TriggeredAbility.Timing timing = TriggeredAbility.Timing.Pre)
        {
            TriggeredAbility ta = new TriggeredAbility(this, activeIn, effect, castRange, cost, filter, optional, timing, description);
            triggeredAbilities.Add(ta);
        }

        private void AddActivatedAbility(string description, Foo effect, Foo cost, int castRange, PileLocation activeIn, CastSpeed castSpeed, bool alternateCast = false)
        {
            ActivatedAbility aa = new ActivatedAbility(this, activeIn, castRange, effect, cost, castSpeed, description);
            activatedAbilities.Add(aa);
            if (alternateCast) alternateCasts.Add(aa);
        }
    }
}
