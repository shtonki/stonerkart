using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System;

namespace stonerkart
{
    class Map
    {
        public int width => tiles.Max(x => x.Length);
        public int height => tiles.Length;

        private Tile[][] tiles; /* array of rows */

        public Map(int[] ws)
        {
            ws = new int[] {4, 5, 2};
            tiles = ws.Select((l, y) => Enumerable.Range(0, l).Select(x => new Tile(x, y)).ToArray()).ToArray();
        }

        public int widthAt(int i)
        {
            return tiles[i].Length;
        }

        public Tile tileAt(int x, int y)
        {
            return tiles[y][x];
        }
    }
}
