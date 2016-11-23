using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Effect
    {
        public TargetSet ts;
        public Doer doer;

        public Effect(TargetSet ts, Doer d)
        {
            this.ts = ts;
            doer = d;
        }
    }
    
    abstract class Doer
    {

        public abstract GameEvent[] act(Targetable[][] ts);

        public static Doer<T> simpleDoer<T>(Func<T, GameEvent[]> f)
        {
            return new Doer<T>((x) =>
            {
                List<GameEvent> rt = new List<GameEvent>();

                for (int i = 0; i < x.Length; i++)
                {
                    rt.AddRange(f(x[i]));
                }

                return rt.ToArray();
            });
        }

        public static Doer<T1, T2> simpleDoer<T1, T2>(Func<T1, T2, GameEvent[]> f)
        {
            return new Doer<T1, T2>((x, y) =>
            {
                List<GameEvent> rt = new List<GameEvent>();

                for (int i = 0; i < x.Length; i++)
                {
                    rt.AddRange(f(x[i], y[i]));
                }

                return rt.ToArray();
            });
        }
        
        public static Doer<Card, Tile> MoveToTileDoer()
        {
            return simpleDoer<Card, Tile>((card, tile) => new GameEvent[] {new MoveToTileEvent(card, tile), });
        }

        public static Doer<Card, Card> ZepDoer(int damage)
        {
            return simpleDoer<Card, Card>((source, target) => new GameEvent[] { new DamageEvent(source, target, damage),  });
        }
    }

    class Doer<T1> : Doer
    {
        private Func<T1[], GameEvent[]> f;

        public Doer(Func<T1[], GameEvent[]> f)
        {
            this.f = f;
        }

        public override GameEvent[] act(Targetable[][] ts)
        {
            if (ts.Length != 1) throw new Exception();
            
            return f(ts[0].Cast<T1>().ToArray());
        }
    }

    class Doer<T1, T2> : Doer
    {
        private Func<T1[], T2[], GameEvent[]> f;

        public Doer(Func<T1[], T2[], GameEvent[]> f)
        {
            this.f = f;
        }

        public override GameEvent[] act(Targetable[][] ts)
        {
            if (ts.Length != 2) throw new Exception();

            return f(
                ts[0].Cast<T1>().ToArray(),
                ts[1].Cast<T2>().ToArray()
                );
        }
    }
}
