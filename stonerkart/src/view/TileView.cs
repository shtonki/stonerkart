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
        public Point centre => new Point((int)poly[1].X, (int)((poly[0].Y + poly[3].Y)/2));

        public Color color { get; set; }

        public TileView(Tile tile)
        {
            this.tile = tile;
            color = Color.Black;
        }

        public Stuff getStuff()
        {
            return tile;
        }
    }
}