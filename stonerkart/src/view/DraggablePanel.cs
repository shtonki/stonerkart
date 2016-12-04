using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart
{
    public class DraggablePanel : Panel
    {
        public bool closed;

        private Panel topPanel;
        private Panel resizeButton;
        private Button closeButton;
        private Control content;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        public DraggablePanel(Control content, bool resizeable = true, bool closeable = true)
        {
            topPanel = new Panel();
            topPanel.BackColor = Color.Chocolate;
            Controls.Add(topPanel);

            resizeButton = new Panel();
            resizeButton.Size = new Size(5, 5);
            resizeButton.BackColor = Color.Black;
            resizeButton.Visible = resizeable;
            Controls.Add(resizeButton);

            closeButton = new Button();
            closeButton.Size = new Size(20, 20);
            closeButton.Visible = closeable;
            closeButton.BackColor = Color.DarkRed;

            topPanel.Controls.Add(closeButton);

            this.content = content;
            Controls.Add(content);

            topPanel.MouseMove += (_, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            };

            resizeButton.MouseMove += (_, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    Size = new Size(Size.Width + e.X, Size.Height + e.Y);
                }
            };

            closeButton.MouseClick += (_, __) => close();

            Resize += (_, __) => layoutShit();

            layoutShit();
        }

        public void close()
        {
            Parent.Controls.Remove(this);
            closed = true;
        }

        private void layoutShit()
        {
            topPanel.Size = new Size(Size.Width, 20);
            topPanel.Location = new Point(0, 0);

            resizeButton.Location = new Point(Size.Width - 5, Size.Height - 5);

            closeButton.Location = new Point(Size.Width - 20, 0);

            content.SetBounds(0, 20, Size.Width, Size.Height - 20);
        }
    }
}
