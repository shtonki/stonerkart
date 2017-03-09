using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using stonerkart.src.view;

namespace stonerkart
{
    /// <summary>
    /// 
    /// </summary>
    class GameFrame : Form
    {
        public Screen mainMenuPanel { get; private set; }
        public Screen loginPanel { get; private set; }
        public GamePanel gamePanel;
        private StickyPanel mainPanel;
        public MenuBar menuBar1;

        public Screen mapEditorScreen { get; private set; }
        public Screen deckEditorScreen { get; private set; }
        public Control activeScreen { get; private set; }
        public Screen activeScreenS => (Screen)activeScreen;

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

        public DraggablePanel showControl(Control content, DraggablePanelConfig dfg = null)
        {
            dfg = dfg ?? new DraggablePanelConfig();

            DraggablePanel r = new DraggablePanel(content, dfg.resizeable, dfg.closeable);
            this.memeout(() =>
            {
                Controls.Add(r);
                r.BringToFront();

                int w = ClientSize.Width;
                int h = ClientSize.Height;

                r.Bounds = new Rectangle(
                    (int)((dfg.hackrect.X / 100f) * w),
                    (int)((dfg.hackrect.Y / 100f) * h),
                    (int)((dfg.hackrect.Width / 100f) * w),
                    (int)((dfg.hackrect.Height / 100f) * h)
                    );

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
            if (optionPanel != null) toggleOptionPanel();

            this.memeout(() =>
            {
                if (activeScreen != null)
                {
                    mainPanel.Controls.Remove(activeScreen);
                }
                activeScreen = (Control)s;
                activeScreen.Dock = DockStyle.Fill;
                mainPanel.Controls.Add(activeScreen);
            });
        }

        private void showBugReportScreen()
        {
            DraggablePanel v = null;
            BugReportPanel c = new BugReportPanel();
            c.submitButton.Click += (__, _) =>
            {
                v.close();
                Network.submitBug(c.bugText.Text);
            };
            c.cancelButton.Click += (__, _) =>
            {
                v.close();
            };
            v = showControl(c);
        }

        private DraggablePanel optionPanel;
        public void toggleOptionPanel()
        {
            if (optionPanel == null)
            {
                Panel p = menuFromItems(activeScreenS.getMenuPanel());
                optionPanel = showControl(p);
            }
            else
            {
                optionPanel.close();
                optionPanel = null;
            }
        }

        private Panel menuFromItems(IEnumerable<MenuItem> mis)
        {
            MenuItem mi = new MenuItem("Report Bug", showBugReportScreen);
            MenuItem[] dflts = { mi };

            Panel p = new Panel();
            MenuItem[] items = mis.Concat(dflts).ToArray();

            p.Size = new Size(220, 20 + 50 * items.Count());
            for(int i = 0; i < items.Length; i++)
            {
                var m = items[i];
                Button b = new Button();
                b.Text = m.title;
                b.Size = new Size(200, 50);
                b.Location = new Point(10, 10 + i * 50);
                b.Click += (__, _) =>
                {
                    toggleOptionPanel();
                    m.action();
                };
                p.Controls.Add(b);
            }
            return p;
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
        IEnumerable<MenuItem> getMenuPanel();
    }

    struct MenuItem
    {
        public string title;
        public Action action;

        public MenuItem(string title, Action action)
        {
            this.title = title;
            this.action = action;
        }
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
    }
}
