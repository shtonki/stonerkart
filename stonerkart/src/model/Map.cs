using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System;

namespace stonerkart
{
    class Map
    {

        public readonly int width;
        public readonly int height;
        public readonly bool fatLeft;
        public readonly bool fatRight;
        public readonly int widthEx;
        public int size => tiles.Length;

        private Tile[] tiles;
        private int gpr;
        private Tile[][] rows;

        public Map(int width, int height, bool fatLeft, bool fatRight)
        {
            this.width = width;
            this.height = height;
            this.fatLeft = fatLeft;
            this.fatRight = fatRight;
            gpr = ((fatLeft ? 1 : 0) + (fatRight ? 1 : 0) - 1);
            tiles = new Tile[width * height + (height/2)*gpr];
            widthEx = width * 2 + gpr + 1;

            rows = new Tile[height][];
            int c = 0;
            for (int r = 0; r < height; r++)
            {
                int w = width + (r % 2) * gpr;
                rows[r] = new Tile[w];
                for (int i = 0; i < w; i++)
                {
                    rows[r][i] = tiles[c];
                }
            }
        }
        

        public Tile[] this[int r] => rows[r];

        public int paddingAt(int r)
        {
            int rt =
                (r%2)*(1+gpr) +
                (r & 1)*-gpr;
            return rt;
        }

        public Tile tileAt(int x, int y)
        {
            return null;
        }
    }
}
