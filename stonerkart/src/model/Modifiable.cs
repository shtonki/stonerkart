using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    interface Modifiable
    {
        void check(GameEvent e);
        object getValue();
    }

    struct ModifiableStruct<T>
    {
        public T modifier { get; }
        public Func<T, T, T> modifyFunction { get; }
        public GameEventFilter unmodifyCondition { get; }

        public ModifiableStruct(T modifier, Func<T, T, T> modifyFunction, GameEventFilter unmodifyCondition)
        {
            this.modifier = modifier;
            this.modifyFunction = modifyFunction;
            this.unmodifyCondition = unmodifyCondition;
        }
    }

    static class ModifiableSchmoo
    {
        public static int intAdd(int i1, int i2)
        {
            return i1 + i2;
        }

        public static int intSet(int i1, int i2)
        {
            return i2;
        }

        public static TypedGameEventFilter<StartOfStepEvent> startOfEndStep = new TypedGameEventFilter<StartOfStepEvent>(e => e.step == Steps.End);
        public static TypedGameEventFilter<GameEvent>        never = new TypedGameEventFilter<GameEvent>(e => false);

        public static TypedGameEventFilter<StartOfStepEvent> startOfOwnersTurn(Card card)
        {
            return new TypedGameEventFilter<StartOfStepEvent>(evnt => evnt.activePlayer == card.owner && evnt.step == Steps.Untap);
        }
    }

    class Modifiable<T> : Observable<int>, Modifiable
    {
        private T baseValue;
        private T currentValue;
        private List<ModifiableStruct<T>> modifiers;

        public Modifiable(T baseValue)
        {
            setBaseValue(baseValue);
            modifiers = new List<ModifiableStruct<T>>();
            reherp();
        }

        public static implicit operator T(Modifiable<T> m)
        {
            return (T)m.getValue();
        }

        public void setBaseValue(T v)
        {
            baseValue = v;
        }

        public void modify(T o, Func<T, T, T> f, GameEventFilter filter)
        {
            modifiers.Add(new ModifiableStruct<T>(o, f, filter));
            reherp();
        }

        public object getValue()
        {
            return currentValue;
        }

        /// <summary>
        /// Checks if the given GameEvent fulfills any of the modifiers release condition
        /// and if it does it updates the current value as well as the modifier list.
        /// </summary>
        /// <param name="e"></param>
        public void check(GameEvent e)
        {
            bool dirty = false;
            List<ModifiableStruct<T>> l = new List<ModifiableStruct<T>>(modifiers.Count);
            foreach (ModifiableStruct<T> v in modifiers)
            {
                if (v.unmodifyCondition.filter(e))
                {
                    dirty = true;
                }
                else
                {
                    l.Add(v);
                }
            }
            modifiers = l;
            if (dirty) reherp();
        }

        private void reherp()
        {
            currentValue = modifiers.Aggregate(baseValue, (current, m) => m.modifyFunction(current, m.modifier));
            notify(0);
        }

        public override string ToString()
        {
            return getValue().ToString();
        }
    }
}
