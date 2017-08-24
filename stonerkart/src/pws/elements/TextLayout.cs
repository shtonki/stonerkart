using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace stonerkart
{
    class LaidText
    {
        public List<charLayout> characters { get; }
        public int charHeight { get; }

        public LaidText(List<charLayout> characters, int charHeight)
        {
            this.characters = characters;
            this.charHeight = charHeight;
        }

        public void draw(DrawerMaym dm, int x, int y, int maxWidth, Color textColor, bool disableClipping = false)
        {
            foreach (var character in characters)
            {
                var xp = character.xpos + x;
                if (disableClipping || xp >= 0 && xp + character.width < maxWidth)
                {
                    character.draw(dm, x, y, textColor);
                    /*
                    dm.drawTexture(
                        ff.fontImage,
                        xp,
                        character.ypos + yoffset,
                        character.width,
                        charHeight,
                        character.crop,
                        character.glyph[0] == '\\' ? Color.White : textColor);
                        */
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

        protected charLayout makeCharLayout(string glyph, int xpos, int ypos, int width, int height, FontFamille ff)
        {
            if (glyph.Length == 1)
            {
                return new charLayout(ff[glyph], glyph, xpos, ypos, width, height);
            }
            else
            {
                return new charLayout(ff[glyph], glyph, xpos, ypos, height, height);
            }
            throw new Exception();/*
            return new fontGlyphLayout(glyph, xpos, ypos, width, height, crop, ff);
            throw new Exception();*/
        }
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
            if (text.Length == 0) return new LaidText(new List<charLayout>(), height);

            List<charLayout> xs = new List<charLayout>();

            double h = ff.Height;

            var yscale = height/h;
            var scaledTextWidth = text.Sum(c => yscale*ff.widthOf(c));
            int xpos = jstfy(width, scaledTextWidth);
            double ws = Math.Min((width - 1)/scaledTextWidth, 1);

            foreach (string c in text)
            {
                int rw = (int)(ff.widthOf(c)*ws*yscale);
                xs.Add(makeCharLayout(c, xpos, 0, rw, height, ff));
                xpos += rw;
            }
            if (xpos >= width) throw new Exception();
            return new LaidText(xs, height);
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
            List<charLayout> xlist = new List<charLayout>();
            double h = (double)ff.Height;

            double scale = ((double)height) / h;

            foreach (string c in text)
            {
                var charwidth = ff.widthOf(c) * scale;
                xlist.Add(makeCharLayout(c, xpos, 0, (int)charwidth, height, ff));
                xpos += (int)charwidth;
            }

            return new LaidText(xlist, height);
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
            if (word.Count > 0) words.Add(word.ToArray());
            return words.ToArray();
        }

        protected override LaidText layout(string[] text, int width, int height, FontFamille ff)
        {
            var words = wordify(text);

            var sz = TextureLoader.sizeOf(ff.fontImage);
            double w = sz.Width;
            double h = sz.Height;

            List<charLayout> candidate = null;

            for (int fontheight = minsize; fontheight < maxsize; fontheight++)
            {
                int xpos = 0;
                int ypos = 0;

                List<charLayout> xlist = new List<charLayout>();
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
                        var charwidth = ff.widthOf(c) * scale;
                        wordwidth += (int)charwidth;
                    }

                    if (xpos + wordwidth >= width)
                    {
                        xpos = 0;
                        ypos += fontheight;
                    }

                    if (ypos + fontheight >= height || wordwidth >= width)
                    {
                        return new LaidText(candidate, fontheight - 1);
                    }

                    foreach (string c in wrd)
                    {
                        var v = ff[c];
                        var charwidth = ff.widthOf(c) * scale;
                        xlist.Add(makeCharLayout(c, xpos, ypos, (int)charwidth, fontheight, ff));
                        xpos += (int)charwidth;
                    }

                    xpos += (int)(ff.widthOf(" ")*scale);
                }

                candidate = xlist;
            }

            return new LaidText(candidate, maxsize);
        }
    }

    class charLayout
    {
        public string glyph { get; }
        public int xpos { get; }
        public int ypos { get; }
        public int width { get; }
        public int height { get; }
        private Imege imege;

        public charLayout(Imege imege, string glyph, int xpos, int ypos, int width, int height)
        {
            this.imege = imege;
            this.glyph = glyph;
            this.xpos = xpos;
            this.ypos = ypos;
            this.width = width;
            this.height = height;
        }

        public void draw(DrawerMaym dm, int xoffset, int yoffset, Color textColor)
        {
            dm.drawImegeForceColour(
                imege,
                xpos + xoffset,
                ypos + yoffset,
                width,
                height,
                glyph.Length > 1 ? Color.White : textColor
                );
        }
    }
}