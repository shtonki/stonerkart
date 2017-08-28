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

        public IEnumerable<GameEvent> resolve(HackStruct hs, TargetSet[][] cached)
        {
            if (cached.Length != effects.Length) throw new Exception();

            List<GameEvent> rt = new List<GameEvent>();

            for (int i = 0; i < effects.Length; i++)
            {
                var effect = effects[i];
                var cache = cached[i];
                var events = effect.
                resolve(hs, cache);
                if (events == null) //targeting was cancelled
                {
                    i = -1;
                }
                else
                {
                    rt.AddRange(events);
                }
            }

            return rt;
        }

        public TargetSet[][] fillCast(HackStruct hs)
        {
            TargetSet[][] rt = new TargetSet[effects.Length][];

            for (int i = 0; i < effects.Length; i++)
            {
                var cache = effects[i].fillCast(hs);
                if (cache == null) return null;
                rt[i] = cache;
            }

            return rt;
        }
        
    }
}