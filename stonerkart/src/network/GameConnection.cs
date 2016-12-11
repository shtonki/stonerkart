using System;
using System.Collections.Generic;

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
            else
            {
                throw new NotImplementedException();
            }

            return (T)r;
        }
    }

    interface GameConnection
    {
        void sendAction(GameAction g);
        T receiveAction<T>() where T : GameAction;
    }
}