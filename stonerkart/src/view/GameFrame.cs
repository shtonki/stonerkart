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
        public GamePanel gamePanel { get; private set; }

        public GameFrame()
        {
            InitializeComponent();
        }


        private void InitializeComponent()
        {
            this.gamePanel = new stonerkart.GamePanel();
            this.SuspendLayout();
            // 
            // gamePanel1
            // 
            this.gamePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gamePanel.BackColor = System.Drawing.Color.Fuchsia;
            this.gamePanel.Location = new System.Drawing.Point(2, 1);
            this.gamePanel.Name = "gamePanel1";
            this.gamePanel.Size = new System.Drawing.Size(597, 413);
            this.gamePanel.TabIndex = 0;
            // 
            // GameFrame
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.ClientSize = new System.Drawing.Size(596, 414);
            this.Controls.Add(this.gamePanel);
            this.Name = "GameFrame";
            this.ResumeLayout(false);

        }
        
    }
}
