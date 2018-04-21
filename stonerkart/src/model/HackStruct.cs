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

        public Tile castFrom => !resolveAbility.IsCastAbility
                    ? (resolveAbility.Card.tile ?? resolveAbility.Card.lastSeenAt)
                    : resolveCard.Controller.heroCard.tile;
        public int castRange => resolveAbility.CastRange;
        public Player resolveController => resolveCard.Controller;
        public bool heroIsResolver => hero == resolveController;
        public Card resolveCard => resolveAbility.Card;
        public Ability resolveAbility { get; }
        public TargetVector[] previousTargets { get; set; }
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