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
        private Map map;
        private TileView[] hexes;

        public HexPanel()
        {
            DoubleBuffered = true;
            BackColor = Color.Aqua;
            map = new Map(5, 5, false, true);
            hexes = new TileView[map.size];
            for (int i = 0; i < map.size; i++) hexes[i] = new TileView(map.tileAt(i));
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            int HACK1 = 2, HACK2 = 7; /* todo unhack */
            int W = Size.Width;
            int H = Size.Height;
            int dw = 2*(W / (map.widthEx)) - HACK1;
            int dh = (int)Math.Floor(H / (1 + (-1 + map.height) * 0.75));

            PointF[] hexagon = generateHexagon(dw, dh, 1);
            bool indent = map.fatLeft;
            int c = 0;
            for (int y = 0; y < map.height; y++)
            {
                for (int x = 0; x < map[y].Length; x++)
                {
                    int ox = ((map.width - map[y].Length) / 2 + x) * dw
                             + (indent ? dw/2 : 0)
                             + HACK2;
                    int oy = (int)Math.Floor(0.75 * y * dh);

                    var v = hexagon.Select(a => new PointF(a.X + ox, a.Y + oy)).ToArray();
                    hexes[c++].poly = v;
                }
                indent = !indent;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!xd(e, Controller.entered)) Controller.entered(null);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            xd(e, Controller.clicked);
        }

        private bool xd(MouseEventArgs e, Action<TileView> a)
        {
            base.OnMouseClick(e);
            PointF clickPoint = new PointF(e.X, e.Y);
            foreach (var hex in hexes)
            {
                if (pip(hex.poly, clickPoint))
                {
                    a(hex);
                    return true;
                }
            }
            return false;
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
            int dw = (int)Math.Round(((float)W / (map.width)));
            int dh = (int)Math.Round(H / (1 + (-1+map.height)*0.75));
            int to = dw/2;
            using (Brush b = new SolidBrush(Color.Firebrick))
            using (Brush bh = new SolidBrush(Color.DeepPink))
            using (Pen pen = new Pen(Color.Black, 4))
            {
                foreach (var tv in hexes)
                {
                    g.DrawPolygon(pen, tv.poly);
                    g.FillPolygon(tv.highlighted ? bh : b, tv.poly);
                }
            }
        }


        private static PointF[] generateHexagon(int w, int h, int e)
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
    }
}
