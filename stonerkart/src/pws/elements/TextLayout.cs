using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace stonerkart
{
    class LaidText
    {
        public List<characterLayout> xs { get; }
        public FontFamille ff { get; }
        public int charHeight { get; }

        public LaidText(List<characterLayout> xs, FontFamille ff, int charHeight)
        {
            this.xs = xs;
            this.ff = ff;
            this.charHeight = charHeight;
        }

        public void draw(DrawerMaym dm, int yoffset, int xoffset, int maxWidth, Color textColor)
        {
            foreach (var xdd in xs)
            {
                var xp = xdd.xpos + xoffset;
                if (xp >= 0 && xp + xdd.width < maxWidth)
                {
                    dm.drawTexture(
                        ff.fontImage,
                        xdd.xpos + xoffset,
                        xdd.ypos + yoffset,
                        xdd.width,
                        charHeight,
                        xdd.crop, textColor);
                }
                else
                {
                    throw new Exception();
                }
            }
        }
    }

    abstract class TextLayout
    {
        private List<characterLayout> xs;
        private FontFamille ff;
        private double tw;
        private int charHeight;

        public abstract LaidText layout(string text, int width, int height, FontFamille ff);
    }

    public enum Justify
    {
        Left,
        Middle
    };

    class SingleLineFitLayout : TextLayout
    {
        private Justify justify;

        public SingleLineFitLayout() : this(Justify.Middle)
        {
        }

        public SingleLineFitLayout(Justify justify)
        {
            this.justify = justify;
        }

        public override LaidText layout(string text, int width, int height, FontFamille ff)
        {
            if (text == "") return new LaidText(new List<characterLayout>(), ff, height);

            List<characterLayout> xs = new List<characterLayout>();

            var sz = TextureLoader.sizeOf(ff.fontImage);
            double w = (double)sz.Width;
            double h = (double)sz.Height;

            var yscale = height/h;
            var scaledTextWidth = text.Sum(c => yscale*ff.characters[c.ToString()].width);
            int xpos = jstfy(width, scaledTextWidth);
            double ws = Math.Min((width - 1)/scaledTextWidth, 1);

            foreach (char c in text)
            {
                var v = ff.characters[c.ToString()];

                int rw = (int)(v.width*ws*yscale);
                //int rw = (int)Math.Min((v.width * (width-1) / tw), height);

                xs.Add(new characterLayout(xpos, 0, rw, new Box(v.startx / w, 0, v.width / w, 1)));
                xpos += rw;
            }
            if (xpos >= width) throw new Exception();
            return new LaidText(xs, ff, height);
        }
        private int jstfy(int width, double tw)
        {
            switch (justify)
            {
                case Justify.Left:
                {
                    return 0;
                }

                case Justify.Middle:
                {
                    if (tw < width)
                    {
                        return (int)((width - tw) / 2);
                    }
                    else
                    {
                        return 0;
                    }
                }

                default: throw new Exception();
            }
        }
    }

    

    class SingleLineLayout : TextLayout
    {
        public override LaidText layout(string text, int width, int height, FontFamille ff)
        {
            int xpos = 0;
            List<characterLayout> xlist = new List<characterLayout>();
            var sz = TextureLoader.sizeOf(ff.fontImage);
            double w = (double)sz.Width;
            double h = (double)sz.Height;

            double scale = ((double)height) / h;

            foreach (char c in text)
            {
                glyphxd v = ff.characters[c.ToString()];
                var charwidth = v.width * scale;
                xlist.Add(new characterLayout(xpos, 0, (int)charwidth, new Box(v.startx/w, 0, v.width/w, 1)));
                xpos += (int)charwidth;
            }

            return new LaidText(xlist, ff, height);
        }
    }

    class MultiLineFitLayout : TextLayout
    {
        private int minsize = 1;
        private int maxsize = Int32.MaxValue;

        public MultiLineFitLayout()
        {

        }

        public MultiLineFitLayout(int maxsize)
        {
            this.maxsize = maxsize;
        }

        public override LaidText layout(string text, int width, int height, FontFamille ff)
        {
            var words = text.Split(' ');

            var sz = TextureLoader.sizeOf(ff.fontImage);
            double w = sz.Width;
            double h = sz.Height;

            List<characterLayout> candidate = null;

            for (int fontheight = minsize; fontheight < maxsize; fontheight++)
            {
                int xpos = 0;
                int ypos = 0;

                List<characterLayout> xlist = new List<characterLayout>();
                double scale = ((double)fontheight) / h;

                foreach (string wrd in words)
                {
                    int wordwidth = 0;

                    foreach (char c in wrd)
                    {
                        glyphxd v = ff.characters[c.ToString()];
                        var charwidth = v.width * scale;
                        wordwidth += (int)charwidth;
                    }

                    if (xpos + wordwidth >= width)
                    {
                        xpos = 0;
                        ypos += fontheight;
                    }

                    if (ypos + fontheight >= height || wordwidth >= width)
                    {
                        return new LaidText(candidate, ff, fontheight - 1);
                    }

                    foreach (char c in wrd)
                    {
                        glyphxd v = ff.characters[c.ToString()];
                        var charwidth = v.width * scale;
                        xlist.Add(new characterLayout(xpos, ypos, (int)charwidth, new Box(v.startx / w, 0, v.width / w, 1)));
                        xpos += (int)charwidth;
                    }

                    glyphxd vx = ff.characters[" "];
                    var charwidthx = vx.width * scale;
                    xpos += (int)charwidthx;
                }

                candidate = xlist;
            }

            return new LaidText(candidate, ff, maxsize);
        }
    }

    public struct characterLayout
    {
        public int xpos;
        public int ypos;
        public int width;
        public Box crop;

        public characterLayout(int xpos, int ypos, int width, Box crop)
        {
            this.xpos = xpos;
            this.ypos = ypos;
            this.width = width;
            this.crop = crop;
        }
    }
}