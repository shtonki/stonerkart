using System;
using System.Collections.Generic;

namespace stonerkart
{
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