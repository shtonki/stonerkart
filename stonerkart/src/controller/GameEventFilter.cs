using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    interface GameEventFilter
    {
        bool filter(GameEvent e);
    }

    class TypedGameEventFilter<T> : GameEventFilter where T : GameEvent
    {
        private Func<T, bool> f;

        public TypedGameEventFilter() : this((_) => true)
        {
        }

        public TypedGameEventFilter(Func<T, bool> f)
        {
            this.f = f;
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
