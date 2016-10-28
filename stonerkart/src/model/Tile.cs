using System.Drawing;
using System;

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

        private readonly XYCoord xyCoord;
        private readonly ABCCoord abcCoord;

        public Tile(Map map, int x, int y)
        {
            this.map = map;
            xyCoord = new XYCoord(x, y);
            abcCoord = new ABCCoord(x, y);
        }

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
    }
}
