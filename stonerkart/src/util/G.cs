using System;
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
    static class G
    {

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
