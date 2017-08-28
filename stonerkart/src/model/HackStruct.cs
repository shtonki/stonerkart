using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace stonerkart
{
    struct HackStruct
    {
        public Player castingPlayer { get; }
        public Player hero => gameState.hero;
        public bool heroIsCasting => hero == castingPlayer;
        public Tile castFrom => !resolveAbility.isCastAbility ? (resolveAbility.card.tile ?? resolveAbility.card.lastSeenAt) : hero.heroCard.tile;
        public int castRange => resolveAbility.castRange;
        public Player resolveController => resolveCard.controller;
        public bool heroIsResolver => hero == resolveController;
        public Card resolveCard => resolveAbility.card;
        public Ability resolveAbility { get; }
        public TargetSet previousTargets { get; set; }
        public GameEvent triggeringEvent { get; set; }
        public IEnumerable<Tile> tilesInRange => castFrom.withinDistance(castRange);

        public IEnumerable<Card> cards => gameState.cards;
        public IEnumerable<Player> players => gameState.players;

        public GameState gameState => game.gameState;
        public Game game { get; }

        public HackStruct(Game g) : this()
        {
            game = g;
        }

        public HackStruct(Game g, Player p) : this(g)
        {
            castingPlayer = p;
        }

        public HackStruct(Game g, Ability resolvingAbility) : this(g)
        {
            resolveAbility = resolvingAbility;
        }

        public HackStruct(Game g, Ability resolvingAbility, Player castingPlayer) : this(g)
        {
            resolveAbility = resolvingAbility;
            this.castingPlayer = castingPlayer;
        }
        
    }
}