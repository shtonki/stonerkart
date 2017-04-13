using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Foo
    {
        protected Effect[] effects;

        public Foo()
        {
            effects = new Effect[0];
        }

        public Foo(params Effect[] effects)
        {
            this.effects = effects;
        }

        public IEnumerable<GameEvent> resolve(HackStruct hs, TargetMatrix[] tsx)
        {
            var ts = fillResolve(hs, tsx);
            if (ts == null) return new GameEvent[0];

            List<GameEvent> rt = new List<GameEvent>();

            for (int i = 0; i < ts.Length; i++)
            {
                Effect effect = effects[i];
                TargetRow[] rows = ts[i].generateRows(effect.straightRows);
                if (rows.Length == 0)
                {
                    if (!effects[i].allowEmpty()) return new GameEvent[0];
                }
                rt.AddRange(effect.doer.act(hs, rows));
            }

            return rt;
        }

        public TargetMatrix[] fillCast(HackStruct hs)
        {
            TargetMatrix[] rt = new TargetMatrix[effects.Length];

            for (int i = 0; i < effects.Length; i++)
            {
                rt[i] = effects[i].fillCast(hs);
                if (rt[i] == null) return null;

            }

            return rt;
        }

        public TargetMatrix[] fillResolve(HackStruct hs, TargetMatrix[] ts)
        {
            TargetMatrix[] rt = new TargetMatrix[effects.Length];

            for (int i = 0; i < effects.Length; i++)
            {
                rt[i] = effects[i].fillResolve(ts[i], hs);
                if (rt[i] == null) return null;
                hs.previousTargets = rt[i];
            }
            hs.previousTargets = null;

            return rt;
        }

        public bool possibleAsCost(HackStruct hs)
        {
            return effects.All(e => e.possibleAsCost(hs));
        }

        public bool possibleTargets(HackStruct hs)
        {
            foreach (Effect e in effects)
            {
                foreach (var r in e.ts.rules)
                {
                    var v = r.possible(hs);
                    int i = v.targets.Length;
                    if (i == 0 && !r.allowEmpty()) return false;
                    //hs.previousColumn = v;
                }
            }
            return true;
        }
    }
}