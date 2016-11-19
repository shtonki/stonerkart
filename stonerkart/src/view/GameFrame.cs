using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace stonerkart
{
    

    class GameFrame : Form
    {
        

        public Screen mainMenuPanel { get; private set; }
        public GamePanel gamePanel;


        public Control active;

        public GameFrame()
        {
            mainMenuPanel = new MainMenuPanel();
            Closed += (_, __) => Environment.Exit(1);
        }

        public void setPrompt(string message, string[] buttons)
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
                    if (buttons.Length > i)
                    {
                        bs[i].Visible = true;
                        bs[i].Text = buttons[i];
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
                    Controls.Remove(active);
                }
                active = (Control)s;
                active.Dock = DockStyle.Fill;
                Controls.Add(active);
            });
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
    }
}
