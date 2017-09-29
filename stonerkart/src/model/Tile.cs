using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace stonerkart
{
    class Tile : Stuff
    {
        public enum TileType
        {
            Invisible,
            Normie,
            NoSummon,
            UnpassableOUTOFORDER,
        }

        public Color? RimHighlight { get; set; }
        public TileType tileType { get; set; }
        public Map map { get; }
        public int x => xyCoord.x;
        public int y => xyCoord.y;
        public int a => abcCoord.a;
        public int b => abcCoord.b;
        public int c => abcCoord.c;
        public int ord => map.ord(this);

        public Card card { get; private set; }

        public bool Passable => card == null && (tileType == TileType.Normie || tileType == TileType.NoSummon);
        public bool Summonable => Passable && tileType == TileType.Normie;


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
            return x + " " + y;
        }

        public void place(Card c)
        {
            if (card != null) throw new Exception();
            card = c;
            card.tile = this;
        }

        public Card removeCard()
        {
            if (card == null) throw new Exception();
            var r = card;
            card.tile = null;
            card = null;
            return r;
        }

        public bool enterableBy(Card c)
        {
            return card == null || c.canAttack(card);
        }

        public List<Tile> withinDistance(int distance, int mindistance = 0)
        {
            if (distance == -1) return map.Tiles.ToList();

            IEnumerable<Tile> r = new List<Tile>(3*(distance*(distance + 1)) + 1);

            for (int d = mindistance; d <= distance; d++)
            {
                r = r.Concat(atDistance(d));
            }

            return r.ToList();
        }

        public List<Tile> atDistance(int distance)
        {
            List<Tile> ta = new List<Tile>();
            foreach (var t in map.Tiles)
            {
                int da = Math.Abs(a - t.a);
                int db = Math.Abs(b - t.b);
                int dc = Math.Abs(c - t.c);
                if (da + db + dc == distance*2)
                {
                    ta.Add(t);
                }
            }
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
                throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
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
                a = y + x/2;
                b = -x;
                c = -(a + b);
            }
            
        }
        #endregion
    }
}
