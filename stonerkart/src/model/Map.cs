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
        public int size => tiles.Length;

        public IEnumerable<Tile> Tiles => tiles;

        private Tile[] tiles;
        private Tile[][] cols;

        public static Map MapFromConfiguration(MapConfiguration mc)
        {
            switch (mc)
            {
                case MapConfiguration.Default: return new Map(9, 7);
                default: throw new Exception();
            }
        }

        public Map(int width, int height)
        {
            this.width = width;
            this.height = height;
            tiles = new Tile[width*height];

            cols = new Tile[width][];
            int c = 0;
            for (int x = 0; x < width; x++)
            {
                var col = new Tile[height];
                for (int y = 0; y < height; y++)
                {
                    col[y] = tiles[x*height + y] = new Tile(this, x, y);
                }
                cols[x] = col;
            }
        }

        public Tile tileAt(int x, int y)
        {
            return cols[x][y];
        }

        public Tile tileAt(int p)
        {
            return tiles[p];
        }

        public Path path(Card card, Tile endTile)
        {
            var startTile = card.tile;
            List<Path> v = pathCosts(card, startTile);
            return v.First(t => t.to == endTile);
        }

        public List<Path> pathCosts(Card traveller, Tile startTile)
        {
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
            if (from.card != null && from.card.controller != card.controller)
            {
                return 10000;
            }
            return 1;
        }

        public int ord(Tile t)
        {
            return Array.IndexOf(tiles, t);
        }
    }

    public enum MapConfiguration
    {
        Default,
    }

    class Path
    {
        public int length { get; private set; }
        public Tile from => tiles[0];
        public Tile last => tiles[Math.Max(0, tiles.Count - (attacking ? 2 : 1))];
        public Tile to => tiles[tiles.Count - 1];
        public bool attacking => to.card != null && to.card.controller != from.card?.controller;

        public Color colorHack { get; set; } = Color.ForestGreen;

        public List<Tile> tiles { get; private set; }

        public Path(Tile toAndFrom) : this(new Tile[] { toAndFrom }.ToList(), 0)
        {
        }

        public Path(List<Tile> tiles, int length)
        {
            if (tiles.Count < 1 || length < 0) throw new Exception();
            this.tiles = tiles;
            this.length = length;
        }

        public static implicit operator List<Tile>(Path p)
        {
            return p.tiles;
        }

        public void concat(Path p)
        {
            if (tiles.Count == 0)
            {
                tiles = p.tiles;
                length = p.length;
                return;
            }

            var cctiles = p.tiles;
            if (tiles.Last() != cctiles.First()) throw new Exception();

            tiles = tiles.Take(tiles.Count - 1).Concat(cctiles).ToList();
            length += p.length;
        }
    }
}
