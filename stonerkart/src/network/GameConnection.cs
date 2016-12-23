using System;
using System.Collections.Generic;
using System.Linq;

namespace stonerkart
{
    class MultiplayerConnection : GameConnection
    {
        private Game game;
        private string[] otherPlayers;


        public MultiplayerConnection(Game g, NewGameStruct ngs)
        {
            game = g;

            List<string> ps = new List<string>();
            for (int i = 0; i < ngs.playerNames.Length; i++)
            {
                if (i == ngs.heroIndex) continue;
                ps.Add(ngs.playerNames[i]);
            }
            otherPlayers = ps.ToArray();
        }

        public T receiveAction<T>() where T : GameAction
        {
            GameAction r;
            string s = Network.dequeueGameMessage();
            if (typeof(T) == typeof (ManaOrbSelection))
            {
                r = new ManaOrbSelection(game, s);
            }
            else if (typeof(T) == typeof(MoveSelection))
            {
                r = new MoveSelection(game, s);
            }
            else if (typeof (T) == typeof (CastSelection))
            {
                r = new CastSelection(game, s);
            }
            else if (typeof (T) == typeof (ChoiceSelection))
            {
                r = new ChoiceSelection(game, s);
            }
            else
            {
                throw new NotImplementedException();
            }
            return (T)r;
        }

        public void sendAction(GameAction g)
        {
            string s = g.toString(game);
            Network.sendGameMessage(s, otherPlayers);
        }

        public Deck[] deckify(Deck myDeck, int myIndex)
        {
            Deck[] decks = new Deck[otherPlayers.Length + 1];
            decks[myIndex] = myDeck;

            List<int> send = new List<int>();
            send.Add(myIndex);
            send.Add((int)myDeck.hero);
            send.AddRange(myDeck.templates.Select(v => (int)v));
            var mc = new ChoiceSelection(send);
            sendAction(mc);

            for (int i = 1; i < decks.Length; i++)
            {
                string s = Network.dequeueGameMessage();
                var c = new ChoiceSelection(null, s);

                int pix = c.choices[0];
                CardTemplate hero = (CardTemplate)c.choices[1];

                List<CardTemplate> ts = new List<CardTemplate>();
                for (int j = 2; j < c.choices.Length; j++)
                {
                    ts.Add((CardTemplate)c.choices[j]);
                }

                decks[pix] = new Deck(hero, ts.ToArray());
            }
            
            return decks;
        }
    }

    class DummyConnection : GameConnection
    {
        public void sendAction(GameAction g)
        {
            Console.WriteLine(g.GetType());
        }

        public T receiveAction<T>() where T : GameAction
        {
            GameAction r;
            if (typeof(T) == typeof (ManaOrbSelection))
            {
                r =  new ManaOrbSelection(ManaColour.Life);
            }
            else if (typeof(T) == typeof(MoveSelection))
            {
                r = new MoveSelection(new List<Tuple<Card, Path>>());
            }
            else if (typeof (T) == typeof (CastSelection))
            {
                r = new CastSelection(null);
            }
            else if (typeof (T) == typeof (ChoiceSelection))
            {
                r = new ChoiceSelection();
            }
            else
            {
                throw new NotImplementedException();
            }

            return (T)r;
        }

        public Deck[] deckify(Deck myDeck, int myIndex)
        {
            Deck fucked = new Deck(CardTemplate.Belwas, new []
            {
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
                CardTemplate.Cantrip, 
            });

            Deck[] r = new Deck[2];
            r[myIndex] = myDeck;
            r[(myIndex + 1)%2] = fucked;

            return r;
        }
    }

    interface GameConnection
    {
        void sendAction(GameAction g);
        T receiveAction<T>() where T : GameAction;
        Deck[] deckify(Deck myDeck, int myIndex);
    }
}