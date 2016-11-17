using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
        private TileView[] tileViews;

        public HexPanel(Map m)
        {
            DoubleBuffered = true;
            BackColor = Color.Aqua;

            map = new Map(1, 1, false, false);

            setMap(m);

            getDwDh(out dw, out dh);
        }


        public HexPanel() : this(new Map(2, 2, false, false))
        {
        }

        public void setMap(Map m)
        {
            map = m;
            tileViews = new TileView[map.size];
            for (int i = 0; i < map.size; i++) tileViews[i] = new TileView(map.tileAt(i));
        }

        int HACK1 = 2, HACK2 = 7; /* todo unhack */
        private void getDwDh(out int dw, out int dh)
        {
            
            dw = 2 * (Size.Width / (map.widthEx)) - HACK1;
            dh = (int)Math.Floor(Size.Height/(1 + (-1 + map.height)*0.75));
        }

        private int dw, dh;
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            
            getDwDh(out dw, out dh);
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
                    tileViews[c++].poly = v;
                }
                indent = !indent;
            }

            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var v = clickToTile(e);
            Controller.mouseEntered(v);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Controller.mouseEntered(null);
        }



        protected override void OnMouseDown(MouseEventArgs e)
        {
            var v = clickToTile(e);
            if (v != null)
                Controller.clicked(clickToTile(e));
        }

        private TileView clickToTile(MouseEventArgs e)
        {
            PointF clickPoint = new PointF(e.X, e.Y);
            foreach (var hex in tileViews)
            {
                if (pip(hex.poly, clickPoint))
                {
                    return hex;
                }
            }
            return null;
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
            
            using (Pen pen = new Pen(Color.Black, 4))
            {
                foreach (var tv in tileViews)
                {
                    g.DrawPolygon(pen, tv.poly);
                    if (tv.tile.card != null)
                    {
                        Card card = tv.tile.card;
                        TextureBrush bh = new TextureBrush(G.ResizeImage(card.image, dw, dh));
                        var x = tv.poly[0].X;
                        var y = tv.poly[0].Y;
                        var m = new Matrix();
                        m.Translate(x + dw * ((float)75 / 78), y + dh * ((float)35 / 51));
                        bh.Transform = m;
                        
                        g.FillPolygon(bh, tv.poly);

                        int b = (int)((tv.poly[3].Y - tv.poly[0].Y)/2);
                        int a = (int)((tv.poly[2].X - tv.poly[0].X)/4);
                        Font f = new Font("Ariel Black", (b+a) / 5, FontStyle.Bold);
                        float vxR = tv.poly[0].X;
                        float vxL = tv.poly[2].X - a;
                        float vy = (4*tv.poly[0].Y + 6*tv.poly[3].Y)/10;
                        float vxM = tv.poly[4].X - a/2;
                        float vyB = tv.poly[4].Y - b;
                        int yp = 5;
                        int xp = 1;

                        g.FillEllipse(Brushes.Crimson, vxR, vy, a, b);
                        g.DrawString(card.power.ToString().PadLeft(2), f, Brushes.Black, vxR + xp, vy + yp);

                        g.FillEllipse(Brushes.LightSeaGreen, vxL, vy, a, b);
                        g.DrawString(card.toughness.ToString().PadLeft(2), f, Brushes.Black, vxL + xp, vy + yp);
                        
                        g.FillEllipse(Brushes.LightGray, vxM, vyB, a, b);
                        g.DrawString(card.baseMovement.ToString().PadLeft(2),  f, Brushes.Black, vxM + xp, vyB + yp);
                    }
                    else
                    {
                        using (Brush b = new SolidBrush(tv.color))
                            g.FillPolygon(b, tv.poly);
                    }
                }
            }
        }

        public TileView viewOf(Tile t)
        {
            return tileViews[t.ord];
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
