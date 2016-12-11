using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart
{
    class MainMenuPanel : StickyPanel, Screen
    {
        private Button button1;
        private Button button2;

        public MainMenuPanel()
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
            this.button1.Location = new System.Drawing.Point(434, 114);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(232, 151);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(434, 513);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(232, 151);
            this.button2.TabIndex = 1;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // MainMenuPanel
            // 
            this.BackColor = System.Drawing.Color.Firebrick;
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "MainMenuPanel";
            this.Size = new System.Drawing.Size(1118, 879);
            this.ResumeLayout(false);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Controller.newGame();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Controller.transitionToMapEditor();
            Controller.transitionToDeckEditor();
        }
    }
}
