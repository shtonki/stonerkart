using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace stonerkart
{
    class MultiplayerConnection : GameConnection
    {
        public MultiplayerConnection(Game g) : base(g)
        {
        }

        public override int[] receiveChoices()
        {
            string s = dequeueGameMessage();
            if (s == "") return new int[0];
            var ss = s.Split(intsplitter);
            return ss.Select(Int32.Parse).ToArray();
        }

        public override void sendChoices(int [] choices)
        {
            if (closed) return;
            StringBuilder sb = new StringBuilder();
            foreach (var choice in choices) sb.Append(choice.ToString() + intsplitter);
            if (sb.Length > 0) sb.Length--;

            string s = sb.ToString();
            Network.sendGameMessage(game.gameid, s);
        }

        public override Deck[] deckify(Deck myDeck, int myIndex)
        {
            Deck[] decks = new Deck[2]; //todo this is fucked
            decks[myIndex] = myDeck;

            List<int> send = new List<int>();
            send.Add(myIndex);
            send.Add((int)myDeck.hero);
            send.AddRange(myDeck.templates.Select(v => (int)v));
            sendChoices(send.ToArray());

            for (int i = 1; i < decks.Length; i++)
            {
                int[] fuckme = receiveChoices();

                int playerIndex = fuckme[0];
                CardTemplate hero = (CardTemplate)fuckme[1];

                List<CardTemplate> ts = new List<CardTemplate>();
                for (int j = 2; j < fuckme.Length; j++)
                {
                    ts.Add((CardTemplate)fuckme[j]);
                }

                decks[playerIndex] = new Deck(hero, ts.ToArray());
            }
            
            return decks;
        }

        private bool closed;

        public override void surrender(GameEndStateReason reason)
        {
            Network.surrender(game.gameid, reason);
            closed = true;
        }

        private char intsplitter = ',';

        private string qdstring;
        private ManualResetEventSlim mre = new ManualResetEventSlim();
        public override void enqueueGameMessage(string s)
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
    }

    class DummyConnection : GameConnection
    {
        public DummyConnection(Game g) : base(g)
        {
        }

        public override void sendChoices(int[] choices)
        {
            
        }

        private bool sendno = true;

        public override int[] receiveChoices()
        {
            if (sendno)
            {
                sendno = false;
                return new [] { (int)ButtonOption.No };
            }
            return new int[0];
        }

        public override Deck[] deckify(Deck myDeck, int myIndex)
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

        public override void enqueueGameMessage(string s)
        {
            throw new Exception();
        }

        public override void surrender(GameEndStateReason rn)
        {
            GUI.transitionToScreen(new PostGameScreen(game));
        }
    }

    abstract class GameConnection
    {
        public abstract void sendChoices(int[] choices);
        public abstract int[] receiveChoices();
        public abstract void enqueueGameMessage(string s);
        public abstract Deck[] deckify(Deck myDeck, int myIndex);
        public abstract void surrender(GameEndStateReason r);


        protected Game game;

        protected GameConnection(Game g)
        {
            game = g;
        }

        public void sendChoice(int? choice)
        {
            if (choice.HasValue)
            {
                sendChoices(new[] {choice.Value});
            }
            else
            {
                sendChoices(new int[0]);
            }
        }

        public int? receiveChoice()
        {
            var received = receiveChoices();
            if (received.Length == 0) return null;
            if (received.Length == 1) return received[0];
            throw new Exception();
        }
    }
}