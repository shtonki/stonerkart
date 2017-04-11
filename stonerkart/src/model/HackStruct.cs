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

        public Tile castFrom => !resolveAbility.isCastAbility ? resolveAbility.card.tile : hero.heroCard.tile;
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
        public Func<Player, ManaColour> selectColour { get; }
        private Action<string, ButtonOption[]> _setPrompt { get; }

        public Action clearHighlights { get; }
        public Action<IEnumerable<Tile>, Color> highlight { get; }

        private Func<IEnumerable<Card>, bool, int, Func<Card, bool>, IEnumerable<Card>> selectCardEx;


        public HackStruct(Game g) : this()
        {
            ordP = g.ord;
            ordC = g.ord;
            ordT = g.ord;
            Cord = g.cardFromOrd;
            Pord = g.playerFromOrd;
            Tord = g.tileFromOrd;
            cards = g.ca;
            sendChoices = g.sendChoices;
            receiveChoices = g.receiveChoices;
            selectCardEx = g.selectCardFromCards;
            getStuff = g.waitForAnything;
            hero = g.hero;
            _setPrompt = g.gameController.setPrompt;
            clearHighlights = () => g.gameController.clearHighlights(true);
            highlight = g.gameController.highlight;
            createToken = g.createToken;
            players = g.allPlayers;
            selectColour = g.selectManaColour;
        }

        public HackStruct(Game g, Player p) : this(g)
        {
            castingPlayer = p;
        }

        public HackStruct(Game g, Ability a) : this(g)
        {
            resolveAbility = a;
        }

        public HackStruct(Game g, Ability a, Player p) : this(g)
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