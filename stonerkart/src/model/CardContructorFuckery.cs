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

            #region oophell

            switch (ct)
            {
                #region Illegal Goblin Laboratory
                case CardTemplate.Illegal_sGoblin_sLaboratory:
                {
                    cardType = CardType.Relic;

                    chaosCost = 2;
                    greyCost = 1;

                    addTriggeredAbility(
                        "At the end of your turn deal 1 damage to every enemy player.",
                        new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard), new CardResolveRule(CardResolveRule.Rule.VillainHeroes)),
                        new ZepperDoer(1),
                        new Foo(),
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
                        new Foo(),
                        new TypedGameEventFilter<MoveToPileEvent>(moveEvent => moveEvent.card == this && location.pile == PileLocation.Field),
                        0, 
                        PileLocation.Field,
                        TriggeredAbility.Timing.Post
                        );

                } break;
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
                case CardTemplate.Temple_sHealer:
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
                        new Foo(),
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
                case CardTemplate.Nature_sHeroman:
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
                case CardTemplate.Risen_sAbberation:
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
                        new ToOwnersDoer(PileLocation.Hand));
                    castDescription = "Return target non-heroic creature to its owner's hand.";
                } break;
                #endregion
                #region Rockhand Ogre
                case CardTemplate.Rockhand_sOgre:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Uncommon;

                    baseMovement = 2;
                    basePower = 2;
                    baseToughness = 2;
                    mightCost = 1;
                } break;
                #endregion
                #region Bear Cavalary
                case CardTemplate.Bear_sCavalary:
                {
                    cardType = CardType.Creature;
                    rarity = Rarity.Common;

                    baseMovement = 3;
                    basePower = 3;
                    baseToughness = 3;
                    mightCost = 2;
                } break;
                #endregion
                #region Cleansing Fire
                case CardTemplate.Cleansing_sFire:
                {
                    cardType = CardType.Instant;
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


                } break;
                #endregion
                #region Goblin Grenade
                case CardTemplate.Goblin_sGrenade:
                {
                    cardType = CardType.Instant;

                    chaosCost = 2;
                    greyCost = 1;

                    castRange = 5;
                    castEffect = new Effect(new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard), new AoeRule(t => true, 2, c => true)),
                    new ZepperDoer(1));
                    castDescription = "Deal 1 damage to all creatures within 2 tiles of target tile.";

                } break;
                #endregion
                #region Teleport
                case CardTemplate.Teleport:
                {
                    cardType = CardType.Sorcery;

                    orderCost = 2;
                    castRange = 6;
                    castEffect = new Effect(
                            new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveControllerCard),
                                new PryTileRule(f => f.passable)), new MoveToTileDoer());
                    castDescription = "Move your hero to target tile.";
                } break;
                #endregion
                #region One With Nature
                case CardTemplate.One_sWith_sNature:
                {
                    cardType = CardType.Sorcery;

                    natureCost = 1;

                    castEffect = new Effect(new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController)), 
                        new GainBonusManaDoer(ManaColour.Nature, ManaColour.Nature, ManaColour.Nature));
                    castDescription = String.Format("You gain {0}{0}{0} until the end of the step.",
                        G.coloured(ManaColour.Nature));
                } break;
                #endregion
                #region Graverobber Syrdin
                case CardTemplate.Graverobber_sSyrdin:
                {
                    cardType = CardType.Creature;

                    baseMovement = 2;
                    basePower = 3;
                    baseToughness = 3;

                    deathCost = 1;
                    lifeCost = 1;
                    natureCost = 1;

                    addTriggeredAbility(
                        "Whenever this creature enters the battlefield under your control, you may return a card from your graveyard to your hand.",
                        new TargetRuleSet(new SelectCardRule(PileLocation.Graveyard, new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController))),
                        new ToOwnersDoer(PileLocation.Hand), 
                        new Foo(),
                        new TypedGameEventFilter<MoveToPileEvent>(moveEvent => moveEvent.card == this && moveEvent.to.location.pile == PileLocation.Field),
                        0,
                        PileLocation.Field,
                        TriggeredAbility.Timing.Post
                        );

                    } break;
                #endregion
                #region Alter Fate
                case CardTemplate.Alter_sFate:
                { 
                    cardType = CardType.Instant;

                    orderCost = 1;

                    castEffect =
                        new Effect(
                            new TargetRuleSet(new SelectCardRule(PileLocation.Deck,
                                new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController))),
                            new ToOwnersDoer(PileLocation.Deck));
                    castDescription =
                        "Search your deck for a card. Shuffle your deck then put the selected card on top.";
                } break;
                #endregion

                case CardTemplate.Damage_sWard:
                {
                    cardType = CardType.Instant;

                    lifeCost = 1;
                    castRange = 4;
                    castEffect = new Effect(
                        new TargetRuleSet(new PryCardRule()),
                        new ModifyDoer(ModifiableStats.Toughness, 3, LL.add, LL.never));
                    castDescription = "Target creature gains 3 toughness.";

                } break;


                case CardTemplate.Survival_sInstincts:
                {
                    cardType = CardType.Instant;

                    castRange = 3;
                    natureCost = 2;
                    castEffect = new Effect(new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard), new PryCardRule()), 
                        new ZepperDoer(-2));
                    additionalCastEffects.Add(new Effect(new CopyPreviousRule<Card>(1), 
                        new ModifyDoer(ModifiableStats.Power, 2, LL.add, LL.endOfTurn)));
                    castDescription =
                        "Target creature is healed for 2 and gains 2 power until the end of this turn.";
                } break;

                case CardTemplate.missingno:
                {
                    
                } break;

                default:
                {
                    throw new Exception("missing cardtemplate in switch");
                }
            }


            #endregion

            int[] x = new int[ManaSet.size];
            x[(int)ManaColour.Chaos] = chaosCost;
            x[(int)ManaColour.Colourless] = greyCost;
            x[(int)ManaColour.Death] = deathCost;
            x[(int)ManaColour.Life] = lifeCost;
            x[(int)ManaColour.Might] = mightCost;
            x[(int)ManaColour.Nature] = natureCost;
            x[(int)ManaColour.Order] = orderCost;
            castManaCost = new ManaSet(x);

            Power = new Modifiable(basePower);
            Toughness = new Modifiable(baseToughness);
            Movement = new Modifiable(baseMovement);

            modifiables = new Modifiable[]
            {
                Power,
                Toughness,
                Movement,
            };


            if (cardType == CardType.Instant)
            {
                castSpeed = CastSpeed.Instant;
            }
            else if (cardType == CardType.Sorcery)
            {
                castSpeed = CastSpeed.Slow;
            }
            else if (cardType == CardType.Creature || cardType == CardType.Relic)
            {
                castSpeed = CastSpeed.Slow;
                castRange = 2;
                castEffect = new Effect(new TargetRuleSet(
                    new CardResolveRule(CardResolveRule.Rule.ResolveCard),
                    new PryTileRule(t => t.card == null)),
                    new MoveToTileDoer());
            }
            else throw new Exception();
            
            if (castEffect == null) throw new Exception("these don't show up anyway");
            List<Effect> es = new List<Effect>();

            es.Add(castEffect);
            es.AddRange(additionalCastEffects);

            additionalCastCosts.Add(new Effect(new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), new ManaCostRule(castManaCost)), new PayManaDoer()));

            castAbility = new ActivatedAbility(PileLocation.Hand, castRange, new Foo(additionalCastCosts.ToArray()), castSpeed, castDescription, es.ToArray());
            abilities.Add(castAbility);

            this.owner = owner;
            controller = owner;

            name = ct.ToString().Replace("_a", "'").Replace("_s", " ");

            eventHandler = generatedlft();
        }

        private void addTriggeredAbility(string description, TargetRuleSet trs, Doer doer, Foo foo, GameEventFilter filter, int castRange, PileLocation activeIn, TriggeredAbility.Timing timing = TriggeredAbility.Timing.Pre)
        {
            Effect e = new Effect(trs, doer);
            TriggeredAbility ta = new TriggeredAbility(this, activeIn, new[] { e }, castRange, foo, filter, timing, description);
            abilities.Add(ta);
        }

        private void addActivatedAbility(string description, TargetRuleSet trs, Doer doer, Foo foo, int castRange, PileLocation activeIn, CastSpeed castSpeed)
        {
            Effect e = new Effect(trs, doer);
            ActivatedAbility ta = new ActivatedAbility(activeIn, castRange, foo, castSpeed, description, e);
            abilities.Add(ta);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] //yagni said no one ever
        private static Foo fooFromManaCost(params ManaColour[] cs)
        {
            return new Foo(new Effect(new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController),
                new ManaCostRule(cs)), new PayManaDoer()));
        }

    }
}
