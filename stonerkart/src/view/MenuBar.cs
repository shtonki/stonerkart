using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart
{
    class MenuBar : StickyPanel
    {
        private Button optionsButton;
        private Button friendsButton;

        public MenuBar()
        {
            InitializeComponent();
            optionsButton.Click += (__, _) => Controller.toggleOptionPanel();
        }

        public void enableFriendsButton()
        {
            friendsButton.Enabled = true;
        }

        private void InitializeComponent()
        {
            this.friendsButton = new System.Windows.Forms.Button();
            this.optionsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // friendsButton
            // 
            this.friendsButton.Enabled = false;
            this.friendsButton.Location = new System.Drawing.Point(0, 0);
            this.friendsButton.Name = "friendsButton";
            this.friendsButton.Size = new System.Drawing.Size(64, 37);
            this.friendsButton.TabIndex = 0;
            this.friendsButton.Text = "Friends";
            this.friendsButton.UseVisualStyleBackColor = true;
            this.friendsButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // optionsButton
            // 
            this.optionsButton.Location = new System.Drawing.Point(747, 0);
            this.optionsButton.Name = "optionsButton";
            this.optionsButton.Size = new System.Drawing.Size(64, 37);
            this.optionsButton.TabIndex = 1;
            this.optionsButton.Text = "Options";
            this.optionsButton.UseVisualStyleBackColor = true;
            // 
            // MenuBar
            // 
            this.BackColor = System.Drawing.Color.SlateBlue;
            this.Controls.Add(this.optionsButton);
            this.Controls.Add(this.friendsButton);
            this.Name = "MenuBar";
            this.Size = new System.Drawing.Size(811, 37);
            this.ResumeLayout(false);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Controller.toggleFriendsList();
        }
    }
}
