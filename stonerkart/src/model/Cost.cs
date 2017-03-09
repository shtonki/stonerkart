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
            ts = fillResolve(hs, ts);

            List<GameEvent> rt = new List<GameEvent>();

            for (int i = 0; i < ts.Length; i++)
            {
                Effect effect = effects[i];
                rt.AddRange(effect.doer.act(hs, ts[i].generateRows(effect.straightRows)));
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
    }
}