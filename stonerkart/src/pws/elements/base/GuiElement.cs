using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace stonerkart
{
    abstract class GuiElement
    {
        protected int x;
        protected int y;
        protected int width;
        protected int height;

        private bool focused;
        private bool selectable;
        private bool hoverable = true;
        private bool visible = true;

        public Screen screen { get; set; }

        public List<GuiElement> children { get; private set; }= new List<GuiElement>();
        public GuiElement parent { get; private set; }

        public int X
        {
            get { return x; }
            set { setLocation(value, y); }
        }

        public int Y
        {
            get { return y; }
            set { setLocation(x, value); }
        }

        public int AbsoluteX => parent == null ? X : X + parent.AbsoluteX;
        public int AbsoluteY => parent == null ? Y : Y + parent.AbsoluteY;
        public int Bottom => Y + Height;
        public int Right => X + Width;

        /// <summary>
        /// The order within the parent which the element is drawn, 0 being first;
        /// Set to -1 to make it drawn last
        /// </summary>
        public int DrawOrder
        {
            get
            {
                if (parent == null) return -1;
                return parent.children.IndexOf(this);
            }
            set
            {
                var p = parent;
                parent.removeChild(this);
                p.addChild(this, value);
            }
        }

        public virtual int Width
        {
            get { return width; }
            set { setSize(value, height); }
        }

        public virtual int Height
        {
            get { return height; }
            set { setSize(width, value); }
        }

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        public bool Hoverable
        {
            get { return hoverable && visible; }
            set { hoverable = value; }
        }

        public bool Selectable
        {
            get { return selectable && Hoverable; }
            set { selectable = value; }
        }

        public bool Focused
        {
            get { return focused; }
            set { focused = value; }
        }

        public GuiElement(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public GuiElement(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public void setLocation(int newx, int newy)
        {
            x = newx;
            y = newy;
        }

        public void setSize(int newwidth, int newheight)
        {
            resizeEventStruct args = new resizeEventStruct(newwidth, newheight, width, height);
            lock (this)
            {

                width = newwidth;
                height = newheight;
            }
            onResize(args);
        }

        public void addChild(GuiElement child, int index = -1)
        {
            index = index == -1 ? children.Count : index;
            if (child.parent != null) throw new Exception();
            if (children.Contains(child)) throw new Exception();
            children.Insert(index, child);
            child.parent = this;
        }

        public void removeChild(GuiElement child)
        {
            if (!children.Remove(child)) throw new Exception();
            child.parent = null;
        }

        public void clearChildren()
        {
            while (children.Count > 0)
            {
                removeChild(children[0]);
            }
        }

        public bool focus()
        {
            if (Selectable) Focused = true;
            return Selectable;
        }

        public void unfocus()
        {
            Focused = false;
        }

        public void moveTo(MoveTo xPlacement, int yVal)
        {
            moveTo(xPlacement, MoveTo.Nowhere);
            Y = yVal;
        }

        public void moveTo(int xVal, MoveTo yPlacement)
        {
            moveTo(MoveTo.Nowhere, yPlacement);
            X = xVal;
        }

        public void moveTo(MoveTo xPlacement, MoveTo yPlacement)
        {
            int px, py, pw, ph;
            if (parent == null)
            {
                px = py = 0;
                pw = Frame.BACKSCREENWIDTH;
                ph = Frame.BACKSCREENHEIGHT;
            }
            else
            {
                px = parent.X;
                py = parent.Y;
                pw = parent.Width;
                ph = parent.Height;
            }

            X = koenDontKillMe(xPlacement, X, Width, px, pw);
            Y = koenDontKillMe(yPlacement, Y, Height, py, ph);
        }
        
        private int koenDontKillMe(MoveTo mt, int thisLocation, int thisSize, int parentLocation, int parentSize)
        {
            switch (mt)
            {
                case MoveTo.Center:
                {
                    return (parentSize - thisSize) / 2;
                }

                case MoveTo.Nowhere:
                {
                    return thisLocation;
                }

                case MoveTo.Bottom:
                {
                    return parentSize - thisSize;
                }

                default:
                    throw new Exception();
            }
        }

        public void Draw(DrawerMaym dm)
        {
            lock (this)
            {
                draw(dm);
            }
        }

        protected abstract void draw(DrawerMaym dm);

        protected bool pressed;

        public virtual void onClick(MouseButtonEventArgs args)
        {
            clicked?.Invoke(args);
        }

        public virtual void onMouseDown(MouseButtonEventArgs args)
        {
            mouseDown?.Invoke(args);
            pressed = true;
        }

        public virtual void onMouseUp(MouseButtonEventArgs args)
        {
            mouseUp?.Invoke(args);
            if (pressed)
            {
                onClick(args);
            }
            pressed = false;
        }

        public virtual void onMouseExit(MouseMoveEventArgs args)
        {
            mouseExit?.Invoke(args);
            pressed = false;
        }

        public virtual void onKeyDown(KeyboardKeyEventArgs args)
        {
            keyDown?.Invoke(args);
        }

        public virtual void onMouseEnter(MouseMoveEventArgs args)
        {
            mouseEnter?.Invoke(args);
        }

        public virtual void onMouseMove(MouseMoveEventArgs args)
        {
            mouseMove?.Invoke(args);
        }

        public virtual void onResize(resizeEventStruct args)
        {
            resize?.Invoke(args);
        }

        public delegate void  mouseClickEventHandler(MouseButtonEventArgs args);
        public delegate void mouseMoveEventHandler(MouseMoveEventArgs args);
        public delegate void keyboardEvent(KeyboardKeyEventArgs args);
        public delegate void resizeEvent(resizeEventStruct args);

        public event mouseClickEventHandler mouseDown;
        public event mouseClickEventHandler mouseUp;
        public event mouseMoveEventHandler mouseExit;
        public event mouseMoveEventHandler mouseMove;
        public event mouseMoveEventHandler mouseEnter;
        public event mouseClickEventHandler clicked;
        public event keyboardEvent keyDown;
        public event resizeEvent resize;
    }

    public struct resizeEventStruct
    {
        public int newHeight;
        public int newWidth;
        public int oldHeight;
        public int oldWidth;

        public resizeEventStruct(int newWidth, int newHeight, int oldWidth, int oldHeight)
        {
            this.newHeight = newHeight;
            this.newWidth = newWidth;
            this.oldHeight = oldHeight;
            this.oldWidth = oldWidth;
        }
    }

    public enum MoveTo
    {
        Center,
        Nowhere,
        Bottom,
    }
}
