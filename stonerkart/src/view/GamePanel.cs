using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart
{
    class GamePanel : UserControl
    {
        private SplitContainer splitContainer1;
        public ConsolePanel consolePanel { get; private set; }
        private HexPanel hexPanel3;

        public GamePanel()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.hexPanel3 = new stonerkart.HexPanel();
            this.consolePanel = new stonerkart.ConsolePanel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(3, 4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.hexPanel3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.Crimson;
            this.splitContainer1.Panel2.Controls.Add(this.consolePanel);
            this.splitContainer1.Size = new System.Drawing.Size(672, 537);
            this.splitContainer1.SplitterDistance = 518;
            this.splitContainer1.TabIndex = 0;
            // 
            // hexPanel3
            // 
            this.hexPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexPanel3.BackColor = System.Drawing.Color.Aqua;
            this.hexPanel3.Location = new System.Drawing.Point(3, 3);
            this.hexPanel3.Name = "hexPanel3";
            this.hexPanel3.Size = new System.Drawing.Size(512, 531);
            this.hexPanel3.TabIndex = 0;
            // 
            // consolePanel1
            // 
            this.consolePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.consolePanel.Location = new System.Drawing.Point(3, 0);
            this.consolePanel.Name = "consolePanel1";
            this.consolePanel.Size = new System.Drawing.Size(144, 534);
            this.consolePanel.TabIndex = 0;
            // 
            // GamePanel
            // 
            this.BackColor = System.Drawing.Color.Fuchsia;
            this.Controls.Add(this.splitContainer1);
            this.Name = "GamePanel";
            this.Size = new System.Drawing.Size(678, 544);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
