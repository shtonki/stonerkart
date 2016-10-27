using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;

namespace stonerkart
{
    class HexPanel : Panel
    {
        private Map map = new Map(null);
        private List<Tuple<Tile, PointF[]>> hexes;

        public HexPanel()
        {
            DoubleBuffered = true;
            BackColor = Color.Aqua;
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            int W = Size.Width;
            int H = Size.Height;
            int dw = (W / (map.width));
            int dh = (int)Math.Floor(H / (1 + (-1 + map.height) * 0.75));

            PointF[] hexagon = generateHexagon(dw, dh);
            int to = dw / 2;

            hexes = new List<Tuple<Tile, PointF[]>>();
            for (int y = 0; y < map.height; y++)
            {
                for (int x = 0; x < map.widthAt(y); x++)
                {
                    int ox = ((map.width - map.widthAt(y)) / 2 + x) * dw + to;
                    int oy = (int)Math.Floor(0.75 * y * dh);

                    var v = hexagon.Select(a => new PointF(a.X + ox, a.Y + oy)).ToArray();
                    hexes.Add(new Tuple<Tile, PointF[]>(map.tileAt(x, y), v));
                }
                to = to == 0 ? dw / 2 : 0;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            xd(e, Controller.clicked);
        }

        private void xd(MouseEventArgs e, Action<Tile> a)
        {
            base.OnMouseClick(e);
            PointF clickPoint = new PointF(e.X, e.Y);
            foreach (var hex in hexes)
            {
                if (pip(hex.Item2, clickPoint))
                {
                    a(hex.Item1);
                }
            }
        }

        private static bool pip(PointF[] polygon, PointF testPoint)
        {
            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y || polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y)
                {
                    if (polygon[i].X + (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < testPoint.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;

            int W = Size.Width;
            int H = Size.Height;
            int dw = (W / (map.width));
            int dh = (int)Math.Floor(H / (1 + (-1+map.height)*0.75));

            PointF[] hexagon = generateHexagon(dw, dh);
            int to = dw/2;
            using (Brush b = new SolidBrush(Color.Aqua))
            using (Pen pen = new Pen(Color.Red, 4))
            {
                //drawHexagon(g, pen, new Rectangle(0, 0, 50, 50));

                for (int y = 0; y < map.height; y++)
                {
                    for (int x = 0; x < map.widthAt(y); x++)
                    {
                        int ox = ((map.width - map.widthAt(y))/2 + x)*dw + to;
                        int oy = (int)Math.Floor(0.75*y*dh);

                        var v = hexagon.Select(a => new PointF(a.X + ox, a.Y + oy)).ToArray();

                        using (Brush p = new SolidBrush(Color.DarkSlateBlue))
                            g.FillPolygon(p, v);
                        using (Pen p = new Pen(Color.Indigo, 4))
                        {
                            g.DrawPolygon(p, v);
                            g.DrawString(-(y/2 + x) + " " + (-(y + 1)/2 - x) + " " + y, DefaultFont, Brushes.Black, ox + dw/2, oy + dh/2);
                        }
                    }
                    to = to == 0 ? dw/2 : 0;
                }
            }
        }

        public static int e = 1;

        private static PointF[] generateHexagon(int w, int h)
        {
            Rectangle area = new Rectangle(e, e, w - e, h - e);
            PointF[] ps = new PointF[6];
            int wm = e + (w - e - e)/2;
            int h1 = e + (h - e - e)/4;
            int h2 = h - h1;
            ps[0] = new PointF(e, h1);
            ps[1] = new PointF(wm, e);
            ps[2] = new PointF(w-e, h1);
            ps[3] = new PointF(w-e, h2);
            ps[4] = new PointF(wm, h-e);
            ps[5] = new PointF(e, h2);
            return ps;
        }


        private class TileView
        {
            public Tile tile { get; }
            public bool highlighted;

            public TileView(Tile tile)
            {
                this.tile = tile;
            }
        }
    }
}
