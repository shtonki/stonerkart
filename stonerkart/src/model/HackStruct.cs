using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace stonerkart
{
    struct HackStruct
    {
        //game stuff
        public IEnumerable<Player> players { get; }
        public Player hero { get; }
        public Player castingPlayer { get; }
        public bool heroIsCasting => hero == castingPlayer;
        public Func<CardTemplate, Player, Card> createToken;

        public Tile castFrom => !resolveAbility.isCastAbility ? (resolveAbility.card.tile ?? resolveAbility.card.lastSeenAt) : hero.heroCard.tile;
        public int castRange => resolveAbility.castRange;

        public Func<Card, int> ordC { get; }
        public Func<Player, int> ordP { get; }
        public Func<Tile, int> ordT { get; }
        public Func <int, Card> Cord { get; }
        public Func <int, Player> Pord { get; }
        public Func <int, Tile> Tord { get; }

        //resolve shit
        public Player resolveController => resolveCard.controller;
        public bool heroIsResolver => hero == resolveController;
        public Card resolveCard => resolveAbility.card;
        public Ability resolveAbility { get; }
        public IEnumerable<Card> cards { get; }
        public GameEvent triggeringEvent { get; set; }

        public TargetMatrix previousTargets { get; set; }
        public TargetColumn previousColumn { get; set; }
        public IEnumerable<Tile> tilesInRange => castFrom.withinDistance(castRange);
        

        //network stuff
        public Action<int[]> sendChoices { get; }
        public Func<int[]> receiveChoices { get; }

        //ui stuff
        public Func<Stuff> getStuff { get; }
        public Func<Player, Func<ManaOrb, bool>, ManaColour> selectColour { get; }
        private Action<string, ButtonOption[]> _setPrompt { get; }

        public Action clearHighlights { get; }
        public Action<IEnumerable<Tile>, Color> highlight { get; }

        private Func<IEnumerable<Card>, bool, int, Func<Card, bool>, IEnumerable<Card>> selectCardEx;


        public HackStruct(GameState g, Game gg) : this()
        {
            ordP = g.ord;
            ordC = g.ord;
            ordT = g.ord;
            Cord = g.cardFromOrd;
            Pord = g.playerFromOrd;
            Tord = g.tileFromOrd;
            cards = g.cards;
            players = g.allPlayers;

            sendChoices = gg.sendChoices;
            receiveChoices = gg.receiveChoices;
            selectCardEx = gg.selectCardFromCards;
            getStuff = gg.waitForAnything;
            hero = g.hero;
            _setPrompt = gg.gameController.setPrompt;
            clearHighlights = () => gg.gameController.clearHighlights(true);
            highlight = gg.gameController.highlight;
            createToken = gg.createToken;
            selectColour = gg.selectManaColour;
        }

        public HackStruct(GameState g, Game gg, Player p) : this(g, gg)
        {
            castingPlayer = p;
        }

        public HackStruct(GameState g, Game gg, Ability a) : this(g, gg)
        {
            resolveAbility = a;
        }

        public HackStruct(GameState g, Game gg, Ability a, Player p) : this(g, gg)
        {
            resolveAbility = a;
            castingPlayer = p;
        }

        public void setPrompt(string s)
        {
            _setPrompt(s, new ButtonOption[] {});
        }

        public void setPrompt(string s, params ButtonOption[] os)
        {
            _setPrompt(s, os);
        }

        public T waitForStuff<T>(Func<T, bool> f) where T : Stuff
        {
            while (true)
            {
                Stuff s = getStuff();
                if (s is T)
                {
                    T t = (T)s;
                    if (f(t)) return t;
                }
            }
        }

        public Card selectCardHalfSynchronized(IEnumerable<Card> cs, Player chooser, Func<Card, bool> filter, bool cancelable)
        {
            var v = selectCardsSynchronized(cs, chooser, 1, filter, cancelable).ToArray();
            if (v.Length == 0) return null;
            return v[0];
        }

        public Card selectCardUnsynchronized(IEnumerable<Card> cs, Player chooser,Func<Card, bool> filter, bool cancelable)
        {
            var v =  selectCardEx(cs, cancelable, 1, filter);
            if (v == null || v.Count() != 1) throw new Exception();
            return v.ElementAt(0);
        }

        public IEnumerable<Card> selectCardsSynchronized(IEnumerable<Card> cs, Player chooser, int cardCount, Func<Card, bool> filter, bool cancelable)
        {
            IEnumerable<Card> c;
            if (hero == chooser)
            {
                c = selectCardEx(cs, cancelable, cardCount, filter);
                int[] ss = c.Select(ordC).ToArray();
                sendChoices(ss);
            }
            else
            {
                setPrompt("Opponent is choosing a card.");
                int[] v = receiveChoices();
                c = v.Select(Cord);
            }
            return c;
        }
    }
}