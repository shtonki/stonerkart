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
        private CardsPanel cardsPanel1;
        private ConsolePanel consolePanel1;
        private HexPanel hexPanel;

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
            this.hexPanel = new stonerkart.HexPanel();
            this.cardsPanel1 = new stonerkart.CardsPanel();
            this.consolePanel1 = new stonerkart.ConsolePanel();
            this.SuspendLayout();
            // 
            // hexPanel
            // 
            this.hexPanel.BackColor = System.Drawing.Color.Aqua;
            this.hexPanel.Location = new System.Drawing.Point(217, 3);
            this.hexPanel.Name = "hexPanel";
            this.hexPanel.Size = new System.Drawing.Size(688, 507);
            this.hexPanel.TabIndex = 0;
            // 
            // cardsPanel1
            // 
            this.cardsPanel1.BackColor = System.Drawing.Color.Navy;
            this.cardsPanel1.Location = new System.Drawing.Point(217, 516);
            this.cardsPanel1.Name = "cardsPanel1";
            this.cardsPanel1.Size = new System.Drawing.Size(866, 150);
            this.cardsPanel1.TabIndex = 1;
            // 
            // consolePanel1
            // 
            this.consolePanel1.Location = new System.Drawing.Point(911, 3);
            this.consolePanel1.Name = "consolePanel1";
            this.consolePanel1.Size = new System.Drawing.Size(172, 507);
            this.consolePanel1.TabIndex = 2;
            // 
            // GamePanel
            // 
            this.BackColor = System.Drawing.Color.Fuchsia;
            this.Controls.Add(this.consolePanel1);
            this.Controls.Add(this.cardsPanel1);
            this.Controls.Add(this.hexPanel);
            this.Name = "GamePanel";
            this.Size = new System.Drawing.Size(1086, 669);
            this.ResumeLayout(false);

        }
        
    }
}
