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
            if (effects.Any(e => e == null)) throw new Exception();
            this.effects = effects;
        }

        public TargetMatrix fillCast(HackStruct hs)
        {
            TargetVector[] vectors = new TargetVector[effects.Length];

            for (int i = 0; i < effects.Length; i++)
            {
                TargetVector cache = effects[i].fillCast(hs);
                if (cache.Cancelled) return TargetMatrix.CreateCancelled();
                if (cache.Fizzled) return TargetMatrix.CreateFizzled();
                vectors[i] = cache;
            }

            return new TargetMatrix(vectors);
        }

        public TargetMatrix fillResolve(HackStruct hs, TargetMatrix cache)
        {
            if (effects.Length != cache.targetVectors.Length) throw new Exception();

            TargetVector[] vectors = new TargetVector[effects.Length];
            hs.previousTargets = vectors;

            for (int i = 0; i < effects.Length; i++)
            {
                TargetVector newcache = effects[i].fillResolve(hs, cache.targetVectors[i]);
                if (newcache.Cancelled) return TargetMatrix.CreateCancelled();
                if (newcache.Fizzled) return TargetMatrix.CreateFizzled();
                vectors[i] = newcache;
            }

            return new TargetMatrix(vectors);
        }

        public IEnumerable<GameEvent> resolve(HackStruct hs, TargetMatrix cached)
        {
            if (cached.targetVectors.Length != effects.Length) throw new Exception();

            List<GameEvent> rt = new List<GameEvent>();

            for (int i = 0; i < effects.Length; i++)
            {
                var effect = effects[i];
                var cache = cached.targetVectors[i];
                var events = effect.resolve(hs, cache);
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

        
    }
}