using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Winduh : Square
    {
        private GuiElement content;
        private Square heading;
        private Square closeButton;

        private const int headingHeight = 30;

        private Point? headingClick;

        public string Title
        {
            get { return heading.Text; }
            set { heading.Text = value; }
        }

        public Winduh(GuiElement content, bool closeable = false, bool persistent = false) : this(content, "", closeable, persistent)
        {
            
        }

        public Winduh(GuiElement content, string title, bool closeable, bool persistent)
        {
            this.content = content;
            setSize(content.Width, content.Height + headingHeight);
            addChild(content);
            content.X = 0;
            content.Y = headingHeight;

            int closeButtonSize = closeable ? headingHeight : 0;

            if (closeable)
            {
                closeButton = new Button(closeButtonSize, closeButtonSize);
                addChild(closeButton);
                closeButton.Text = "x";
                closeButton.Backcolor = Color.Maroon;
                closeButton.Border = new SolidBorder(2, Color.Black);
                closeButton.X = Width - closeButtonSize;
                if (persistent)
                {
                    closeButton.clicked += a => Visible = false;
                }
                else
                {
                    closeButton.clicked += a =>
                    {
                        close();
                    };
                }
            }

            heading = new Square(content.Width - closeButtonSize, headingHeight);
            addChild(heading);
            heading.Backcolor = Color.Tomato;
            Title = title;

            heading.mouseDown += a =>
            {
                headingClick = a.Position;
            };

            heading.mouseMove += a =>
            {
                if (headingClick.HasValue)
                {
                    int dx = a.X - headingClick.Value.X;
                    int dy = a.Y - headingClick.Value.Y;
                    X += dx;
                    Y += dy;
                    headingClick = a.Position;
                }
            };

            heading.mouseUp += a =>
            {
                headingClick = null;
            };
        }


        public void close()
        {
            if (parent != null)
            {
                parent.removeChild(this);
            }
            else
            {
                Screen.removeElement(this);
            }
        }
    }
}
