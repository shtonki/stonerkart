using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    interface Modifiable
    {
        void check(object e);
        object getValue();
        void modify(ModifierStruct modifierStruct);
    }

    abstract class TypedModifiable<M, C> : Observable<int>, Modifiable
    {
        public M baseValue  {get;}

        public M value => (M)getValue();

        public TypedModifiable(M baseValue)
        {
            this.baseValue = baseValue;
        }

        public static implicit operator M(TypedModifiable<M, C> m)
        {
            return m.value;
        }

        public void check(object e)
        {
            bool dirty = false;
            List<TypedModifierStruct<M, C>> rm = new List<TypedModifierStruct<M, C>>();
            foreach (var v in mss)
            {
                if (v.filter(e))
                {
                    rm.Add(v);
                    dirty = true;
                }
            }

            if (dirty)
            {
                foreach (var v in rm)
                {
                    mss.Remove(v);
                }
                notify(0);
            }
        }

        public object getValue()
        {
            M v = baseValue;
            foreach (TypedModifierStruct<M, C> m in mss)
            {
                v = optof(m.operation)(v, (M)m.modifier);
            }
            return v;
        }

        public void modify(ModifierStruct modifierStruct)
        {
            if (!(modifierStruct is TypedModifierStruct<M, C>)) throw new Exception();
            modifyT((TypedModifierStruct<M, C>)modifierStruct);
        }

        public void modify(M modifier, Operations op, Func<C, bool> f)
        {
            modify(new TypedModifierStruct<M, C>(modifier, op, o => o is C && f((C)o)));
        }

        public void modifyT(TypedModifierStruct<M, C> typedModifierStruct)
        {
            mss.Add(typedModifierStruct);
        }

        protected abstract Func<M, M, M> optof(Operations operation);

        private List<TypedModifierStruct<M, C>> mss = new List<TypedModifierStruct<M, C>>();
    }

    class ModifiableInt<C> : TypedModifiable<int, C>
    {
        private int? mxv, mnv;

        public ModifiableInt(int baseValue) : base(baseValue)
        {
        }

        public ModifiableInt(int baseValue, int? mnv, int? mxv) : base(baseValue)
        {
            this.mxv = mxv;
            this.mnv = mnv;
        }

        protected override Func<int, int, int> optof(Operations operation)
        {
            switch (operation)
            {
                case Operations.add: return (a, b) => a+b;
                case Operations.sub: return (a, b) => a-b;
                case Operations.set: return (a, b) => b;
            }
            throw new Exception();
        }
    }

    class ModifiableIntGE : ModifiableInt<GameEvent>
    {
        public ModifiableIntGE(int baseValue, int? mnv = null, int? mxv = null) : base(baseValue, mxv, mnv)
        {
        }

        public static Func<GameEvent, bool> startOfOwnersTurn(Card c)
        {
            return new TypedGameEventFilter<StartOfStepEvent>(
                    e => e.activePlayer == c.controller && e.step == Steps.Untap).filter;
        }

        public static Func<GameEvent, bool> never()
        {
            return e => false;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }

    class ModifierStruct
    {
        public object modifier;
        public Operations operation;
        public Func<object, bool> filter;

        public ModifierStruct(object modifier, Operations operation, Func<object, bool> filter)
        {
            this.modifier = modifier;
            this.operation = operation;
            this.filter = filter;
        }
    }

    class TypedModifierStruct<M, C> : ModifierStruct
    {
        public M typedModifier => (M) modifier;

        public TypedModifierStruct(M modifier, Operations operation, Func<C, bool> fltr) : base(modifier, operation, o => o is C && fltr((C)o))
        {
        }
    }

    enum Operations
    {
        add,
        sub,
        set,
    }
    /*
    interface Modifiable
    {
        void check(GameEvent e);
        object getValue();
    }

    class ModifiableInt : Modifiable
    {
        public enum Operations
        {
            Add,
            Subtract,
            Set,
        }

        public int baseValue { get; }
        public int? maxValue { get; }
        public int? minValue { get; }
        public int value => (int)getValue();

        public ModifiableInt(int baseValue, int? maxValue, int? minValue)
        {
            this.baseValue = baseValue;
            this.maxValue = maxValue;
            this.minValue = minValue;
        }

        public ModifiableInt(int baseValue)
        {
            this.baseValue = baseValue;
        }

        public void modify(int modifier, Operations op, GameEventFilter f)
        {
            modify(new ModStruct(modifier, op, f));
        }

        public void modify(ModStruct ms)
        {
            mods.Add(ms);
        }

        public void check(GameEvent e)
        {
            throw new NotImplementedException();
        }

        public object getValue()
        {
            throw new NotImplementedException();
        }

        private List<ModStruct> mods = new List<ModStruct>();
        private bool dirty;

        private delegate int f(int v, int m);

        private f add = (a, b) => a + b;
        private f sub = (a, b) => a - b;
        private f set = (a, b) => b;

        public struct ModStruct
        {
            public readonly int val;
            public readonly Operations op;
            public readonly GameEventFilter filter;

            public ModStruct(int val, Operations op, GameEventFilter filter)
            {
                this.val = val;
                this.op = op;
                this.filter = filter;
            }
        }
    }
    */
}
