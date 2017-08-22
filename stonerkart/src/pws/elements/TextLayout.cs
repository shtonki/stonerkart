using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

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

        public void draw(DrawerMaym dm, int xoffset, int yoffset, int maxWidth, Color textColor)
        {
            foreach (var xdd in xs)
            {
                var xp = xdd.xpos + xoffset;
                if (xp >= 0 && xp + xdd.width < maxWidth)
                {
                    dm.drawTexture(
                        ff.fontImage,
                        xp,
                        xdd.ypos + yoffset,
                        xdd.width,
                        charHeight,
                        xdd.crop,
                        xdd.glyph[0] == '\\' ? Color.White : textColor);
                }
                else
                {
                    //throw new Exception();
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

        public LaidText Layout(string text, int width, int height, FontFamille ff)
        {
            return layout(chop(text), width, height, ff);
        }

        private string[] chop(string text)
        {
            List<string> glyphs = new List<string>();
            int i = 0;
            while (i < text.Length)
            {
                char c = text[i++];

                if (c == '\\')
                {
                    StringBuilder sb = new StringBuilder();
                    char ch;
                    do
                    {
                        ch = text[i++];
                        sb.Append(ch.ToString());
                    } while (ch != '\\');
                    glyphs.Add("\\" + sb.ToString());
                }
                else
                {
                    glyphs.Add(c.ToString());
                }
            }
            return glyphs.ToArray();
        }

        protected abstract LaidText layout(string[] text, int width, int height, FontFamille ff);
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

        protected override LaidText layout(string[] text, int width, int height, FontFamille ff)
        {
            if (text.Length == 0) return new LaidText(new List<characterLayout>(), ff, height);

            List<characterLayout> xs = new List<characterLayout>();

            var sz = TextureLoader.sizeOf(ff.fontImage);
            double w = (double)sz.Width;
            double h = (double)sz.Height;

            var yscale = height/h;
            var scaledTextWidth = text.Sum(c => yscale*ff.characters[c].width);
            int xpos = jstfy(width, scaledTextWidth);
            double ws = Math.Min((width - 1)/scaledTextWidth, 1);

            foreach (string c in text)
            {
                var v = ff.characters[c];

                int rw = (int)(v.width*ws*yscale);
                //int rw = (int)Math.Min((v.width * (width-1) / tw), height);

                xs.Add(new characterLayout(c, xpos, 0, rw, new Box(v.startx / w, 0, v.width / w, 1)));
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
        protected override LaidText layout(string[] text, int width, int height, FontFamille ff)
        {
            int xpos = 0;
            List<characterLayout> xlist = new List<characterLayout>();
            var sz = TextureLoader.sizeOf(ff.fontImage);
            double w = (double)sz.Width;
            double h = (double)sz.Height;

            double scale = ((double)height) / h;

            foreach (string c in text)
            {
                glyphxd v = ff.characters[c];
                var charwidth = v.width * scale;
                xlist.Add(new characterLayout(c, xpos, 0, (int)charwidth, new Box(v.startx/w, 0, v.width/w, 1)));
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

        private string[][] wordify(string[] text)
        {
            List<string[]> words = new List<string[]>();

            List<string> word = new List<string>();
            foreach (var v in text)
            {
                if (v == " " || v == G.newlineGlyph)
                {
                    words.Add(word.ToArray());
                    word = new List<string>();
                    if (v == G.newlineGlyph)
                    {
                        words.Add(new []{G.newlineGlyph});
                    }
                }
                else
                {
                    word.Add(v);
                }
            }
            return words.ToArray();
        }

        protected override LaidText layout(string[] text, int width, int height, FontFamille ff)
        {
            var words = wordify(text);

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

                foreach (string[] wrd in words)
                {
                    if (wrd.Length == 0) continue;
                    if (wrd[0] == G.newlineGlyph)
                    {
                        if (wrd.Length != 1) throw new Exception();
                        ypos += fontheight;
                        xpos = 0;
                        continue;
                    }

                    int wordwidth = 0;

                    foreach (string c in wrd)
                    {
                        glyphxd v = ff.characters[c];
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

                    foreach (string c in wrd)
                    {
                        glyphxd v = ff.characters[c];
                        var charwidth = v.width * scale;
                        xlist.Add(new characterLayout(c, xpos, ypos, (int)charwidth, new Box(v.startx / w, 0, v.width / w, 1)));
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
        public string glyph { get; }
        public int xpos { get; }
        public int ypos { get; }
        public int width { get; }
        public Box crop { get;}

        public characterLayout(string glyph, int xpos, int ypos, int width, Box crop)
        {
            this.glyph = glyph;
            this.xpos = xpos;
            this.ypos = ypos;
            this.width = width;
            this.crop = crop;
        }
    }
}