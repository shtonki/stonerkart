using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart
{
    class GameFrame : Form
    {
        private GamePanel gamePanel1;

        public GameFrame()
        {
            InitializeComponent();
        }


        private void InitializeComponent()
        {
            this.gamePanel1 = new stonerkart.GamePanel();
            this.SuspendLayout();
            // 
            // gamePanel1
            // 
            this.gamePanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gamePanel1.BackColor = System.Drawing.Color.Fuchsia;
            this.gamePanel1.Location = new System.Drawing.Point(-2, 0);
            this.gamePanel1.Name = "gamePanel1";
            this.gamePanel1.Size = new System.Drawing.Size(1004, 826);
            this.gamePanel1.TabIndex = 0;
            // 
            // GameFrame
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.ClientSize = new System.Drawing.Size(1004, 827);
            this.Controls.Add(this.gamePanel1);
            this.Name = "GameFrame";
            this.ResumeLayout(false);

        }
        
    }
}
