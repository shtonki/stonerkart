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
            Size = new Size(1200, 900);
            mainMenuPanel = new MainMenuPanel();
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
