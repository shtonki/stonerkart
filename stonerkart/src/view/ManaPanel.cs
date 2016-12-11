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
                    var p = new ManaButton((ManaColour)i, 0.3f);
                    p.setVisibility(ManaButton.Visibility.Hidden);
                    images[i][j] = p;
                    Controls.Add(p);
                    p.MouseDown += (_, __) => { foreach (var c in callbacks) c(p); };
                }
            }
            layoutPicures();
        }

        public void setLightUp(ManaPool p)
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    ManaButton.Visibility v;
                    if (p.current[i] > j)
                    {
                        v = ManaButton.Visibility.Full;
                    }
                    else if (p.max[i] > j)
                    {
                        v = ManaButton.Visibility.Faded;
                    }
                    else
                    {
                        v = ManaButton.Visibility.Hidden;
                    }
                    images[i][j].setVisibility(v);
                }
            }
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

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    images[i][j].Bounds = new Rectangle(j * w + padding, i * h + padding,  w - padding, h - padding);
                }
            }
        }
    }

    class ManaButton : Label, Clickable
    {
        public ManaColour orb { get; }
        public Visibility visibility;
        private float fade;
        private Image full, faded;

        public ManaButton(ManaColour orb, float fade)
        {
            this.fade = fade;
            this.orb = orb;
            Resize += (_, __) => renigra();

        }

        public void setVisibility(Visibility v)
        {
            this.memeout(() =>
            {
                visibility = v;
                switch (visibility)
                {
                    case Visibility.Full:
                    {
                        Visible = true;
                        Image = full;
                    }
                        break;

                    case Visibility.Faded:
                    {
                        Visible = true;
                        Image = faded;
                    }
                        break;

                    case Visibility.Hidden:
                    {
                        Visible = false;
                    }
                        break;
                }
            });
        }

        private void renigra()
        {
            full = G.ResizeImage(G.manaImage(orb), Size.Width, Size.Height);
            faded = G.SetImageOpacity(full, fade);
            setVisibility(visibility);
        }

        public Stuff getStuff()
        {
            return new ManaOrb(orb);
        }

        public enum Visibility
        {
            Full,
            Faded,
            Hidden
        }
    }
}
