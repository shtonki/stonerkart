using System;
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
    class ReduceResult<T>
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

    static class G
    {
        private static char[] hackish = G.range(0, 20).Select(v => (char)('\u2460' + v)).ToArray();

        private static char[] hackedaf =
            Enum.GetValues(typeof (ManaColour))
                .Cast<ManaColour>()
                .Select(c => (char)('\u24b6' + (c.ToString().ToLower())[0] - 97))
                .ToArray();

        public static ReduceResult<T> Reduce<T>(this IEnumerable<T> e)
        {
            return new ReduceResult<T>(e);
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

        public static char colourless(int i)
        {
            return hackish[i-1];
        }

        public static char coloured(ManaColour c)
        {
            return hackedaf[(int)c];
        }

        public static IEnumerable<int> range(int min, int max)
        {
            int[] r = new int[max - min];

            for (int i = 0; i < max - min; i++)
            {
                r[i] = min + i;
            }

            return r;
        }

        public static void clapTrap(object sender, UnhandledExceptionEventArgs e)
        {
            Form2 f = new Form2();
            AutoFontTextBox b = new AutoFontTextBox();
            b.Text = e.ExceptionObject.ToString();
            f.Controls.Add(b);
            f.Closed += (_, __) => Environment.Exit(2);
            f.Resize += (_, __) => b.Size = f.ClientSize;
            f.Size = new Size(600, 600);
            Application.Run(f);
        }

        private class Form2 : Form
        {
            
        }
    }
}
