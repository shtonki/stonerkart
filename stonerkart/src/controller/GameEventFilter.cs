using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    abstract class GameEventFilter
    {
        public abstract bool filter(GameEvent e);

        public static GameEventFilter startOfOwnersTurn(Card c)
        {
            return new TypedGameEventFilter<StartOfStepEvent>(
                e => e.activePlayer == c.controller && e.step == Steps.Untap);
        }

        public static GameEventFilter never()
        {
            return new StaticGameEventFilter(() => false);
        }
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

        public override bool filter(GameEvent e)
        {
            if (e is T)
            {
                return f((T)e);
            }
            return false;
        }
    }

    class StaticGameEventFilter : GameEventFilter
    {
        private Func<bool> f;

        public StaticGameEventFilter(Func<bool> f)
        {
            this.f = f;
        }

        public override bool filter(GameEvent e)
        {
            return f();
        }
    }
}
