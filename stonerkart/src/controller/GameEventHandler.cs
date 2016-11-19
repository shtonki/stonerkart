using System;

namespace stonerkart
{
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

    abstract class GameEventHandler
    {
        public abstract void handle(GameEvent ge);
    }
}