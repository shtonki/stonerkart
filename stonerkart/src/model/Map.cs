using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System;

namespace stonerkart
{
    internal class Map
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
            tiles = new Tile[width*height + (height/2)*gpr];
            widthEx = width*2 + gpr + 1;

            rows = new Tile[height][];
            int c = 0;
            for (int row = 0; row < height; row++)
            {
                int w = width + (row%2)*gpr;
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
            int x = a + c/2;
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
                (r%2)*(1 + gpr) +
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

        public Path path(Tile startTile, Tile endTile)
        {
            List<Path> v = dijkstra(startTile);
            return v.First(t => t.last == endTile);
        }

        public List<Path> dijkstra(Tile startTile)
        {
            Card traveller = startTile.card;
            Dictionary<Tile,Tuple<int, Tile>> dict = new Dictionary<Tile, Tuple<int, Tile>>();
            List<Tile> Q = new List<Tile>();

            foreach (Tile t in tiles)
            {
                Q.Add(t);
                dict[t] = Tuple.Create<int, Tile>(Int32.MaxValue/2, null);
            }
            dict[startTile] = Tuple.Create<int, Tile>(0, null);

            while (Q.Count > 0)
            {
                Tile u = null;
                int w = Int32.MaxValue/2;

                foreach (Tile t in Q)
                {
                    if (dict[t].Item1 <= w)
                    {
                        w = dict[t].Item1;
                        u = t;
                    }
                }
                foreach (Tile v in u.atDistance(1))
                {
                    int a = dict[u].Item1 + cost(u, v, traveller);
                    if (a < dict[v].Item1) dict[v] = new Tuple<int, Tile>(a, u);
                }
                Q.Remove(u);
            }

            var r = new List<Path>();

            foreach (KeyValuePair<Tile, Tuple<int, Tile>> v in dict)
            {
                List<Tile> l = new List<Tile>();
                Tile c = v.Key;
                while (c != null)
                {
                    l.Add(c);
                    c = dict[c].Item2;
                }

                l.Reverse();
                r.Add(new Path(l, v.Value.Item1));
            }
            return r;
        }

        private int cost(Tile from, Tile to, Card card)
        {
            if (card == null) return 1;
            if ((to.card != null && to.card.owner == card.owner) ||
                from.card != null && from.card != card)
            {
                    return 10000;
            }
            return 1;
        }

        public int ord(Tile t)
        {
            return t.x + t.y*width + t.y/2*gpr;
        }
    }

    struct Path
    {
        public int length { get; }
        public Tile from => tiles[0];
        public Tile to => tiles[Math.Max(0, tiles.Count - (attacking ? 2 : 1))];
        public Tile last => tiles[tiles.Count - 1];
        public bool attacking => last.card != from.card && last.card != null;

        private List<Tile> tiles;

        public Path(List<Tile> tiles, int length)
        {
            if (length < 0) throw new Exception();
            this.tiles = tiles;
            this.length = length;
        }

        public static implicit operator List<Tile>(Path p)
        {
            return p.tiles;
        }
    }
}
