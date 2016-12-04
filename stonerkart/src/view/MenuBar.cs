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
        private Button button2;
        private Button button1;

        public MenuBar()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(64, 37);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(747, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(64, 37);
            this.button2.TabIndex = 1;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // MenuBar
            // 
            this.BackColor = System.Drawing.Color.SlateBlue;
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
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
