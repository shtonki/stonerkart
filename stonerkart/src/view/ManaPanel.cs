using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace stonerkart
{
    class ManaPanel : UserControl
    {
        private ManaButton[][] images;
        public readonly List<Action<Clickable>> callbacks = new List<Action<Clickable>>();

        public ManaPanel()
        {
            BackColor = Color.DarkGray;
            images = new ManaButton[6][];
            for (int i = 0; i < 6; i++)
            {
                images[i] = new ManaButton[6];
                for (int j = 0; j < 6; j++)
                {
                    var p = new ManaButton();
                    images[i][j] = p;
                    Controls.Add(p);
                    p.MouseDown += (_, __) => { foreach (var c in callbacks) c(p); };
                }
            }
            layoutPicures();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            layoutPicures();
        }

        private void layoutPicures()
        {
            int w = Size.Width/6;
            int h = Size.Height/6;
            int padding = 4;
            int dw = w - 2 * padding;
            int dh = h - 2 * padding;

            Bitmap[] fullColour = new Image[]
            {
                Properties.Resources.chaos, 
                Properties.Resources.death, 
                Properties.Resources.life, 
                Properties.Resources.might, 
                Properties.Resources.nature, 
                Properties.Resources.order, 
            }.Select(i => G.ResizeImage(i, dw, dh)).ToArray();

            Image[] halfColour = fullColour.Select(i => G.SetImageOpacity(i, 0.5f)).ToArray();

            Image m = G.ResizeImage(Properties.Resources.might, dw, dh);
            m = G.SetImageOpacity(m, 0.4f);
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    images[i][j].Bounds = new Rectangle(i * w + padding, j * h + padding, w - padding, h - padding);
                    images[i][j].Image = i > 2 ? fullColour[j] : halfColour[j];
                }
            }
        }
    }

    class ManaButton : Label, Clickable
    {
        public ManaOrb orb { get; }

        public Stuff getStuff()
        {
            Console.WriteLine("stuffing");
            return orb;
        }
    }
}
