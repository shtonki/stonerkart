using System.Drawing;
using System;
using System.Collections.Generic;

namespace stonerkart
{
    class TileView : Clickable
    {
        public Tile tile { get; }
        public PointF[] poly { get; set; }
        public int x => tile.x;
        public int y => tile.y;

        public Color color { get; set; }

        public TileView(Tile tile)
        {
            this.tile = tile;
            color = Color.Firebrick;
        }

        public Stuff getStuff()
        {
            return tile;
        }
    }
}