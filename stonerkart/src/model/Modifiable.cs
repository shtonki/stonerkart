using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Modifiable
    {
        public int baseValue;
        public int? minValue;
        public int? maxValue;

        public int value => getValue();

        private List<ModifierStruct> modifiers = new List<ModifierStruct>();
        private int cachedValue;
        private bool dirty { get; set; } = true;


        public Modifiable(int baseValue, int? minValue = null, int? maxValue = null)
        {
            this.baseValue = baseValue;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public static implicit operator int(Modifiable m)
        {
            return m.value;
        }

        public void check(GameEvent e)
        {
            List<ModifierStruct> rm = new List<ModifierStruct>();
            foreach (var v in modifiers)
            {
                if (v.filter.filter(e))
                {
                    rm.Add(v);
                    dirty = true;
                }
            }

            if (dirty)
            {
                foreach (var v in rm)
                {
                    modifiers.Remove(v);
                }
            }
        }

        public void modify(ModifierStruct ms)
        {
            modifiers.Add(ms);
            //dirty = true;
        }

        public void recount()
        {
            int v = baseValue;

            foreach (ModifierStruct m in modifiers)
            {
                v = m.operation(v);
            }

            cachedValue = v;
            dirty = false;
        }

        private int getValue()
        {
            if (dirty) recount();
            int r = cachedValue;
            r = Math.Max(r, minValue??r);
            r = Math.Min(r, maxValue??r);
            return r;
        }


        

        public override string ToString()
        {
            return value.ToString();
        }
    }
    struct ModifierStruct
    {
        public readonly Func<int, int> operation;
        public readonly GameEventFilter filter;

        public ModifierStruct(Func<int, int> operation, GameEventFilter filter)
        {
            this.operation = operation;
            this.filter = filter;
        }
    }

    struct RaceModifierStruct
    {
        public readonly Race race;
        public readonly GameEventFilter filter;

        public RaceModifierStruct(Race race, GameEventFilter filter)
        {
            this.race = race;
            this.filter = filter;
        }
    }
}
