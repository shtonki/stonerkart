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
            for (int row = 0; row < height; row++)
            {
                int w = width + (row % 2) * gpr;
                rows[row] = new Tile[w];
                for (int col = 0; col < w; col++)
                {
                    tiles[c] = new Tile(this, col, row);
                    rows[row][col] = tiles[c];
                    c++;
                }
            }
        }
        
        public Tile[] this[int r] => rows[r];

        public Tile tileAt(int a, int b, int c)
        {
            if (a + b + c != 0) throw new Exception();
            int y = c;
            int x = a + c / 2;
            if (y < 0 || y >= height || x < 0 || x >= rows[y].Length) return null;
            return rows[y][x];
        }

        private Tuple<int, int> ABCtoXY(int a, int b, int c)
        {
            int y = c;
            int x = a + c/2;
            return Tuple.Create<int, int>(x, y);
        }

        public Tuple<Tile, int>[] within()
        {
            return null;
        } 

        public int paddingAt(int r)
        {
            int rt =
                (r%2)*(1+gpr) +
                (r & 1)*-gpr;
            return rt;
        }

        public Tile tileAt(int x, int y)
        {
            return rows[y][x];
        }

        public Tile tileAt(int p)
        {
            return tiles[p];
        }

        
        public int ord(Tile t)
        {
            return t.x + t.y*width + t.y/2*gpr;
        }
    }
}
