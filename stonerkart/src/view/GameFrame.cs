using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace stonerkart
{
    

    class GameFrame : Form
    {
        public Screen mainMenuPanel { get; private set; }
        public Screen loginPanel { get; private set; }
        public GamePanel gamePanel;
        private StickyPanel mainPanel;
        public MenuBar menuBar1;

        public Screen mapEditorScreen { get; private set; }
        public Screen deckEditorScreen { get; private set; }
        public Control active;
        

        public GameFrame()
        {
            InitializeComponent();

            mainMenuPanel = new MainMenuPanel();
            mapEditorScreen = new MapEditor();
            deckEditorScreen = new DeckEditorPanel();
            loginPanel = new LoginScreen();
            Closed += (_, __) => Controller.quit();
            Resize += (_, __) =>
            {
                int w = ClientSize.Width;
                int h = ClientSize.Height;
                menuBar1.Width = w;
                menuBar1.Location = new Point(0, h - menuBar1.Height);
                mainPanel.Size = new Size(w, h - menuBar1.Height);
            };
        }

        public DraggablePanel showPanel(Control content, bool resizeable = true, bool closeable = true)
        {
            DraggablePanel r = new DraggablePanel(content, resizeable, closeable);
            this.memeout(() =>
            {
                Controls.Add(r);
                r.BringToFront();
            });
            return r;
        }

        public void setPrompt(string message, ButtonOption[] buttons)
        {
            this.memeout(() =>
            {
                gamePanel.promtText.Text = message;
                Shibbutton[] bs = new[] {
                    gamePanel.shibbutton2,
                    gamePanel.shibbutton3,
                    gamePanel.shibbutton4,
                    gamePanel.shibbutton5,
                };
                for (int i = 0; i < bs.Length; i++)
                {
                    if (buttons.Length > i && buttons[i] != ButtonOption.NOTHING)
                    {
                        bs[i].Visible = true;
                        bs[i].setOption(buttons[i]);
                    }
                    else
                    {
                        bs[i].Visible = false;
                    }
                }
                gamePanel.Invalidate();
            });
        }

        public void toGame(Game g)
        {
            gamePanel = new GamePanel(g);
            transitionTo(gamePanel);
        }

        public void transitionTo(Screen s)
        {
            this.memeout(() =>
            {
                if (active != null)
                {
                    mainPanel.Controls.Remove(active);
                }
                active = (Control)s;
                active.Dock = DockStyle.Fill;
                mainPanel.Controls.Add(active);
            });
        }

        private void InitializeComponent()
        {
            this.mainPanel = new stonerkart.StickyPanel();
            this.menuBar1 = new stonerkart.MenuBar();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.DarkViolet;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(1211, 682);
            this.mainPanel.TabIndex = 0;
            // 
            // menuBar1
            // 
            this.menuBar1.BackColor = System.Drawing.Color.SlateBlue;
            this.menuBar1.Location = new System.Drawing.Point(0, 682);
            this.menuBar1.Name = "menuBar1";
            this.menuBar1.Size = new System.Drawing.Size(1211, 31);
            this.menuBar1.TabIndex = 1;
            // 
            // GameFrame
            // 
            this.ClientSize = new System.Drawing.Size(1211, 709);
            this.Controls.Add(this.menuBar1);
            this.Controls.Add(this.mainPanel);
            this.Name = "GameFrame";
            this.ResumeLayout(false);

        }
    }

    interface Screen
    {
    }

    public static class xd
    {
        public static void memeout(this Control uc, Action a, bool wait = false)
        {
            if (uc.InvokeRequired)
            {
                uc.Invoke(a);
            }
            else
            {
                a();
            }
        }

        public static void memehard(this Control uc, Action a)
        {
            
        }
    }
}
