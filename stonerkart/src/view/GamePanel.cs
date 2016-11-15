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
        public ConsolePanel consolePanel;
        private Shibbutton shibbutton1;
        public AutoFontTextBox promtText;
        public HexPanel hexPanel;

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
            this.consolePanel = new stonerkart.ConsolePanel();
            this.shibbutton1 = new stonerkart.Shibbutton();
            this.promtText = new stonerkart.AutoFontTextBox();
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
            // consolePanel
            // 
            this.consolePanel.Location = new System.Drawing.Point(911, 3);
            this.consolePanel.Name = "consolePanel";
            this.consolePanel.Size = new System.Drawing.Size(172, 507);
            this.consolePanel.TabIndex = 2;
            // 
            // shibbutton1
            // 
            this.shibbutton1.Location = new System.Drawing.Point(3, 87);
            this.shibbutton1.Name = "shibbutton1";
            this.shibbutton1.Size = new System.Drawing.Size(208, 47);
            this.shibbutton1.TabIndex = 3;
            this.shibbutton1.Text = "shibbutton1";
            this.shibbutton1.UseVisualStyleBackColor = true;
            // 
            // promtText
            // 
            this.promtText.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.promtText.Location = new System.Drawing.Point(0, 0);
            this.promtText.Name = "promtText";
            this.promtText.Size = new System.Drawing.Size(211, 84);
            this.promtText.TabIndex = 4;
            this.promtText.Text = "asd asd asd asd asd asd asd asd asd asd asd asd asd asd asd asd ";
            // 
            // GamePanel
            // 
            this.BackColor = System.Drawing.Color.Fuchsia;
            this.Controls.Add(this.promtText);
            this.Controls.Add(this.shibbutton1);
            this.Controls.Add(this.consolePanel);
            this.Controls.Add(this.cardsPanel1);
            this.Controls.Add(this.hexPanel);
            this.Name = "GamePanel";
            this.Size = new System.Drawing.Size(1086, 669);
            this.ResumeLayout(false);

        }
        
    }
}
