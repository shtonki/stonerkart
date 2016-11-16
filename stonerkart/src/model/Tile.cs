using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace stonerkart
{
    internal class Tile : Stuff
    {
        public Map map { get; }
        public int x => xyCoord.x;
        public int y => xyCoord.y;
        public int a => abcCoord.a;
        public int b => abcCoord.b;
        public int c => abcCoord.c;
        public int ord => map.ord(this);
        public Tile[] neighbours => atDistance(1).ToArray();

        public Card card { get; private set; }

        private readonly XYCoord xyCoord;
        private readonly ABCCoord abcCoord;

        public Tile(Map map, int x, int y)
        {
            this.map = map;
            xyCoord = new XYCoord(x, y);
            abcCoord = new ABCCoord(x, y);
        }

        public override string ToString()
        {
            return String.Format("Tile containing {0}",card == null ? "nothing" : card.ToString());
        }

        public void place(Card c)
        {
            if (card != null) throw new Exception();
            card = c;
            card.tile = this;
            Controller.redraw();
        }

        public Card removeCard()
        {
            if (card == null) throw new Exception();
            var r = card;
            card = null;
            Controller.redraw();
            return r;
        }

        public List<Tile> withinDistance(int distance)
        {
            IEnumerable<Tile> r = new List<Tile>(3*(distance*(distance + 1)) + 1);

            for (int d = 0; d <= distance; d++)
            {
                r = r.Concat(atDistance(d));
            }

            return r.ToList();
        }

        public List<Tile> atDistance(int distance)
        {
            List<Tile> ta = new List<Tile>();
            if (distance == 0)
            {
                ta.Add(this);
                return ta;
            }
            unsafe
            {
                int ba;
                int bb;
                int bc;
                var ps = new int*[][]
                {
                    new int*[] {&ba, &bb, &bc}, 
                    new int*[] {&bb, &bc, &ba}, 
                    new int*[] {&bc, &ba, &bb}, 
                };
                foreach (var v in ps)
                {
                    for (int r = -1; r <= 1; r = r + 2)
                    {
                        ba = a;
                        bb = b;
                        bc = c;
                        *v[0] += r*distance;
                        *v[1] -= r*distance;

                        for (int i = 0; i < distance; i++)
                        {
                            Tile t = map.tileAt(ba, bb, bc);
                            if (t != null) ta.Add(t);
                            *v[0] -= r*1;
                            *v[2] += r*1;
                        }
                    }
                }
            }
            return ta;
        }

        public List<Tile> atDistance2(int distance)
        {
            List<Tile> ta = new List<Tile>();
            if (distance == 0)
            {
                ta.Add(this);
                return ta;
            }

            int big, sma, mak;
            big = distance;

                for (sma = -big; sma <= -(big + 1) / 2; sma++)
                {
                    mak = -sma - big;
                    ta.Add(map.tileAt(a + big, b + sma, c + mak));
                    ta.Add(map.tileAt(a + big, b + mak, c + sma));
                    ta.Add(map.tileAt(a + sma, b + big, c + mak));
                    ta.Add(map.tileAt(a + sma, b + mak, c + big));
                    ta.Add(map.tileAt(a + mak, b + big, c + sma));
                    ta.Add(map.tileAt(a + mak, b + sma, c + big));
                    /*
                    ts[c++] = map.tileAt(a, b - 1, c + 1);
                    ts[c++] = map.tileAt(a + 1, b, c - 1);
                    ts[c++] = map.tileAt(a + 1, b - 1, c);
                    ts[c++] = map.tileAt(a, b + 1, c - 1);
                    ts[c++] = map.tileAt(a - 1, b, c + 1);
                    ts[c++] = map.tileAt(a - 1, b + 1, c);
                    */
                }
            /*
            Tile[] ts = new Tile[6];
            ts[0] = map.tileAt(a, b - 1, c + 1);
            ts[1] = map.tileAt(a + 1, b, c - 1);
            ts[2] = map.tileAt(a + 1, b - 1, c);
            ts[3] = map.tileAt(a, b + 1, c - 1);
            ts[4] = map.tileAt(a - 1, b, c + 1);
            ts[5] = map.tileAt(a - 1, b + 1, c);
            */
            return ta;
        }

        #region Coordnonsense
        class XYCoord
        {
            public readonly int x;
            public readonly int y;

            public XYCoord(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public XYCoord(int a, int b, int c)
            {
                y = c;
                x = a + y/2;
            }
            
        }

        class ABCCoord
        {
            public readonly int a;
            public readonly int b;
            public readonly int c;

            public ABCCoord(int a, int b, int c)
            {
                this.a = a;
                this.b = b;
                this.c = c;
            }

            public ABCCoord(int x, int y)
            {
                a = -y / 2 + x;
                b = -((y + 1) / 2 + x);
                c = y;
            }
            
        }
        #endregion
    }
}
