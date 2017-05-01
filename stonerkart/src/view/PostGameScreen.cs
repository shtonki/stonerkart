using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart
{
    class PostGameScreen : StickyPanel, Screen
    {
        private AutoFontTextBox autoFontTextBox1;
        private Button button1;

        public PostGameScreen()
        {
            InitializeComponent();
        }

        public PostGameScreen(Game g, GameEndStruct ges) : this()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(ges.winningPlayer);
            sb.Append(" won the game by ");
            sb.AppendLine(ges.reason.ToString());

            if (ges.yourRating > 0)
            {

                sb.Append("Your rating: ");
                sb.Append(ges.yourRating.ToString());
                sb.Append(" -> ");
                sb.AppendLine((ges.yourRating + ges.ratingChange).ToString());

                sb.Append("Their rating: ");
                sb.Append(ges.theirRating.ToString());
                sb.Append(" -> ");
                sb.AppendLine((ges.theirRating - ges.ratingChange).ToString());
            }

            autoFontTextBox1.Text = sb.ToString();
        }

        public IEnumerable<MenuItem> getMenuPanel()
        {
            return new MenuItem[0];
        }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.autoFontTextBox1 = new stonerkart.AutoFontTextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(237, 521);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(453, 65);
            this.button1.TabIndex = 0;
            this.button1.Text = "Main Menu";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // autoFontTextBox1
            // 
            this.autoFontTextBox1.BackColor = System.Drawing.Color.Transparent;
            this.autoFontTextBox1.Enabled = false;
            this.autoFontTextBox1.justifyText = stonerkart.Justify.Left;
            this.autoFontTextBox1.Location = new System.Drawing.Point(237, 14);
            this.autoFontTextBox1.Name = "autoFontTextBox1";
            this.autoFontTextBox1.Opacity = 100;
            this.autoFontTextBox1.Size = new System.Drawing.Size(453, 471);
            this.autoFontTextBox1.TabIndex = 1;
            // 
            // PostGameScreen
            // 
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.Controls.Add(this.autoFontTextBox1);
            this.Controls.Add(this.button1);
            this.Name = "PostGameScreen";
            this.Size = new System.Drawing.Size(900, 589);
            this.ResumeLayout(false);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ScreenController.transitionToMainMenu();
        }
    }
}
