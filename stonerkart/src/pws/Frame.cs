﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using stonerkart.Properties;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace stonerkart
{
    class Frame : GameWindow
    {
        public Screen activeScreen { get; private set; }
        private IEnumerable<GuiElement> activeElements => activeScreen.Elements.Concat(frameElements);

        private ManualResetEventSlim loadre;

        private Designer designer;

        #region FrameElements
        private List<GuiElement> frameElements;
        public GameMenuBar menu { get; private set; }
        public FriendsPanel friendsPanel { get; private set; }
        public Winduh friendsWinduh { get; private set; }
        public PendingFriendsPanel addFriendsPanel { get; private set; }
        public Winduh addFriendsWinduh { get; private set; }

        private void generateFrameElements()
        {
            frameElements = new List<GuiElement>();

            menu = new GameMenuBar();
            frameElements.Add(menu);
            menu.Visible = false;
            menu.showFriendsButton.clicked += a =>
            {
                friendsWinduh.Visible = !friendsWinduh.Visible;
            };
            menu.addFriendsButton.clicked += a =>
            {
                addFriendsWinduh.Visible = !addFriendsWinduh.Visible;
            };

            friendsPanel = new FriendsPanel(300, 500);
            friendsWinduh = new Winduh(friendsPanel, "Friends", true, true);
            friendsWinduh.Backimege = new MemeImege(Textures.buttonbg2, 6845324);
            friendsWinduh.Visible = false;
            frameElements.Add(friendsWinduh);


            addFriendsPanel = new PendingFriendsPanel(300, 500);
            addFriendsWinduh = new Winduh(addFriendsPanel, "Friend Requests", true, true);
            addFriendsWinduh.Backimege = new MemeImege(Textures.buttonbg2, 6845324);
            addFriendsWinduh.Visible = false;
            frameElements.Add(addFriendsWinduh);
        }

        #endregion


        public Frame(int width, int height, ManualResetEventSlim ld = null, bool design = false) : base(width, height, new GraphicsMode(32,24,0,32), "StonerKart")
        {
            loadre = ld;

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            if (design)
            {
                Thread t = new Thread(() =>
                {
                    designer = new Designer();
                    Application.Run(designer);
                });

                t.Start();
            }

            generateFrameElements();
        }

        public void setScreen(Screen screen)
        {
            activeScreen = screen;
        }

        public void loginAs(User user)
        {
            menu.setFlare(user);
            menu.Visible = true;
            user.addObserver(friendsPanel);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            loadTextures();
            if (loadre != null) loadre.Set();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }

        class fpscounter
        {
            private double[] buf;
            private int ctr;

            public fpscounter(int bufsize)
            {
                buf = new double[bufsize];
            }

            public void addVal(double v)
            {
                ctr = (ctr + 1)%buf.Length;
                buf[ctr] = v;
            }

            public double getVal()
            {
                return buf.Sum()/buf.Count();
            }
        }

        fpscounter ctr = new fpscounter(30);

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            ctr.addVal(RenderFrequency);

            Title = ctr.getVal().ToString();
               
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(Color.CornflowerBlue);
            GL.PushMatrix();


            if (activeScreen != null)
            {
                DrawerMaym dm = new DrawerMaym(textures);

                if (activeScreen.background != null) dm.drawImege(activeScreen.background, 0, 0, BACKSCREENWIDTH, BACKSCREENHEIGHT);

                lock (activeScreen.elements)
                {
                    foreach (var elem in activeElements)
                    {
                        drawElement(elem, dm);
                    }
                }
            }

            this.SwapBuffers();
            GL.PopMatrix();
        }

        private void drawElement(GuiElement ge, DrawerMaym dm)
        {
            if (ge == null) return;
            if (!ge.Visible) return;

            dm.translate(ge.X, ge.Y);

            ge.Draw(dm);

            var threadHack = ge.children.ToArray();

            foreach (var kid in threadHack)
            {
                drawElement(kid, dm);
            }

            dm.translate(-ge.X, -ge.Y);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (hovered != null && !hovered.Selectable) OnMouseMove(new MouseMoveEventArgs(e.X, e.Y, 0, 0));
            e.Position = scalePoint(e.Position);
            focus(hovered);
            hovered?.onMouseDown(e);
            designer?.setActive(hovered);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (hovered != null && !hovered.Selectable) OnMouseMove(new MouseMoveEventArgs(e.X, e.Y, 0, 0));
            e.Position = scalePoint(e.Position);

            hovered?.onMouseUp(e);
        }

        private GuiElement hovered;
        private GuiElement focused;

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            e.Position = scalePoint(e.Position);
            var newover = elementAt(e.Position);

            if (newover != hovered)
            {
                hovered?.onMouseExit(e);
                newover?.onMouseEnter(e);
                hovered = newover;
            }
            else
            {
                hovered?.onMouseMove(e);
            }
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            focused?.onKeyDown(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Controller.quit();
        }

        private void focus(GuiElement f)
        {
            if (f == null) return;
            if (f.focus())
            {
                focused?.unfocus();
                focused = f;
            }
        }

        private GuiElement elementAt(int x, int y, GuiElement[] l)
        {
            foreach (var v in l.Reverse())
            {
                if (v.Hoverable &&
                    v.X < x &&
                    v.X + v.Width > x &&
                    v.Y < y &&
                    v.Y + v.Height > y)
                {
                    var nl = v.children.ToArray();
                    var r = elementAt(x - v.X, y - v.Y, nl);
                    return r ?? v;
                }
            }

            return null;
        }

        private GuiElement elementAt(Point sp)
        {
            if (activeScreen == null) return null;

            int x = sp.X;
            int y = sp.Y;
            var l = activeElements.ToArray();

            return elementAt(x, y, l);
        }


        public static PointF pixToGL(int x, int y)
        {
            return pixToGL(new Point(x, y));
        }

        public static PointF pixToGL(Point p)
        {
            var sx = ((float)(p.X - BACKSCREENWIDTHd2)) / BACKSCREENWIDTHd2;
            var sy = ((float)(p.Y - BACKSCREENHEIGHTd2)) / BACKSCREENHEIGHTd2;

            return new PointF(sx, sy);
        }

        public const int BACKSCREENWIDTH = 1920;
        public const int BACKSCREENHEIGHT = 1080;
        public const int BACKSCREENWIDTHd2 = BACKSCREENWIDTH/2;
        public const int BACKSCREENHEIGHTd2 = BACKSCREENHEIGHT/2;
        public const int MENUHEIGHT = 50;
        public const int AVAILABLEHEIGHT = BACKSCREENHEIGHT - MENUHEIGHT;

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
        }

        private Point scalePoint(Point p)
        {
            double xs = ((double)BACKSCREENWIDTH) / Width;
            double ys = ((double)BACKSCREENHEIGHT) / Height;

            return new Point((int)Math.Round(p.X*xs), (int)Math.Round(p.Y*ys));
        }

        private static Dictionary<Textures, int> textures;

        private static void loadTextures()
        {
            if (textures != null) throw new Exception();
            textures = new Dictionary<Textures, int>();

            foreach (Textures t in Enum.GetValues(typeof (Textures)))
            {
                loadTexture(t, TextureLoader.images[t]);
            }
        }

        private static void loadTexture(Textures t, Image i)
        {
            if (textures.ContainsKey(t)) throw new Exception("Reloading existing texture");
            textures[t] = makeTexture(i);
        }

        private static int makeTexture(Image image)
        {
            int id = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, id);

            Bitmap bmp = new Bitmap(image);
            BitmapData data = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bmp.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

            return id;
        }

    }
}
