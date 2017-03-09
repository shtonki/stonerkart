using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart.src.view
{
    class PostGameScreen : StickyPanel, Screen
    {
        private Button button1;

        public PostGameScreen()
        {
            InitializeComponent();
        }

        public IEnumerable<MenuItem> getMenuPanel()
        {
            return new MenuItem[0];
        }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(212, 145);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(446, 283);
            this.button1.TabIndex = 0;
            this.button1.Text = "Main Menu";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // PostGameScreen
            // 
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.Controls.Add(this.button1);
            this.Name = "PostGameScreen";
            this.Size = new System.Drawing.Size(900, 589);
            this.ResumeLayout(false);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Controller.transitionToMainMenu();
        }
    }
}
