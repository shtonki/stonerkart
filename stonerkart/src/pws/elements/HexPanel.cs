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
        private int xcount;
        private int ycount;
        private int hexsize;

        public HexPanel(int xcount, int ycount, int hexsize) : base()
        {
            Backcolor = Color.Aqua;
            width = hexsize+(int)((xcount-1)*hexsize*0.75);
            height = ycount*hexsize + hexsize/2;

            this.xcount = xcount;
            this.ycount = ycount;
            this.hexsize = hexsize;
        }

        public override void onMouseDown(MouseButtonEventArgs args)
        {
            base.onMouseDown(args);

            var v = findHexagon(args.Position);
            if (v != null)
            {
                System.Console.WriteLine("{0} {1}", v.Item1, v.Item2);
            }
        }

        private Tuple<int, int> findHexagon(Point p)
        {
            var mousex = p.X;
            var mousey = p.Y;


            int cx = 0;
            int cy = 0;
            double cd = 2000;

            for (int i = 0; i < xcount; i++)
            {
                int os = (i % 2) * hexsize / 2;
                for (int j = 0; j < ycount; j++)
                {
                    int hexX = x + (int)(0.75 * hexsize * i) + hexsize / 2;
                    int hexY = y + (j * hexsize + os) + hexsize / 2;

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

        protected override void draw(DrawerMaym dm)
        {
            base.draw(dm);

            for (int i = 0; i < xcount; i++)
            {
                int os = (i%2)*hexsize/2;
                for (int j = 0; j < ycount; j++)
                {
                    int hexX = (int)(0.75*hexsize*i);
                    int hexY = j*hexsize + os;

                    if (i%2 == 0)
                    {
                        dm.fillHexagon(hexX, hexY, hexsize, Color.Black, Textures.artDaringPoppy);

                    }
                    else
                    {
                        dm.fillHexagon(hexX, hexY, hexsize, Color.Black, Color.AntiqueWhite);
                    }
                }
            }

        }
    }
}
