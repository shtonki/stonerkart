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
        private Button button3;
        private Button button4;
        private Button button2;

        public MainMenuPanel()
        {
            InitializeComponent();
        }

        public IEnumerable<MenuItem> getMenuPanel()
        {
            return new List<MenuItem>();
        }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(434, 114);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(232, 151);
            this.button1.TabIndex = 0;
            this.button1.Text = "Play vs AI";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(434, 513);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(232, 151);
            this.button2.TabIndex = 1;
            this.button2.Text = "Deck Editor";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(434, 310);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(232, 151);
            this.button3.TabIndex = 2;
            this.button3.Text = "Play Rankered";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(672, 513);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(41, 27);
            this.button4.TabIndex = 3;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // MainMenuPanel
            // 
            this.BackColor = System.Drawing.Color.Firebrick;
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "MainMenuPanel";
            this.Size = new System.Drawing.Size(1118, 879);
            this.ResumeLayout(false);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            NewGameStruct s = new NewGameStruct(-1, r.Next(), new []{"Hero", "Villain"}, 0);
            ScreenController.transitionToGamePanel(s, true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ScreenController.transitionToDeckEditor();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ScreenController.transitionToRankedScreen();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ScreenController.transitionToShowMeYourDeckScreen();
        }
    }
}
