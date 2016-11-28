﻿using System;
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
        private ManaColour[] costs;

        public ManaCostPanel()
        {
            costs = new ManaColour[0];
        }

        public void setCost(int[] costs)
        {
            List<ManaColour> l = new List<ManaColour>();
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < costs[i]; j++)
                {
                    l.Add((ManaColour)i);
                }
            }
            this.costs = l.ToArray();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            int h = Size.Height;
            int w = Size.Width;
            int dh = h;
            int dw = h;

            for (int j = 0; j < costs.Length; j++)
            {
                Image b = G.manaImage(costs[j]);
                Image i = G.ResizeImage(b, dw, dh);
                g.DrawImage(i, new PointF(w - j*dw - dw, 0));
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
