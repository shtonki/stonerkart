using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart
{
    class ManaCostPanel : TransparentPanel
    {
        private List<ManaColour> colouredCosts;
        private int colourless;

        public ManaCostPanel()
        {
            colouredCosts = new List<ManaColour>();
        }

        public void setCost(ManaSet cs)
        {
            colourless = cs[ManaColour.Colourless];
            colouredCosts = cs.colours.Where(x => x != ManaColour.Colourless).ToList();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            int h = Size.Height;
            int w = Size.Width;
            int dh = h;
            int v = colouredCosts.Count + (colourless > 0 ? 1 : 0); 
            int dw = Math.Min(h, v == 0 ? h : w/v);

            int j = 0;

            for (; j < colouredCosts.Count; j++)
            {
                Image b = ImageLoader.orbImage(colouredCosts[j]);
                Image i = G.ResizeImage(b, dw, dh);
                g.DrawImage(i, new PointF(w - j*dw - dw, 0));
            }
            if (colourless > 0)
            {
                Image b = ImageLoader.orbImage(ManaColour.Colourless);
                Image i = G.ResizeImage(b, dw, dh);
                g.DrawImage(i, new PointF(w - j * dw - dw, 0));
                g.DrawString(colourless.ToString(), new Font(new FontFamily("Arial"), dw * 0.75f), Brushes.Black, w - j*dw - dw + dw*0.1f, -0.1f*dh);
            }
        }

        /*{
            foreach (TransparentPanel l in images)
            {
                Controls.Remove(l);
            }

            List<TransparentPanel> ls = new List<TransparentPanel>();

            foreach (int v in costs)
            {
                for (int i = 0; i < v; i++)
                {
                    Label l = new Label();
                    l.BackColor = Color.Aqua;
                    Controls.Add(l);
                    ls.Add(l);
                }
            }

            images = ls.ToArray();
        }

        private void renigra()
        {
            int c = images.Length;
            if (c == 0) return;
            int h = Size.Height;
            int w = Size.Width;

            int dh = h;
            int dw = h;

            for (int i = 0; i < c; i++)
            {
                //images[i].SetBounds(w - i*dw, 0, dw, dh);
                images[i].SetBounds(0, 0, dw, dh);
                images[i].Image = G.ResizeImage(Properties.Resources.chaos, dw, dh);
                //images[i].Image = Properties.Resources.chaos;
            }
        }
        */
    }
}
