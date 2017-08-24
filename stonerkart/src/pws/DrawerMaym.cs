using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;


namespace stonerkart
{
    class DrawerMaym
    {
        private Dictionary<Textures, int> textures;

        public DrawerMaym(Dictionary<Textures, int> textures)
        {
            this.textures = textures;
        }

        public void translate(int x, int y)
        {
            var tx = ((float)x)/Frame.BACKSCREENWIDTHd2;
            var ty = ((float)y)/Frame.BACKSCREENHEIGHTd2;
            GL.Translate(tx, -ty, 0);
        }

        public void fillRectange(Color c, int x, int y, int width, int height)
        {
            fillRectangeR(c, new Box(x, y, width, height));
        }

        private void fillRectangeR(Color c, Box b)
        {
            GL.Begin(PrimitiveType.Quads);

            GL.Color4(c);

            GL.Vertex2(b.x, -b.y);
            GL.Vertex2(b.r, -b.y);
            GL.Vertex2(b.r, -b.b);
            GL.Vertex2(b.x, -b.b);

            GL.End();
        }

        public void fillHexagon(int x, int y, int size, Color border, Textures texture)
        {
            fillHexagonR(x, y, size, border, null, texture);
        }

        public void fillHexagon(int x, int y, int size, Color border, Color centre)
        {
            fillHexagonR(x, y, size, border, centre, null);
        }

        private void fillHexagonR(int x, int y, int size, Color border, Color? centre, Textures? t)
        {
            Box b = new Box(
                x,
                y + size,
                size, 
                size
                );

            if (t.HasValue)
            {
                var tx = t.Value;

                GL.Enable(EnableCap.Texture2D);
                GL.Color4(Color.White);
                GL.BindTexture(TextureTarget.Texture2D, textures[tx]);
                GL.Begin(BeginMode.Polygon);

                GL.TexCoord2(0, 0.5);
                GL.Vertex2(b.x, -b.y + b.h/2);

                GL.TexCoord2(0.25, 1);
                GL.Vertex2(b.x + b.w/4, -b.y);

                GL.TexCoord2(0.75, 1);
                GL.Vertex2(b.x + b.w - b.w/4, -b.y);

                GL.TexCoord2(1, 0.5);
                GL.Vertex2(b.x + b.w, -b.y + b.h/2);

                GL.TexCoord2(0.75, 0);
                GL.Vertex2(b.x + b.w - b.w/4, -b.y + b.h);

                GL.TexCoord2(0.25, 0);
                GL.Vertex2(b.x + b.w/4, -b.y + b.h);

                GL.TexCoord2(0, 0.5);
                GL.Vertex2(b.x, -b.y + b.h/2);

                GL.End();
                GL.Disable(EnableCap.Texture2D);
            }
            else if (centre.HasValue)
            {
                GL.Color4(centre.Value);
                GL.Begin(BeginMode.Polygon);

                GL.Vertex2(b.x, -b.y + b.h/2);
                GL.Vertex2(b.x + b.w/4, -b.y);
                GL.Vertex2(b.x + b.w - b.w/4, -b.y);
                GL.Vertex2(b.x + b.w, -b.y + b.h/2);
                GL.Vertex2(b.x + b.w - b.w/4, -b.y + b.h);
                GL.Vertex2(b.x + b.w/4, -b.y + b.h);
                GL.Vertex2(b.x, -b.y + b.h/2);

                GL.End();
            }
            else throw new Exception();

            GL.LineWidth(4);
            GL.Color4(border);
            GL.Begin(BeginMode.LineLoop);

            GL.Vertex2(b.x                , -b.y + b.h / 2);
            GL.Vertex2(b.x + b.w / 4      , -b.y);
            GL.Vertex2(b.x + b.w - b.w / 4, -b.y);
            GL.Vertex2(b.x + b.w          , -b.y + b.h / 2);
            GL.Vertex2(b.x + b.w - b.w / 4, -b.y + b.h);
            GL.Vertex2(b.x + b.w / 4      , -b.y + b.h);
            GL.Vertex2(b.x                , -b.y + b.h / 2);

            GL.End();

        }

        public void drawImegeForceColour(Imege i, int x, int y, int width, int height, Color c)
        {
            drawTextureR(i.texture, new Box(x, y, width, height), i.crop, c);
        }

        public void drawImege(Imege i, int x, int y, int width, int height)
        {
            drawTextureR(i.texture, new Box(x, y, width, height), i.crop, i.brushColor);
        }

        private void drawTextureR(Textures tx, Box imageLocation, Box crop, Color color)
        {

            GL.Enable(EnableCap.Texture2D);
            GL.Color4(color);
            GL.BindTexture(TextureTarget.Texture2D, textures[tx]);
            GL.Begin(PrimitiveType.Quads);
            
            GL.TexCoord2(crop.x, crop.y);
            GL.Vertex2(imageLocation.x, -imageLocation.y);

            GL.TexCoord2(crop.r, crop.y);
            GL.Vertex2(imageLocation.r, -imageLocation.y);

            GL.TexCoord2(crop.r, crop.b);
            GL.Vertex2(imageLocation.r, -imageLocation.b);

            GL.TexCoord2(crop.x, crop.b);
            GL.Vertex2(imageLocation.x, -imageLocation.b);
            
            GL.End();
            GL.Disable(EnableCap.Texture2D);
        }
    }
    
    public struct Box
    {
        public double x { get; }
        public double y { get; }
        public double w { get; }
        public double h { get; }
        public double r { get; }
        public double b { get; }

        public Box(double x, double y, double w, double h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
            r = x + w;
            b = y + h;
        }

        public Box(PointF topLeft, PointF bottomRight) : this()
        {
            x = topLeft.X;
            y = topLeft.Y;
            r = bottomRight.X;
            b = bottomRight.Y;
            w = r - x;
            h = b - y;
        }

        public Box(int x, int y, int w, int h) : this(Frame.pixToGL(x, y), Frame.pixToGL(x + w, y + h))
        {
        }

        /*
        public static Box boxifyScreen(int X, int Y, int w, int h)
        {
            var p1 = Frame.pixToGL(new Point(X, Y));
            var p2 = Frame.pixToGL(new Point(X+w, Y+h));
            return new Box(p1, p2);
            //return boxify(X, Y, w, h, Frame.BACKSCREENWIDTHd2, Frame.BACKSCREENHEIGHTd2);
        }
        /*
        public static Box boxify(int X, int Y, int w, int h, int swd2, int shd2)
        {
            var sx = ((double)(X - swd2)) / swd2;
            var sy = ((double)(Y - shd2)) / shd2;
            var sw = ((double)(w - 0)) / swd2;
            var sh = ((double)(h - 0)) / shd2;

            return new Box(
                sx,
                sy,
                sw,
                sh
                );
        }
        */
    }
}
