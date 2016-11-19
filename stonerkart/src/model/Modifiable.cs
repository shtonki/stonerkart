using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    abstract class Modifiable
    {
        public abstract void check(GameEvent e);
        public abstract object getValue();

        public static int intAdd(int i1, int i2)
        {
            return i1 + i2;
        }

        public static int intSet(int i1, int i2)
        {
            return i2;
        }

        public static TypedGameEventFilter<StartOfStepEvent> startOfEndStep = new TypedGameEventFilter<StartOfStepEvent>(e => e.step == Steps.End); 
        public static TypedGameEventFilter<GameEvent> never = new TypedGameEventFilter<GameEvent>(e => false);
    }

    class Modifiable<T> : Modifiable
    {
        private T baseValue;
        private T currentValue;
        private List<Tuple<T, Func<T, T, T>, GameEventFilter>> modifiers;
        private bool dirty;

        public Modifiable(T baseValue)
        {
            setBaseValue(baseValue);
            modifiers = new List<Tuple<T, Func<T, T, T>, GameEventFilter>>();
        }

        public static implicit operator T(Modifiable<T> m)
        {
            return (T)m.getValue();
        }

        public void setBaseValue(T v)
        {
            baseValue = v;
            dirty = true;
        }

        public void modify(T o, Func<T, T, T> f, GameEventFilter filter)
        {
            modifiers.Add(new Tuple<T, Func<T, T, T>, GameEventFilter>(o, f, filter));
            dirty = true;
        }

        public override object getValue()
        {
            if (dirty) reherp();
            return currentValue;
        }

        public override void check(GameEvent e)
        {
            List<Tuple<T, Func<T, T, T>, GameEventFilter>> l = new List<Tuple<T, Func<T, T, T>, GameEventFilter>>(modifiers.Count);
            foreach (var v in modifiers)
            {
                if (v.Item3.filter(e))
                {
                    dirty = true;
                }
                else
                {
                    l.Add(v);
                }
            }
            modifiers = l;
        }

        private void reherp()
        {
            currentValue = modifiers.Aggregate(baseValue, (current, m) => m.Item2(current, m.Item1));
            dirty = false;
        }

        public override string ToString()
        {
            return getValue().ToString();
        }
    }
}
