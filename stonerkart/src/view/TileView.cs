using System.Drawing;

namespace stonerkart
{
    class TileView
    {
        public Tile tile { get; }
        public PointF[] poly { get; set; }
        public int x => tile.x;
        public int y => tile.y;
        public bool highlighted;

        public TileView(Tile tile)
        {
            this.tile = tile;
        }
    }
}