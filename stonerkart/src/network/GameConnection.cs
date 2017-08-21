using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace stonerkart
{
    class MultiplayerConnection : GameConnection
    {
        private GameState game;

        public MultiplayerConnection(GameState g, NewGameStruct ngs)
        {
            game = g;
        }

        public T receiveAction<T>() where T : GameAction
        {
            GameAction r;
            string s = dequeueGameMessage();
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
            else if (typeof(T) == typeof(TriggeredAbilitiesGluer))
            {
                r = new TriggeredAbilitiesGluer(game, s);
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
            Network.sendGameMessage(game, s);
        }

        public Deck[] deckify(Deck myDeck, int myIndex)
        {
            Deck[] decks = new Deck[2]; //todo this is fucked
            decks[myIndex] = myDeck;

            List<int> send = new List<int>();
            send.Add(myIndex);
            send.Add((int)myDeck.hero);
            send.AddRange(myDeck.templates.Select(v => (int)v));
            var mc = new ChoiceSelection(send);
            sendAction(mc);

            for (int i = 1; i < decks.Length; i++)
            {
                string s = dequeueGameMessage();
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


        private string qdstring;
        private ManualResetEventSlim mre = new ManualResetEventSlim();
        public void enqueueGameMessage(string s)
        {
            if (qdstring != null) throw new Exception();
            qdstring = s;
            mre.Set();
        }

        private string dequeueGameMessage()
        {
            mre.Wait();
            string r = qdstring;
            qdstring = null;
            mre.Reset();
            return r;
        }

        public void surrender(GameEndStateReason reason)
        {

            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");/*
            Network.surrender(game.gameid, reason);*/
        }
    }

    class DummyConnection : GameConnection
    {
        public void sendAction(GameAction g)
        {
            //Console.WriteLine(g.GetType());
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
            else if (typeof (T) == typeof(TriggeredAbilitiesGluer))
            {
                r = new TriggeredAbilitiesGluer();
            }
            else
            {
                throw new NotImplementedException();
            }

            return (T)r;
        }

        public Deck[] deckify(Deck myDeck, int myIndex)
        {
            Deck fucked = new Deck(CardTemplate.Bhewas, new []
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

        public void enqueueGameMessage(string s)
        {
            throw new Exception();
        }

        public void surrender(GameEndStateReason rn)
        {

            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");/*
            ScreenController.transitionToMainMenu();
            */
        }
    }

    interface GameConnection
    {
        void sendAction(GameAction g);
        T receiveAction<T>() where T : GameAction;
        void enqueueGameMessage(string s);
        Deck[] deckify(Deck myDeck, int myIndex);
        void surrender(GameEndStateReason r);
    }
}