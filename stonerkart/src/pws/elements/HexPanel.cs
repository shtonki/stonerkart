using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace stonerkart
{
    class HexPanel : Square
    {

        public Map Map
        {
            get { return map; }
            set
            {
                map = value;
                setMap(map);
            }
        }

        public int hexsize { get; private set; }
        private int hexsizethreequarters;
        private int xcount => map.width;
        private int ycount => map.height;

        private Map map { get; set; }
        private List<Card> GetMapCards() { return map.Tiles.Where(t => t.card != null).Select(t => t.card).ToList(); }

        public HexPanel(int width, int height) : base(width, height)
        {
        }

        private void setMap(Map map)
        {
            int widthinhexquarters = 1 + map.width * 3;
            var hs1 = 4 * Width / widthinhexquarters;

            int heightinhexquarters = 2 + map.height * 4;
            int hs2 = 4 * Height / heightinhexquarters;

            hexsize = Math.Min(hs1, hs2);
            hexsizethreequarters = (int)Math.Round(hexsize * 0.75);
        }

        public void SubToTiles(PublicSaxophone sax, Func<int, int, Tile> tileFromXY)
        {
            sax.sub(this, (a, g) =>
            {
                var v = findHexagon(a.Position.X, a.Position.Y);
                if (v == null) return null;
                return tileFromXY(v.Item1, v.Item2);
            });
        }

        private CardView rightclickview;
        public override void onMouseDown(MouseButtonEventArgs args)
        {
            base.onMouseDown(args);

            if (args.Button == MouseButton.Right)
            {
                if (map == null) return;
                var v = findHexagon(args.Position.X, args.Position.Y);
                if (v != null)
                {
                    var drawme = GetMapCards();
                    foreach (var c in drawme)
                    {
                        if (c.tile.x == v.Item1 && c.tile.y == v.Item2)
                        {
                            ShowRightClickCard(c, args.X - AbsoluteX, args.Y - AbsoluteY);
                        }
                    }
                }
            }
        }
        public override void onMouseUp(MouseButtonEventArgs args)
        {
            base.onMouseUp(args);

            HideRightClickCard();
        }
        public override void onMouseExit(MouseMoveEventArgs args)
        {
            base.onMouseExit(args);

            HideRightClickCard();
        }


        private void ShowRightClickCard(Card c, int x, int y)
        {
            if (rightclickview != null) throw new Exception();
            rightclickview = new CardView(Card.fromTemplate(c.template));
            rightclickview.Height = 400;
            rightclickview.X = x;
            rightclickview.Y = y;
            rightclickview.Hoverable = false;
            addChild(rightclickview);
        }
        private void HideRightClickCard()
        {
            if (rightclickview == null) return;
            removeChild(rightclickview);
            rightclickview = null;
        }


        private Tuple<int, int> findHexagon(int mousex, int mousey)
        {
            int cx = 0;
            int cy = 0;

            for (int i = 0; i < xcount; i++)
            {
                for (int j = 0; j < ycount; j++)
                {
                    var p = hexCoords(i, j);
                    int hexX = x + p.X + hexsize / 2;
                    int hexY = y + p.Y + hexsize / 2;

                    int dx = hexX - mousex;
                    int dy = hexY - mousey;

                    if (Math.Abs(dx) < hexsize / 2 && Math.Abs(dy) < hexsize / 2)
                    {
                        var xd = Math.Abs(dx);
                        var yd = Math.Abs(dy);

                        if (xd < hexsize / 4)
                        {
                            return new Tuple<int, int>(i, j);
                        }

                        xd -= hexsize / 4;

                        if (hexsize / 4 - yd / 2 > xd)
                        {
                            return new Tuple<int, int>(i, j);
                        }
                    }
                }
            }
            return null;
        }

        public Point hexCoords(int column, int row)
        {
            int os = ((column + 1) % 2) * hexsize / 2;
            int hexX = (int)(hexsizethreequarters * column);
            int hexY = row * hexsize + os;
            return new Point(hexX, hexY);
        }

        protected override void draw(DrawerMaym dm)
        {
            base.draw(dm);

            if (Map == null) return;

            var drawme = GetMapCards();

            for (int i = 0; i < xcount; i++)
            {
                for (int j = 0; j < ycount; j++)
                {
                    var p = hexCoords(i, j);
                    int hexX = p.X;
                    int hexY = p.Y;

                    Tile tile = map.tileAt(i, j);

                    Color bordercolor;
                    Color fillcolor;

                    switch (tile.tileType)
                    {
                        case Tile.TileType.Invisible:
                            {
                                bordercolor = Color.Transparent;
                                fillcolor = Color.Transparent;
                            }
                            break;

                        case Tile.TileType.Normie:
                            {
                                bordercolor = Color.DarkSlateGray;
                                fillcolor = Color.MintCream;
                            }
                            break;

                        case Tile.TileType.NoSummon:
                            {
                                bordercolor = Color.Maroon;
                                fillcolor = Color.Silver;
                            }
                            break;

                        case Tile.TileType.UnpassableOUTOFORDER:
                            {
                                bordercolor = Color.Black;
                                fillcolor = Color.Black;
                            }
                            break;

                        default: throw new Exception();
                    }

                    if (tile.RimHighlight.HasValue) bordercolor = tile.RimHighlight.Value;

                    dm.fillHexagon(hexX, hexY, hexsize, bordercolor, fillcolor);
                }
            }

            foreach (var c in drawme)
            {
                if (c.tile == null) continue;
                var i = c.tile.x;
                var j = c.tile.y;

                var p = hexCoords(i, j);
                int hexX = p.X;
                int hexY = p.Y;

                dm.fillHexagon(hexX, hexY, hexsize, Color.Transparent, TextureLoader.cardArt(c.template));

                int statTextSize = (int)(hexsize * 0.20);
                TextLayout tl = new SingleLineFitLayout(Justify.Middle);
                Color clr = c.owner.isHero ? Color.DarkGreen : Color.DarkRed;

                var toughnessText = tl.Layout(c.toughness.ToString(), statTextSize, statTextSize, ff);
                var powerText = tl.Layout(c.power.ToString(), statTextSize, statTextSize, ff);
                var movementText = tl.Layout(c.movement.ToString(), statTextSize, statTextSize, ff);

                int movementX = hexX + (int)(hexsize * 0.76);
                int movementY = hexY + (int)(hexsize * 0.41);

                dm.fillHexagon(movementX, movementY, statTextSize, clr, clr);
                movementText.draw(dm, movementX, movementY, 0, Color.Black, true);

                if (c.hasPT)
                {
                    int toughnessX = hexX + ((int)(hexsize * 0.58));
                    int toughnessY = hexY + ((int)(hexsize * 0.765));
                    int powerX = hexX + ((int)(hexsize * 0.22));
                    int powerY = toughnessY;

                    dm.fillHexagon(toughnessX, toughnessY, statTextSize, clr, clr);
                    toughnessText.draw(dm, toughnessX, toughnessY, 0, Color.Black, true);

                    dm.fillHexagon(powerX, powerY, statTextSize, clr, clr);
                    powerText.draw(dm, powerX, powerY, 0, Color.Black, true);
                }
            }

            lock (paths)
            {
                foreach (var path in paths)
                {
                    var tiles = path.tiles;
                    var from = tiles[0];


                    for (int i = 1; i < tiles.Count; i++)
                    {
                        var to = tiles[i];
                        arrow(from.x, from.y, to.x, to.y, Color.Aqua, dm);
                        from = to;
                    }
                }
            }
        }

        private void arrow(int xorg, int yorg, int xend, int yend, Color c, DrawerMaym dm)
        {
            var p1 = hexCoords(xorg, yorg);
            var p2 = hexCoords(xend, yend);
            int hsd2 = hexsize / 2;
            dm.drawLine(p1.X + hsd2, p1.Y + hsd2, p2.X + hsd2, p2.Y + hsd2, c);
        }

        private List<Path> paths = new List<Path>();

        public void addPath(Path p)
        {
            lock (paths)
            {
                paths.Add(p);
            }
        }

        public void clearPaths()
        {
            lock (paths)
            {
                paths.Clear();
            }
        }

        public void removeArrow(Path p)
        {
            lock (paths)
            {
                paths.Remove(p);
            }
        }

        FontFamille ff = FontFamille.font1;
    }
}
