using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart
{
    class FriendsListPanel : UserControl
    {
        private Panel fuckPanel;
        private ScrollBar vScrollBar1;
        private List<FriendPanel> friends;
        private Panel addPanel;

        private int friendPanelHeight = 40;

        public FriendsListPanel()
        {
            BackColor = Color.BurlyWood;

            addPanel = new Panel();
            addPanel.BackColor = Color.Teal;
            TextBox addInput = new TextBox();
            addInput.Size = new Size(100, friendPanelHeight);
            addInput.KeyPress += (sender, args) =>
            {
                if (args.KeyChar == (char)13)
                {
                    Controller.addFriend(addInput.Text);
                    addInput.Text = "";
                }
            };
            addPanel.Controls.Add(addInput);
            Controls.Add(addPanel);

            friends = new List<FriendPanel>();

            fuckPanel = new Panel();
            fuckPanel.BackColor = Color.DarkBlue;
            Controls.Add(fuckPanel);

            vScrollBar1 = new VScrollBar();
            vScrollBar1.Dock = DockStyle.Right;
            vScrollBar1.Scroll += (sender, e) => { VerticalScroll.Value = vScrollBar1.Value; };
            Controls.Add(vScrollBar1);

            Resize += (_, __) => xd();
        }

        public void showFriend(string name)
        {
            FriendPanel p = new FriendPanel(name);
            fuckPanel.Controls.Add(p);
            friends.Add(p);
            xd();
        }

        public bool unshowFriend(string name)
        {
            FriendPanel p = friends.Find(v => v.name == name);
            if (p != null)
            {
                fuckPanel.Controls.Remove(p);
                friends.Remove(p);
                xd();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void xd()
        {
            int w = Size.Width - vScrollBar1.Width;
            int h = Size.Height;

            fuckPanel.SetBounds(0, friendPanelHeight, w, h);

            addPanel.SetBounds(0, 0, w, friendPanelHeight);
            addPanel.BringToFront();

            for (int i = 0; i < friends.Count; i++)
            {
                var p = friends[i];
                p.SetBounds(0, i*friendPanelHeight, w, friendPanelHeight);
            }
        }
    }

    class FriendPanel : UserControl
    {
        public string name;

        private Button challengeButton;
        private AutoFontTextBox nameLabel;

        public FriendPanel(string friendName)
        {
            name = friendName;

            BackColor = Color.DodgerBlue;

            challengeButton = new Button();
            challengeButton.Text ="\uD83D\uDC71";
            nameLabel = new AutoFontTextBox();
            nameLabel.Text = friendName;

            Controls.Add(challengeButton);
            Controls.Add(nameLabel);

            Resize += (_, __) => xd();
            MouseClick += (_, __) => Controller.challengePlayer(name);
        }

        private void xd()
        {
            int w = Size.Width;
            int h = Size.Height;

            challengeButton.SetBounds(w-h, 0, h, h);
            nameLabel.SetBounds(0, 0, w - h, h);
        }
    }
}
