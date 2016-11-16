using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    abstract class GameEvent
    {
    }

    class DrawEvent : GameEvent
    {
        public Player player { get; }
        public int cards { get; }

        public DrawEvent(Player player, int cards)
        {
            this.player = player;
            this.cards = cards;
        }
    }

    abstract class GameEventHandler
    {
        public abstract void handle(GameEvent ge);
    }

    class GameEventHandler<T> : GameEventHandler where T : GameEvent
    {
        public GameEventFilter filter { get; }
        public Action<T> act { get; }

        public GameEventHandler(Action<T> act) : this(new TypedGameEventFilter<T>(), act)
        {

        }

        public GameEventHandler(GameEventFilter filter, Action<T> act)
        {
            this.filter = filter;
            this.act = act;
        }

        public override void handle(GameEvent ge)
        {
            if (filter.filter(ge)) act((T)ge);
        }
    }

    interface GameEventFilter
    {
        bool filter(GameEvent e);
    }

    class TypedGameEventFilter<T> : GameEventFilter where T : GameEvent
    {
        private Func<T, bool> f;

        public TypedGameEventFilter()
        {
            f = (_) => true;
        }

        public bool filter(GameEvent e)
        {
            if (e is T)
            {
                return f((T)e);
            }
            return false;
        }
    }
}
