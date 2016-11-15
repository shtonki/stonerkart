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
    class GamePanel : StickyPanel, Screen
    {
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        public CardsPanel cardsPanel1;
        public HexPanel hexPanel;
        private SplitContainer splitContainer3;
        private StickyPanel stickyPanel1;
        private Shibbutton shibbutton1;
        public ConsolePanel consolePanel;


        public GamePanel(Game g)
        {
            InitializeComponent();
            if (g == null) return;
            hexPanel.setMap(g.map);
            cardsPanel1.setPile(g.test);
        }

        public GamePanel() : this(null)
        {
        }


        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.stickyPanel1 = new stonerkart.StickyPanel();
            this.shibbutton1 = new stonerkart.Shibbutton();
            this.hexPanel = new stonerkart.HexPanel();
            this.cardsPanel1 = new stonerkart.CardsPanel();
            this.consolePanel = new stonerkart.ConsolePanel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.stickyPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.CausesValidation = false;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.consolePanel);
            this.splitContainer1.Size = new System.Drawing.Size(1118, 879);
            this.splitContainer1.SplitterDistance = 910;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.cardsPanel1);
            this.splitContainer2.Size = new System.Drawing.Size(910, 879);
            this.splitContainer2.SplitterDistance = 692;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.stickyPanel1);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.hexPanel);
            this.splitContainer3.Size = new System.Drawing.Size(910, 692);
            this.splitContainer3.SplitterDistance = 364;
            this.splitContainer3.TabIndex = 1;
            // 
            // stickyPanel1
            // 
            this.stickyPanel1.BackColor = System.Drawing.Color.DarkViolet;
            this.stickyPanel1.Controls.Add(this.shibbutton1);
            this.stickyPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stickyPanel1.Location = new System.Drawing.Point(0, 0);
            this.stickyPanel1.Name = "stickyPanel1";
            this.stickyPanel1.Size = new System.Drawing.Size(364, 692);
            this.stickyPanel1.TabIndex = 0;
            // 
            // shibbutton1
            // 
            this.shibbutton1.Location = new System.Drawing.Point(94, 54);
            this.shibbutton1.Name = "shibbutton1";
            this.shibbutton1.Size = new System.Drawing.Size(171, 265);
            this.shibbutton1.TabIndex = 0;
            this.shibbutton1.Text = "shibbutton1";
            this.shibbutton1.UseVisualStyleBackColor = true;
            // 
            // hexPanel
            // 
            this.hexPanel.BackColor = System.Drawing.Color.Aqua;
            this.hexPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexPanel.Location = new System.Drawing.Point(0, 0);
            this.hexPanel.Name = "hexPanel";
            this.hexPanel.Size = new System.Drawing.Size(542, 692);
            this.hexPanel.TabIndex = 0;
            // 
            // cardsPanel1
            // 
            this.cardsPanel1.BackColor = System.Drawing.Color.Navy;
            this.cardsPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardsPanel1.Location = new System.Drawing.Point(0, 0);
            this.cardsPanel1.Name = "cardsPanel1";
            this.cardsPanel1.Size = new System.Drawing.Size(910, 183);
            this.cardsPanel1.TabIndex = 0;
            // 
            // consolePanel
            // 
            this.consolePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.consolePanel.Location = new System.Drawing.Point(3, 3);
            this.consolePanel.Name = "consolePanel";
            this.consolePanel.Size = new System.Drawing.Size(198, 873);
            this.consolePanel.TabIndex = 0;
            // 
            // GamePanel
            // 
            this.BackColor = System.Drawing.Color.Fuchsia;
            this.Controls.Add(this.splitContainer1);
            this.Name = "GamePanel";
            this.Size = new System.Drawing.Size(1118, 879);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.stickyPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        
    }
}
