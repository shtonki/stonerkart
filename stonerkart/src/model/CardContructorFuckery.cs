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
            flavourText = "";
            CastSpeed castSpeed;
            List<Effect> additionalCastCosts = new List<Effect>();
            keywordAbilities = new List<KeywordAbility>();


            #region oophell

            switch (ct)
            {
                #region Great White Buffalo
                case CardTemplate.Great_sWhite_sBuffalo:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Rare;

                    lifeCost = 2;
                    natureCost = 2;
                    greyCost = 2;

                    basePower = 7;
                    baseToughness = 7;
                    baseMovement = 3;
                    
                    etbLambda(
                        "When Great White Buffalo enters the battlefield restore 5 toughness to your heroic creatures.",
                        new Effect(
                            new TargetRuleSet(
                                new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                                new CardResolveRule(CardResolveRule.Rule.ResolveControllerCard)),
                            new PingDoer(-5)));
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
                    castEffect = new Effect(
                        new ChooseRule<Card>(
                            new SelectCardRule(PileLocation.Deck, SelectCardRule.Mode.PlayerLooksAtPlayer, 3),
                            new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController),
                            ChooseRule<Card>.ChooseAt.Resolve, 
                            c => true,
                            3, 
                            false
                            ), 
                        new MoveToPileDoer(PileLocation.Deck));

                    additionalCastEffects.Add(new Effect(
                        new TargetOption(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController)),
                        new ShuffleDoer()));
                        
                    additionalCastEffects.Add(new Effect(
                        new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), 
                        new DrawCardsDoer(1)));
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

                        Effect e1 = new Effect(new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard)),
                           new ModifyDoer(add(1), never, ModifiableStats.Power));
                        Effect e2 = new Effect(new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard)),
                            new ModifyDoer(add(1), never, ModifiableStats.Toughness));
                        Effect e3 = new Effect(new ChooseRule<Card>(ChooseRule<Card>.ChooseAt.Cast, c => !c.isHeroic),
                            new ModifyDoer(add(-1), never, ModifiableStats.Power));
                        Effect e4 = new Effect(new ModifyRule<Card, Card>(2, 0, c => c),
                            new ModifyDoer(add(-1), never, ModifiableStats.Toughness));

                        //should be better considering its legendary, maybe create vampire instead of sacrificing? 
                        addActivatedAbility(String.Format("{0}, Exhaust: Target non-Heroic creature becomes Undead and gets -1/-1. Count Fera II gets +1/+1.", G.colouredGlyph(ManaColour.Death)),
                            new Effect[] {e1, e2, e3, e4},
                            new Foo(exhaustThis, manaCostEffect(ManaColour.Death)),//new Foo(new Effect(new TargetRuleSet(new ChooseRule<Card>(c => !c.isHeroic)), new MoveToPileDoer(PileLocation.Graveyard))),
                            -1,
                            PileLocation.Field,
                            CastSpeed.Interrupt);

                        Effect e5 = new Effect(new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard)),
                           new ModifyDoer(add(-3), never, ModifiableStats.Power));
                        Effect e6 = new Effect(new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard)),
                            new ModifyDoer(add(-3), never, ModifiableStats.Toughness));
                        Effect e7 = new Effect(new ChooseRule<Card>(ChooseRule<Card>.ChooseAt.Cast, c => !c.isHeroic),
                            new ModifyDoer(add(-3), never, ModifiableStats.Power));
                        Effect e8 = new Effect(new ModifyRule<Card, Card>(2, 0, c => c),
                            new ModifyDoer(add(-3), never, ModifiableStats.Toughness));

                        addActivatedAbility(
                            String.Format(
                                "{0}{0}, Exhaust: Target non-Undead, non-Heroic creature gets -3/-3. Count Fera II gets -3/-3.",
                                G.colouredGlyph(ManaColour.Death)),
                            new Effect[] {e5, e6, e7, e8},
                            new Foo(exhaustThis, manaCostEffect(ManaColour.Death, ManaColour.Death)),//manaCostFoo(ManaColour.Chaos), 
                            -1,
                            PileLocation.Field,
                            CastSpeed.Interrupt);
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

                        diesLambda(
                        "When Arachosa enters the Graveyard from the Battlefield summon two Green 1/1 Spiderling Tokens with 3 Movement.",
                        Effect.summonTokensEffect(CardTemplate.Spiderling, CardTemplate.Spiderling),
                        1);


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

                        addTriggeredAbility(
                            "Whenever Paralyzing Spider deals damage to a non-Heroic Creature reduce that Creature's movement speed by 2. This cannot reduce the Creature's Movement below 1.",
                            new Effect(new TargetRuleSet(
                            new TriggeredTargetRule<DamageEvent, Card>(g => g.target)),
                            new ModifyDoer(v => Math.Max(Math.Min(v, 1), v - 2), Card.never, ModifiableStats.Movement)),
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

                        addActivatedAbility(
                        String.Format("Sacrifice this creature: deal 3 damage to another target creature within 1 tile."),
                        new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                        new ChooseRule<Card>(c => c != this)),  new PingDoer(3), new Foo(new Effect(resolveCard, new MoveToPileDoer(PileLocation.Graveyard))),
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
                        orderCost = 1;
                        rarity = Rarity.Common;
                        castRange = 5;
                        castEffect = new Effect(new TargetRuleSet(new ChooseRule<Card>(c => c.isHeroic == false), new ChooseRule<Tile>(t => t.passable)), new MoveToTileDoer(true));
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
                        basePower = 3;
                        baseToughness = 1;
                        chaosCost = 2;
                        keywordAbilities.Add(KeywordAbility.Ambush);
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
                        basePower = 4;
                        baseToughness = 3;

                        mightCost = 2;
                        greyCost = 1;
                    }
                    break;
                #endregion
                #region Jabroni
                case CardTemplate.Jabroni:
                    {
                        cardType = CardType.Creature;
                        rarity = Rarity.Legendary;
                        baseRace = Race.Human;

                        baseMovement = 2;
                        basePower = 3;
                        baseToughness = 3;

                        natureCost = 4;
                        etbLambda(
                            "Whenever Jabroni enters the Field from the battlefield under your control, spawn a 2/2 Makaroni token with Flying.",
                            Effect.summonTokensEffect(CardTemplate.Makaroni), 2, false);
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

                        deathCost = 2;

                        Effect e1 = new Effect(new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                            new ModifyDoer(add(2), never, ModifiableStats.Power, ModifiableStats.Toughness));

                        addTriggeredAbility("Whenever a Black creature enters the Field under your control Archfather gets +2/+2.", e1, 
                            new Foo(),
                            new TypedGameEventFilter<MoveToPileEvent>(e => e.card.colours.Contains(ManaColour.Death) && e.card.controller.isHero && e.to.location.pile == PileLocation.Field),
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

                        addActivatedAbility(String.Format(
                                "{1}, {0}: Vincennes deals 3 damage to Target Flying creature within 3 tiles.",
                                G.exhaustGhyph, G.colourlessGlyph(2)),
                                new Effect(new TargetRuleSet(resolveCard, new ChooseRule<Card>(c => c.hasAbility(KeywordAbility.Flying))), new PingDoer(5)),
                                new Foo(exhaustThis, manaCostEffect(ManaColour.Colourless, ManaColour.Colourless)),
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
                        basePower = 3;
                        baseToughness = 3;

                        addActivatedAbility(
                        "You may cast Ilatian Ghoul from the graveyard.",
                        new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                            new ChooseRule<Tile>(
                                ChooseRule<Tile>.ChooseAt.Resolve,
                                t => t.passable && !t.isEdgy)),
                        new SummonToTileDoer(),
                        new Foo(manaCostEffect(new ManaSet(ManaColour.Colourless, ManaColour.Death))),
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
                    greyCost = 1;

                    addTriggeredAbility(
                        "At the end of your turn deal 1 damage to every enemy heroic creature.",
                        new TargetRuleSet(resolveCard, enemyHeroicCreatures),
                        new PingDoer(1),
                        emptyFoo,
                        startOfHerosStep(Steps.End),
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

                    addActivatedAbility(
                        String.Format("{2}{1}{1}, {0}: Your other white creatures get +1/+0 until end of turn. {3}",
                            G.exhaustGhyph, G.colouredGlyph(ManaColour.Life), G.colourlessGlyph(1), G.channelOnly),
                        new TargetRuleSet(
                            new CardsRule(
                                c => c != this && c.controller == this.controller && c.isColour(ManaColour.Life))),
                        new ModifyDoer(add(1), endOfTurn, ModifiableStats.Power),
                        new Foo(exhaustThis, manaCostEffect(ManaColour.Life, ManaColour.Life, ManaColour.Colourless)),
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
                baseRace = Race.Undead;
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
                        new ChooseRule<Card>(
                            new SelectCardRule(PileLocation.Hand, SelectCardRule.Mode.ResolverLooksAtPlayer),
                            new ChooseRule<Player>(),
                            ChooseRule<Card>.ChooseAt.Resolve, 
                            c => true,
                            0, true),
                        new ModifyDoer(add(0), clearAura, ModifiableStats.Movement))); //ugliest hack i've seen in a while
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
                baseRace = Race.Human;
                subtype = Subtype.Cleric;
                rarity = Rarity.Uncommon;

                basePower = 3;
                baseToughness = 4;
                baseMovement = 3;

                lifeCost = 2;
                greyCost = 2;

                addTriggeredAbility(
                    "Whenever a creature enters the battlefield under your control, restore 1 toughness to your hero.",
                    new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                        new CardResolveRule(CardResolveRule.Rule.ResolveControllerCard)),
                    new PingDoer(-1),
                    new Foo(),
                    new TypedGameEventFilter<MoveToPileEvent>(moveEvent =>
                        moveEvent.card.controller == controller &&
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

                addActivatedAbility(
                    String.Format("{1}, {0}: Exhaust another target creature within 3 tiles.", G.exhaustGhyph, G.colouredGlyph(ManaColour.Nature)),
                    new TargetRuleSet(creature(c => c != this)),
                    new FatigueDoer(true),
                    new Foo(exhaustThis, manaCostEffect(ManaColour.Nature)),
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
                baseRace = Race.Human;
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
                    manaCostFoo(ManaColour.Order, ManaColour.Order, ManaColour.Colourless, ManaColour.Colourless),
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
                    new Effect(nonheroicCreature(),
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
                baseRace = Race.Giant;

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
                baseRace = Race.Beast;

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
                        new PingDoer(-3)));
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

                castRange = 5;
                castEffect =
                    new Effect(
                        new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                            new AoeRule(t => true, 1, c => true)),
                        new PingDoer(1));
                castDescription = "Deal 1 damage to all creatures within 1 tile of target tile.";

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
                    new TargetRuleSet(
                        new CardResolveRule(CardResolveRule.Rule.ResolveControllerCard),
                        new ChooseRule<Tile>(f => f.passable)),//new ClickTileRule(f => f.passable)), 
                    new MoveToTileDoer(true));
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
                baseRace = Race.Human;
                subtype = Subtype.Rogue;

                baseMovement = 3;
                basePower = 3;
                baseToughness = 3;

                deathCost = 1;
                lifeCost = 1;
                natureCost = 1;

                addTriggeredAbility(
                    "Whenever Graverobber Syrdin creature enters the battlefield under your control, you may return a card from your graveyard to your hand.",
                    new TargetRuleSet(new ChooseRule<Card>(new SelectCardRule(PileLocation.Graveyard, SelectCardRule.Mode.PlayerLooksAtPlayer))),
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
                #region Sinister Pact

            case CardTemplate.Sinister_sPact:
            {
                cardType = CardType.Interrupt;
                rarity = Rarity.Common;

                deathCost = 1;

                castEffect =
                    new Effect(
                        new TargetRuleSet(
                            new ChooseRule<Card>(
                                new SelectCardRule(PileLocation.Deck, SelectCardRule.Mode.PlayerLooksAtPlayer),
                                ChooseRule<Card>.ChooseAt.Resolve)),
                        new MoveToPileDoer(PileLocation.Deck));

                additionalCastEffects.Add(new Effect(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), new ShuffleDoer()));
                additionalCastEffects.Add(new Effect(new ModifyRule<Card, Card>(0, 0, c => c), new MoveToPileDoer(PileLocation.Deck)));
                   
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
                baseRace = Race.Beast;

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
                    new TargetRuleSet(new ChooseRule<Card>()),
                    new ModifyDoer(add(3), never, ModifiableStats.Toughness));
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
                        new TargetRuleSet(
                            new CardResolveRule(CardResolveRule.Rule.ResolveCard), 
                            new ChooseRule<Card>()),
                        new PingDoer(-2));
                additionalCastEffects.Add(new Effect(new ModifyRule<Card, Card>(0, 1, c => c),
                    new ModifyDoer(add(2), endOfTurn, ModifiableStats.Power)));
                castDescription =
                    "Target creature is healed for 2 and gains 2 power until the end of this turn.";
            } break;

                #endregion
                #region Baby Dragon

            case CardTemplate.Baby_sDragon:
            {
                cardType = CardType.Creature;
                rarity = Rarity.Common;
                baseRace = Race.Dragon;

                basePower = 2;
                baseToughness = 1;
                baseMovement = 2;

                chaosCost = 1;
                greyCost = 1;


                addTriggeredAbility(
                    "When Baby Dragon enters the battlefield you may have it deal 1 damage to target creature within 3 tiles.",
                    new TargetRuleSet(
                        new CardResolveRule(CardResolveRule.Rule.ResolveCard), 
                        new ChooseRule<Card>()),
                    new PingDoer(1),
                    new Foo(),
                    thisEnters(this, PileLocation.Field),
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
                baseRace = Race.Human;
                subtype = Subtype.Hunter;
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
                        new ChooseRule<Card>(
                            new SelectCardRule(PileLocation.Hand, SelectCardRule.Mode.ResolverLooksAtPlayer),
                            new ChooseRule<Player>(), 
                            ChooseRule<Card>.ChooseAt.Resolve, 
                            c => c.cardType == CardType.Creature),
                        new MoveToPileDoer(PileLocation.Graveyard));
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

                addActivatedAbility(
                    String.Format("{1}{1}, {0}: Each player discards a card. {2}", G.exhaustGhyph, G.colouredGlyph(ManaColour.Death), G.channelOnly),
                    new TargetRuleSet(
                        new ChooseRule<Card>(
                            new SelectCardRule(PileLocation.Hand, SelectCardRule.Mode.PlayerLooksAtPlayer),
                            new PlayerResolveRule(PlayerResolveRule.Rule.AllPlayers), 
                            ChooseRule<Card>.ChooseAt.Resolve, 
                            c => true)), 
                    new MoveToPileDoer(PileLocation.Graveyard),
                    new Foo(exhaustThis, manaCostEffect(ManaColour.Death, ManaColour.Death)),
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
                baseRace = Race.Undead;

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
                baseRace = Race.Beast;

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
                baseRace = Race.Zombie;
                rarity = Rarity.Common;

                baseMovement = 2;
                baseToughness = 1;
                basePower = 1;

                deathCost = 1;
                greyCost = 1;

                addActivatedAbility(
                    "You may cast Ilatian Haunter from the graveyard.",
                    new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                        new ChooseRule<Tile>(
                            ChooseRule<Tile>.ChooseAt.Resolve,
                            t => t.passable && !t.isEdgy)),
                    new SummonToTileDoer(),
                    new Foo(manaCostEffect(new ManaSet(ManaColour.Colourless, ManaColour.Death))),
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
                castEffect = new Effect(new ChooseRule<Card>(), new FatigueDoer(false));
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
                castEffect = new Effect(
                    new ChooseRule<Card>(
                        new ClickCardRule(), 
                        ChooseRule<Card>.ChooseAt.Cast, 
                        c => c.location.pile == PileLocation.Stack && !c.isDummy),
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
                    new Effect(nonheroicCreature(),
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
                    new Effect(relic,
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
                            new CardsRule(c => !c.isHeroic), 
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
                        new TargetRuleSet(nonheroicCreature()),
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
                        new TargetRuleSet(new ChooseRule<Card>(
                            new SelectCardRule(PileLocation.Hand, SelectCardRule.Mode.PlayerLooksAtPlayer),
                            new TriggeredTargetRule<DamageEvent, Player>(e => e.target.controller),
                            ChooseRule<Card>.ChooseAt.Resolve, 
                            c => true)),
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
                            new TargetRuleSet(new ChooseRule<Card>(
                                new SelectCardRule(PileLocation.Field, SelectCardRule.Mode.PlayerLooksAtPlayer),
                                new TriggeredTargetRule<DamageEvent, Player>(e => e.target.controller),
                                ChooseRule<Card>.ChooseAt.Resolve, 
                                c => !c.isHeroic && c.cardType == CardType.Creature)),
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
                        new PingDoer(1),
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
                    castEffect = new Effect(nonColouredCreature(ManaColour.Life),
                        new ModifyDoer(setTo(1), never, ModifiableStats.Movement));
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
                        castEffect = new Effect(nonColouredCreature(ManaColour.Death),
                            new ModifyDoer(v => Math.Max(Math.Min(v, 1), v - 2), never, ModifiableStats.Movement));
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
                            new Effect(relic,
                                new MoveToPileDoer(PileLocation.Graveyard));
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
                    castEffect = zepLambda(4);
                    additionalCastEffects.Add(new Effect(new ModifyRule<Card, Card>(0, 1, c => c), new FatigueDoer(true)));
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
                        new ChooseRule<Card>(
                            new SelectCardRule(PileLocation.Graveyard, SelectCardRule.Mode.PlayerLooksAtPlayer),
                            ChooseRule<Card>.ChooseAt.Cast,
                            c => c.cardType == CardType.Creature),
                        new ChooseRule<Tile>(
                            ChooseRule<Tile>.ChooseAt.Resolve,
                            t => t.passable && !t.isEdgy)),
                        new SummonToTileDoer());
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

                    addActivatedAbility(
                        String.Format("{0}: Gain {1} until end of step.", G.exhaustGhyph, G.colouredGlyph(ManaColour.Nature)),
                        new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), new StaticManaRule(ManaColour.Nature)),
                        new GainBonusManaDoer(),
                        new Foo(exhaustThis),
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
                    castEffect = new Effect(new ChooseRule<Card>(c => !c.isHeroic), new MoveToPileDoer(PileLocation.Deck));
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
                            new ChooseRule<Card>(c => c.cardType == CardType.Creature && !c.isHeroic && c.isExhausted),
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
                            new TargetRuleSet(creature()),
                            new FatigueDoer(false));
                    additionalCastEffects.Add(new Effect(new ModifyRule<Card, Card>(0, 0, c => c),
                        new ModifyDoer(add(2), endOfTurn, ModifiableStats.Power)));
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

                        deathCost = 1;
                        greyCost = 1;


                        addTriggeredAbility(
                            "Whenever a creature enters the graveyard from the battlefield under your control, Sanguine Artisan deals 1 damage to target heroic creature.",
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
                    } break;
                #endregion
                #region Houndmaster
                case CardTemplate.Houndmaster:
                {
                    cardType = CardType.Creature;
                    baseRace = Race.Human;
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

                    castRange = 3;
                    chaosCost = 2;
                    greyCost = 1;

                    castEffect = zepNonHeroicLambda(3);
                    additionalCastEffects.Add(new Effect(
                        new TargetRuleSet(
                            new ModifyRule<Card, Card>(0, 0, c => c),
                            new ModifyRule<Card, Card>(0, 1, c => c.controller.heroCard)),
                        new PingDoer(3)
                        ));
                    castDescription = "Deal 3 damage to target non-heroic creature and 3 damage to that creatures controller.";

                } break;
                #endregion
                #region Solemn Lotus
                case CardTemplate.Solemn_sLotus:
                {
                    cardType = CardType.Relic;
                    rarity = Rarity.Uncommon;
                    //forceColour = ManaColour.Death;

                    greyCost = 1;

                    baseMovement = 1;

                    addActivatedAbility(
                        String.Format("{0}, {1}: Gain {2} until end of step.",
                            G.colourlessGlyph(1), G.exhaustGhyph, G.colouredGlyph(ManaColour.Death)),
                        new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), new StaticManaRule(ManaColour.Death)),
                        new GainBonusManaDoer(),
                        new Foo(exhaustThis, manaCostEffect(ManaColour.Colourless)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );
                        
                    addActivatedAbility(
                        String.Format("{0}{0}, {1}, Sacrifice Solemn Lotus: Target player sacrifices a non-heroic creature.", G.colouredGlyph(ManaColour.Death), G.exhaustGhyph),
                        playerSacLambda(new ChooseRule<Player>()),
                        new Foo(exhaustThis, manaCostEffect(ManaColour.Death, ManaColour.Death), sacThisLambda),
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
                    //forceColour = ManaColour.Order;

                    greyCost = 1;

                    baseMovement = 1;

                    addActivatedAbility(
                        String.Format("{0}, {1}: Gain {2} until end of step.",
                            G.colourlessGlyph(1), G.exhaustGhyph, G.colouredGlyph(ManaColour.Order)),
                        new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController),
                            new StaticManaRule(ManaColour.Order)),
                        new GainBonusManaDoer(),
                        new Foo(exhaustThis, manaCostEffect(ManaColour.Colourless)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );

                        addActivatedAbility(
                        String.Format("{0}{0}, {1}, Sacrifice Mysterious Lilac: Draw a card.", G.colouredGlyph(ManaColour.Order), G.exhaustGhyph),
                        new Effect(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), new DrawCardsDoer(1)),
                        new Foo(exhaustThis, manaCostEffect(ManaColour.Order, ManaColour.Order), sacThisLambda),
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
                        //forceColour = ManaColour.Chaos;

                        greyCost = 1;

                        baseMovement = 1;

                        addActivatedAbility(
                        String.Format("{0}, {1}: Gain {2} until end of step.",
                            G.colourlessGlyph(1), G.exhaustGhyph, G.colouredGlyph(ManaColour.Chaos)),
                        new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), new StaticManaRule(ManaColour.Chaos)),
                        new GainBonusManaDoer(),
                        new Foo(exhaustThis, manaCostEffect(ManaColour.Colourless)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );

                        addActivatedAbility(
                            String.Format("{0}{0}, {1}, Sacrifice Daring Poppy: Deal 2 damage to target creature.", G.colouredGlyph(ManaColour.Chaos), G.exhaustGhyph),
                            zepLambda(2),
                            new Foo(exhaustThis, manaCostEffect(ManaColour.Chaos, ManaColour.Chaos), sacThisLambda),
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
                        //forceColour = ManaColour.Life;

                        greyCost = 1;

                        baseMovement = 1;

                        addActivatedAbility(
                        String.Format("{0}, {1}: Gain {2} until end of step.",
                            G.colourlessGlyph(1), G.exhaustGhyph, G.colouredGlyph(ManaColour.Life)),
                        new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), new StaticManaRule(ManaColour.Life)),
                        new GainBonusManaDoer(),
                        new Foo(exhaustThis, manaCostEffect(ManaColour.Colourless)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );

                        addActivatedAbility(
                            String.Format("{0}{0}, {1}, Sacrifice Serene Dandelion: Restore 4 toughness to target creature.", G.colouredGlyph(ManaColour.Life), G.exhaustGhyph),
                            zepLambda(-4),
                            new Foo(exhaustThis, manaCostEffect(ManaColour.Life, ManaColour.Life), sacThisLambda),
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
                        //forceColour = ManaColour.Might;

                    greyCost = 1;

                    baseMovement = 1;

                    addActivatedAbility(
                        String.Format("{0}, {1}: Gain {2} until end of step.",
                            G.colourlessGlyph(1), G.exhaustGhyph, G.colouredGlyph(ManaColour.Might)),
                        new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController),
                            new StaticManaRule(ManaColour.Might)),
                        new GainBonusManaDoer(),
                        new Foo(exhaustThis, manaCostEffect(ManaColour.Colourless)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );

                        addActivatedAbility(
                        String.Format("{0}{0}, {1}, Sacrifice Stark Lily: Summon a 2/2 Gryphon token with Flying.", G.colouredGlyph(ManaColour.Might), G.exhaustGhyph),
                        Effect.summonTokensEffect(CardTemplate.Gryphon),
                        new Foo(exhaustThis, manaCostEffect(ManaColour.Might, ManaColour.Might), sacThisLambda),
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
                        //forceColour = ManaColour.Nature;

                        greyCost = 1;

                        baseMovement = 1;

                        addActivatedAbility(
                        String.Format("{0}, {1}: Gain {2} until end of step.",
                            G.colourlessGlyph(1), G.exhaustGhyph, G.colouredGlyph(ManaColour.Nature)),
                        new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), new StaticManaRule(ManaColour.Nature)),
                        new GainBonusManaDoer(),
                        new Foo(exhaustThis, manaCostEffect(ManaColour.Colourless)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );


                        Effect e1 = new Effect(nonheroicCreature(),
                            new ModifyDoer(add(2), never, ModifiableStats.Power));
                        Effect e2 = new Effect(new ModifyRule<Card, Card>(0, 0, c => c),
                            new ModifyDoer(add(2), never, ModifiableStats.Toughness));

                        addActivatedAbility(
                            String.Format("{0}{0}, {1}, Sacrifice Vibrant Zinnia: Target non-heroic creature gets +2/+2", G.colouredGlyph(ManaColour.Nature), G.exhaustGhyph),
                            new Effect[] {e1, e2}, 
                            new Foo(exhaustThis, manaCostEffect(ManaColour.Nature, ManaColour.Nature), sacThisLambda),
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
                    baseRace = Race.Mecha;
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
                    baseRace = Race.Beast;
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
                    baseRace = Race.Human;
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
                    baseRace = Race.Beast;
                    rarity = Rarity.Legendary;

                    basePower = 4;
                    baseToughness = 5;
                    baseMovement = 3;

                    orderCost = 3;
                    greyCost = 3;

                    etbLambda(
                        "When Bubastis enters the battlefield you may return another target non-heroic creature within 3 tiles to it's owners hand.",
                        new Effect(nonheroicCreature(c => c != this), new MoveToPileDoer(PileLocation.Hand)),
                        3,
                        true
                        );
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

                    diesLambda(
                        "When Unyeilding Stalwart enters the graveyard from the battlefield under your control, summon a 1/1 Spirit token with Flying.",
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
                            manaCostEffect(ManaColour.Death, ManaColour.Life),
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
                    baseRace = Race.Dragon;
                    rarity = Rarity.Common;

                    basePower = 1;
                    baseToughness = 4;
                    baseMovement = 4;

                    chaosCost = 1;
                    greyCost = 2;

                    addActivatedAbility(
                        String.Format("{0}: Enraged Dragon gets +1/+0 until end of turn.", G.colouredGlyph(ManaColour.Chaos)),
                        new Effect(new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                            new ModifyDoer(add(1), endOfTurn, ModifiableStats.Power)),
                        new Foo(manaCostEffect(ManaColour.Chaos)),
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
                    baseRace = Race.Angel;
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
                        never)
                        );

                    Effect e2 = new Effect(
                        new CardResolveRule(CardResolveRule.Rule.ResolveCard), new ForceStaticModifyDoer(ModifiableStats.Toughness,
                        grtr,
                        never)
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

                    lifeCost = 3;

                    auras.Add(new Aura("",
                        add(1),
                        ModifiableStats.Toughness,
                        c => c.controller == this.controller && !c.isHeroic && c.isColour(ManaColour.Life),
                        PileLocation.Field
                        ));

                    auras.Add(new Aura("Your non-heroic Life creatures get +1/+1.",
                        add(1),
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
                    baseRace = Race.Human;
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
                            manaCostEffect(ManaColour.Chaos),
                            displaceFromGraveyard(c => c.cardType == CardType.Channel || c.cardType == CardType.Interrupt),
                            exhaustThis),
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

                    addActivatedAbility(
                        String.Format("{1}{1}, {0}: Deal 2 damage to all heroic creatures.",
                            G.exhaustGhyph, G.colouredGlyph(ManaColour.Chaos)),
                        new TargetRuleSet(
                            new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                            new CardsRule(c => c.isHeroic)),
                        new PingDoer(2),
                        new Foo(exhaustThis, manaCostEffect(ManaColour.Chaos, ManaColour.Chaos)),
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
                    basePower = 4;
                    baseMovement = 3;

                    natureCost = 2;
                    greyCost = 2;

                    keywordAbilities.Add(KeywordAbility.Elusion);
                } break;
                #endregion
                #region Decaying Horror
                case CardTemplate.Decaying_sHorror:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Rare;
                    baseRace = Race.Zombie;

                    basePower = 2;
                    baseToughness = 4;
                    baseMovement = 2;

                    greyCost = 2;
                    deathCost = 2;

                    addTriggeredAbility(
                        "Whenever Decaying Horror takes damage you may summon a 1/1 Zombie token.",
                        Effect.summonTokensEffect(CardTemplate.Zombie),
                        new Foo(),
                        new TypedGameEventFilter<DamageEvent>(damageEvent => damageEvent.target == this),
                        2,
                        PileLocation.Field,
                        true,
                        TriggeredAbility.Timing.Post
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

                    lifeCost = 2;
                    greyCost = 2;

                    basePower = 2;
                    baseToughness = 3;
                    baseMovement = 3;

                    addTriggeredAbility(
                        "At the end of your turn you may summon a 1/1 Squire token.",
                        Effect.summonTokensEffect(CardTemplate.Squire),
                        new Foo(),
                        new TypedGameEventFilter<StartOfStepEvent>(
                            e => e.step == Steps.End && e.activePlayer == controller),
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
                    castEffect =
                        new Effect(
                            new ChooseRule<Card>(c => c.cardType == CardType.Creature && !c.isHeroic && c.damageTaken > 0),
                            new MoveToPileDoer(PileLocation.Graveyard));
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
                    baseToughness = 5;
                    baseMovement = 3;

                    lifeCost = 3;
                    greyCost = 2;

                    keywordAbilities.Add(KeywordAbility.Reinforcement);

                    Effect e1 = new Effect(new ChooseRule<Card>(c => c.controller == this.controller && !c.isHeroic && c.race != Race.Spirit), new MoveToPileDoer(PileLocation.Displaced));
                        Effect e2 = new Effect(new TargetRuleSet(
                    new ModifyRule<Card, Card>(0, 0, c => c),
                    new ChooseRule<Tile>(
                        ChooseRule<Tile>.ChooseAt.Resolve,
                        t => t.passable && !t.isEdgy)),
                    new SummonToTileDoer(),
                    true
                    );

                        etbLambda(
                            "Whenever Spirit of Salvation enters the battlefield you may displace target non-spirit, non-heroic creature you control then summon it to the battlefield to another target tile within 2 tiles.",
                            new Effect[] {e1, e2},
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

                    etbLambda(
                        "When Benedictor enters the battlefield dispace all cards in players graveyards.",
                        new Effect(new CardsRule(c => c.location.pile == PileLocation.Graveyard), new MoveToPileDoer(PileLocation.Displaced))
                        );
                } break;
                #endregion
                #region Pyrostorm
                case CardTemplate.Pyrostorm:
                {
                    cardType = CardType.Channel;
                    rarity = Rarity.Common;

                    chaosCost = 3;
                    greyCost = 1;

                    castEffect =
                        new Effect(
                            new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                                new CardsRule(c => c.location.pile == PileLocation.Field)), new PingDoer(2));
                    castDescription = "Deal 2 damage to all creatures.";
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

                    natureCost = 1;
                    greyCost = 2;

                    etbLambda(
                        "When Elven Cultivator enters the battlefield give another non-heroic creature you control +1/+1.",
                        new Effect(new ChooseRule<Card>(c => c != this && !c.isHeroic && c.controller == this.controller),
                            new ModifyDoer(add(1), never, ModifiableStats.Power, ModifiableStats.Toughness)),
                        3
                        );

                } break;
                #endregion
                #region Faceless Sphinx
                case CardTemplate.Faceless_sSphinx:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Rare;
                    baseRace = Race.Beast;
                    subtype = Subtype.Guardian;

                    basePower = 5;
                    baseToughness = 3;
                    baseMovement = 3;

                    orderCost = 3;
                    greyCost = 3;

                    keywordAbilities.Add(KeywordAbility.Flying);

                    addTriggeredAbility(
                        "Whenever an opponent draws a card you may draw a card.",
                        new Effect(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), new DrawCardsDoer(1)),
                        new Foo(),
                        new TypedGameEventFilter<DrawEvent>(e => e.player != controller),
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

                    chaosCost = 2;

                    basePower = 2;
                    baseToughness = 2;
                    baseMovement = 2;

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


                    castEffect = new Effect(new ChooseRule<Card>(c => c.damageTaken > 0 && !c.isHeroic), new ModifyDoer(add(2), never, ModifiableStats.Power, ModifiableStats.Toughness));

                } break;
                #endregion
                #region Taouy Ruins
                case CardTemplate.Taouy_sRuins:
                {
                    cardType = CardType.Relic;
                    rarity = Rarity.Rare;

                    greyCost = 2;

                    baseMovement = 1;

                    addActivatedAbility(
                        String.Format("{0}, {1}: Gain one mana of any colour until end of step.",
                        G.colourlessGlyph(1), G.exhaustGhyph),
                        new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController),
                        new ChooseRule<ManaOrb>(ChooseRule<ManaOrb>.ChooseAt.Resolve)),
                        new GainBonusManaDoer(),
                        new Foo(exhaustThis, manaCostEffect(ManaColour.Colourless)),
                        0,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );

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
                    greyCost = 1;

                    basePower = 1;
                    baseToughness = 2;
                    baseMovement = 3;

                    keywordAbilities.Add(KeywordAbility.Reinforcement);

                    etbLambda(
                        "When Shibby's Saboteur enters the battlefield you may set target non-heroic creature's power to 1.",
                        new Effect(new ChooseRule<Card>(c => !c.isHeroic), new ModifyDoer(setTo(1), never, ModifiableStats.Power)),
                        4, 
                        true
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

                    castDescription = "Heroic creatures you control get +3/+0 until end of turn.";
                    castEffect = new Effect(new CardResolveRule(CardResolveRule.Rule.ResolveControllerCard),
                        new ModifyDoer(add(3), endOfTurn, ModifiableStats.Power));
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
                    castEffect = Effect.summonTokensEffect(CardTemplate.Rock, CardTemplate.Rock);
                } break;
                #endregion
                #region Malificent Spirit
                case CardTemplate.Maleficent_sSpirit:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Common;
                    baseRace = Race.Spirit;

                    deathCost = 2;
                    greyCost = 2;

                    basePower = 2;
                    baseToughness = 3;
                    baseMovement = 2;

                    keywordAbilities.Add(KeywordAbility.Flying);

                    etbLambda(
                        "When Malificent Spirit enters the battlefield you may look at target players hand and select a card from it. The selected card is discarded.",
                        new Effect(
                            new ChooseRule<Card>(
                                new SelectCardRule(PileLocation.Hand, SelectCardRule.Mode.ResolverLooksAtPlayer),
                                new ChooseRule<Player>(),
                                ChooseRule<Card>.ChooseAt.Resolve,
                                c => true),
                            new MoveToPileDoer(PileLocation.Graveyard))
                        );
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

                    lifeCost = 1;
                    chaosCost = 1;
                    greyCost = 2;

                    basePower = 2;
                    baseToughness = 3;
                    baseMovement = 3;

                    keywordAbilities.Add(KeywordAbility.Flying);

                    diesLambda(
                        "Whenever Flameheart Pheonix enters the graveyard from the battlefield you may return it to your hand.",
                        new Effect(new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                            new MoveToPileDoer(PileLocation.Hand)),
                        0,
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

                    addActivatedAbility(
                        String.Format("{1}, {0}: Deal 1 damage to target creature within 1 tile.", G.exhaustGhyph, G.colouredGlyph(ManaColour.Might)),
                        zepLambda(1),
                        new Foo(exhaustThis, manaCostEffect(ManaColour.Might)),
                        1,
                        PileLocation.Field,
                        CastSpeed.Interrupt
                        );

                    flavourText =
                        "\"What he lacks in brains he makes up for in... Well you get my point.\" -- Shibby Shtank";
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

                    diesLambda(
                        "Whenever Gryphon Rider enters the graveyard from the battlefield summon a 2/2 Gryphon token with flying.",
                        Effect.summonTokensEffect(CardTemplate.Gryphon),
                        2);
                } break;
                #endregion
                #region Ravaging Greed
                case CardTemplate.Ravaging_sGreed:
                {
                    cardType = CardType.Channel;
                    rarity = Rarity.Uncommon;

                    chaosCost = 1;

                    castDescription = "Discard two cards then draw two cards.";
                    castEffect =
                        new Effect(
                            new ChooseRule<Card>(
                                new SelectCardRule(PileLocation.Hand, SelectCardRule.Mode.PlayerLooksAtPlayer),
                                new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController),
                                ChooseRule<Card>.ChooseAt.Resolve,
                                c => true, 
                                2, 
                                false),
                            new MoveToPileDoer(PileLocation.Graveyard));
                    additionalCastEffects.Add(
                        new Effect(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), new DrawCardsDoer(2)));

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
                    castEffect= zepLambda(1);
                    additionalCastEffects.Add(new Effect(
                        new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), new DrawCardsDoer(1)));
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
                        "",
                        v => v + (controller.field.Count(c => c != this && c.cardType == CardType.Creature && !c.isHeroic) == 0 ? 2 : 0),
                        ModifiableStats.Power,
                        c => c == this,
                        PileLocation.Field
                        ));

                    auras.Add(new Aura(
                        "This creature has gets +2/+2 as long as its controller controls no other non-heroic creatures.",
                        v => v + (controller.field.Count(c => c != this && c.cardType == CardType.Creature && !c.isHeroic) == 0 ? 2 : 0),
                        ModifiableStats.Toughness,
                        c => c == this,
                        PileLocation.Field
                        ));
                    } break;
                #endregion
                #region AOE_EXHAUST
                case CardTemplate.AOE_sEXHAUST:
                    {
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
                            CastSpeed.Interrupt);
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
                castSpeed = hasAbility(KeywordAbility.Reinforcement) ? CastSpeed.Interrupt : CastSpeed.Channel;
                castRange = 2;
                castEffect = new Effect(new TargetRuleSet(
                    new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                    new ChooseRule<Tile>(ChooseRule<Tile>.ChooseAt.Resolve,t => t.passable && !t.isEdgy)),
                    new SummonToTileDoer());
            }
            else throw new Exception();
            
            if (castEffect == null) throw new Exception("these don't show up anyway");
            List<Effect> es = new List<Effect>();

            es.Add(castEffect);
            es.AddRange(additionalCastEffects);

            additionalCastCosts.Add(manaCostEffect(castManaCost));

            castAbility = new ActivatedAbility(this, PileLocation.Hand, castRange, new Foo(additionalCastCosts.ToArray()), castSpeed, castDescription, es.ToArray());
            abilities.Add(castAbility);

            this.owner = owner;
            controller = owner;

            name = G.replaceUnderscoresAndShit(ct.ToString());

            eventHandler = generateDefaultEventHandlers();
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



    }
}
