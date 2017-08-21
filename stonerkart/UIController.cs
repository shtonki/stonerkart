using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
/*
namespace stonerkart
{
    static class UIController
    {
        public static GameFrame gameFrame { get; private set; }


        private static DraggablePanel friendsList;
        private static FriendsListPanel friendsListPanel = new FriendsListPanel();

        private static List<string> friends;

        public static void toggleFriendsList()
        {
            if (friendsList != null && !friendsList.closed)
            {
                friendsList.close();
                friendsList = null;
            }
            else
            {
                friendsList = showControl(friendsListPanel);
            }
        }

        public static DraggablePanel showControl(Control c, bool resizable = true, bool closeable = true)
        {
            DraggablePanelConfig cfg = new DraggablePanelConfig();
            cfg.resizeable = resizable;
            cfg.closeable = closeable;
            return showControl(c, cfg);
        }

        public static DraggablePanel showControl(Control c, DraggablePanelConfig cfg)
        {
            return gameFrame.showControl(c, cfg);
        }

        public static GameFrame launchUI()
        {
            gameFrame = new GameFrame();
            ManualResetEvent e = new ManualResetEvent(false);
            EventHandler a = (x, y) => e.Set();
            gameFrame.Load += a;
            Thread t = new Thread(() => Application.Run(gameFrame));
            t.Start();
            e.WaitOne();
            gameFrame.Load -= a;
            return gameFrame;
        }


        public static void setFriendList(List<string> fs)
        {
            friends = new List<string>();

            foreach (var f in fs)
            {
                friends.Add(f);
                friendsListPanel.showFriend(f);
            }
            gameFrame.menuBar1.enableFriendsButton();
        }

        public static void addFriend(string name)
        {
            if (friends.Contains(name)) return;

            if (Network.addFriend(name))
            {
                friends.Add(name);
                friendsListPanel.showFriend(name);
            }
        }

        public static void toggleOptionPanel()
        {
            gameFrame.toggleOptionPanel();
        }
    }
}
*/