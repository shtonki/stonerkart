﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart
{
    public class ReduceResult<T>
    {
        public IEnumerable<T> values => d.Keys;

        public int this[T t]    // Indexer declaration  
        {
            get { return d[t]; }
        }

        private Dictionary<T, int> d = new Dictionary<T, int>();

        public ReduceResult(IEnumerable<T> e)
        {
            foreach (var t in e)
            {
                if (!d.ContainsKey(t)) d[t] = 0;
                d[t] = d[t] + 1;
            }
        }
    }

    public static class G
    {
        public static Tuple<CardSet, Rarity> fuckInternalHack(CardTemplate ct)
        {
            Card c = new Card(ct);
            return new Tuple<CardSet, Rarity>(c.set, c.rarity);
        }

        public static List<ManaColour> orbOrder = new List<ManaColour>(new[]
        {
            ManaColour.Chaos,
            ManaColour.Death,
            ManaColour.Might,
            ManaColour.Order,
            ManaColour.Life,
            ManaColour.Nature,
            ManaColour.Colourless,
        });

        public static string shekelsToString(int shekelCount)
        {
            int bigs = shekelCount/100;
            int smalls = shekelCount%100;
            return bigs + "." + smalls.ToString().PadLeft(2, '0');
        }

        public static string replaceUnderscoresAndShit(string input)
        {
            return input.Replace("_a", "'").Replace("_s", " ");;
        }

        /*
        private static char[] hackish = G.range(0, 20).Select(v => (char)('\u2460' + v)).ToArray();

        private static char[] hackedaf =
            Enum.GetValues(typeof (ManaColour))
                .Cast<ManaColour>()
                .Select(c => (char)('\u24b6' + (c.ToString().ToLower())[0] - 97))
                .ToArray();
        */

        public static ReduceResult<T> Reduce<T>(this IEnumerable<T> e)
        {
            return new ReduceResult<T>(e);
        }

        public static T[] Memesort<T>(this IEnumerable<T> es, Func<T, T, int> cmp)
        {
            T[] tl = es.ToArray();
            Maymsort<T>(tl, 0, tl.Length - 1, cmp);
            return tl;
        }

        public static void Maymsort<T>(T[] es, int left, int right, Func<T, T, int> cmp)
        {
            if (left >= right) return;

            T pivot = es[left];

            int i = left - 1;
            int j = right + 1;
            int r;
            while (true)
            {
                do
                {
                    i++;
                } while (cmp(es[i], pivot) < 0);

                do
                {
                    j--;
                } while (cmp(pivot, es[j]) < 0);

                if (i >= j)
                {
                    r = j;
                    break;
                }

                T io = es[i];
                T jo = es[j];

                T tmp = es[j];
                es[j] = es[i];
                es[i] = tmp;

                if (cmp(es[i], jo) != 0 || cmp(es[j], io) != 0)
                {
                    //throw new Exception();
                }
            }

            Maymsort<T>(es, left, r, cmp);
            Maymsort<T>(es, r+1, right, cmp);
        }

        public static void Quicksort<T>(T[] elements, int left, int right, Func<T, T, int> cmp)
        {
            if (left >= right) return;

            int i = left, j = right;
            T pivot = elements[left + (right - left) / 2];

            while (i <= j)
            {
                while (cmp(elements[i],pivot) < 0)
                {
                    i++;
                }

                while (cmp(elements[j],pivot) > 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    // Swap
                    T tmp = elements[i];
                    elements[i] = elements[j];
                    elements[j] = tmp;

                    i++;
                    j--;
                }
            }

            // Recursive calls
            if (left < j)
            {
                Quicksort(elements, left, j, cmp);
            }

            if (i < right)
            {
                Quicksort(elements, i, right, cmp);
            }
        }

        private static Dictionary<Image, Bitmap> imageCache = new Dictionary<Image, Bitmap>();

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            Size size = new Size(width, height);
            if (imageCache.ContainsKey(image))
            {
                Bitmap bmp = imageCache[image];
                if (size.Width == bmp.Width && size.Height == bmp.Height)
                {
                    return bmp;
                }
            }
            if (width <= 0 || height <= 0) return new Bitmap(image);
            return new Bitmap(image, new Size(width, height));
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            imageCache[image] = destImage;
            return destImage;

    }

        public static Image SetImageOpacity(Image image, float opacity)
        {
            //create a Bitmap the size of the image provided  
            Bitmap bmp = new Bitmap(image.Width, image.Height);

            //create a graphics object from the image  
            using (Graphics gfx = Graphics.FromImage(bmp))
            {

                //create a color matrix object  
                ColorMatrix matrix = new ColorMatrix();

                //set the opacity  
                matrix.Matrix33 = opacity;

                //create image attributes  
                ImageAttributes attributes = new ImageAttributes();

                //set the color(opacity) of the image  
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                //now draw the image  
                gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
            }
            return bmp;
        }

        public static string colourlessGlyph(int i)
        {
            return "\\cl" + i + "\\";
        }

        public static string colouredGlyph(ManaColour c)
        {
            return "\\" + c + "\\";
        }

        public static string newlineGlyph => "\\n\\";

        public static string exhaustGhyph => "Exhaust";

        public const string channelOnly = "Use this only when you could cast a Channel.";

        public static IEnumerable<int> range(int min, int max)
        {
            int[] r = new int[max - min];

            for (int i = 0; i < max - min; i++)
            {
                r[i] = min + i;
            }

            return r;
        }
    }

}
