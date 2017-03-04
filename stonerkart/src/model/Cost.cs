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

        public IEnumerable<GameEvent> resolve(HackStruct hs, TargetMatrix[] ts)
        {
            List<GameEvent> rt = new List<GameEvent>();

            for (int i = 0; i < ts.Length; i++)
            {
                Effect effect = effects[i];
                TargetMatrix matrix = effect.ts.fillResolve(ts[i], hs);
                hs.previousTargets = matrix;

                rt.AddRange(effect.doer.act(hs, matrix.generateRows()));
            }
            hs.previousTargets = null;

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

        public TargetMatrix[] fillResolve(TargetMatrix[] tms, HackStruct hs)
        {
            TargetMatrix[] rt = new TargetMatrix[effects.Length];

            for (int i = 0; i < effects.Length; i++)
            {
                rt[i] = effects[i].fillResolve(tms[i], hs);
                if (rt[i] == null) return null;
            }

            return rt;
        }

        public bool possible(HackStruct hs)
        {
            return effects.All(e => e.possible(hs));
        }
    }

}