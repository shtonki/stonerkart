using System;
using System.Collections.Generic;

namespace stonerkart
{
    class TypedGameEventHandler<T> : GameEventHandler where T : GameEvent
    {
        public GameEventFilter filter { get; }
        public Action<T> act { get; }

        public TypedGameEventHandler(Action<T> act) : this(new TypedGameEventFilter<T>(), act)
        {

        }

        public TypedGameEventHandler(GameEventFilter filter, Action<T> act)
        {
            this.filter = filter;
            this.act = act;
        }

        public void handle(GameEvent ge)
        {
            if (filter.filter(ge)) act((T)ge);
        }
    }

    interface GameEventHandler
    {
        void handle(GameEvent ge);
    }

    class GameEventHandlerBuckets : GameEventHandler
    {
        private Dictionary<Type, GameEventHandler> handlers = new Dictionary<Type, GameEventHandler>();

        public GameEventHandlerBuckets()
        {
        }

        public void add<T>(TypedGameEventHandler<T> h) where T : GameEvent
        {
            Type t = typeof(T);
            if (handlers.ContainsKey(t)) throw new Exception();
            handlers[t] = h;
        }

        public void handle(GameEvent ge)
        {
            Type t = ge.GetType();
            if (!handlers.ContainsKey(t)) return;
            handlers[t].handle(ge);
        }
    }
}