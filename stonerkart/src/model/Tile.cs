using System.Drawing;
using System;
using System.Collections.Generic;

namespace stonerkart
{
    class Tile
    {
        public Map map { get; }
        public int x => xyCoord.x;
        public int y => xyCoord.y;
        public int a => abcCoord.a;
        public int b => abcCoord.b;
        public int c => abcCoord.c;
        public int ord => map.ord(this);

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
            return String.Format("Tile at {0} {1}", x, y);
        }

        public void play(Card c)
        {
            card = c;
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
