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
        public AutoFontTextBox promtText;
        public Shibbutton shibbutton2;
        public Shibbutton shibbutton3;
        public Shibbutton shibbutton4;
        public Shibbutton shibbutton5;
        private ManaPanel manaPanel1;
        private CardsPanel stackPanel;
        public HexPanel hexPanel;

        public GamePanel(Game g)
        {
            InitializeComponent();
            if (g == null) return;
            hexPanel.setMap(g.map);
            hexPanel.tileClicked.Add(g.clicked);

            cardsPanel1.setPile(g.hero.hand);
            cardsPanel1.callbacks.Add(g.clicked);

            stackPanel.vertical = true;
            stackPanel.setPile(g.stack);
            stackPanel.callbacks.Add(g.clicked);

            shibbutton2.MouseDown += (_, __) => g.clicked(shibbutton2);
            shibbutton3.MouseDown += (_, __) => g.clicked(shibbutton3);
            shibbutton4.MouseDown += (_, __) => g.clicked(shibbutton4);
            shibbutton5.MouseDown += (_, __) => g.clicked(shibbutton5);

        }

        public GamePanel() : this(null)
        {
        }

        private void InitializeComponent()
        {
            this.hexPanel = new stonerkart.HexPanel();
            this.cardsPanel1 = new stonerkart.CardsPanel();
            this.consolePanel = new stonerkart.ConsolePanel();
            this.promtText = new stonerkart.AutoFontTextBox();
            this.shibbutton2 = new stonerkart.Shibbutton();
            this.shibbutton3 = new stonerkart.Shibbutton();
            this.shibbutton4 = new stonerkart.Shibbutton();
            this.shibbutton5 = new stonerkart.Shibbutton();
            this.manaPanel1 = new stonerkart.ManaPanel();
            this.stackPanel = new stonerkart.CardsPanel();
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
            // promtText
            // 
            this.promtText.BackColor = System.Drawing.Color.Transparent;
            this.promtText.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.promtText.Location = new System.Drawing.Point(3, 6);
            this.promtText.Name = "promtText";
            this.promtText.Opacity = 100;
            this.promtText.Size = new System.Drawing.Size(211, 84);
            this.promtText.TabIndex = 4;
            this.promtText.Text = "asd asd asd asd asd asd asd asd asd asd asd asd asd asd asd asd ";
            // 
            // shibbutton2
            // 
            this.shibbutton2.Location = new System.Drawing.Point(3, 96);
            this.shibbutton2.Name = "shibbutton2";
            this.shibbutton2.Size = new System.Drawing.Size(105, 34);
            this.shibbutton2.TabIndex = 5;
            this.shibbutton2.Text = "shibbutton2";
            this.shibbutton2.UseVisualStyleBackColor = true;
            // 
            // shibbutton3
            // 
            this.shibbutton3.Location = new System.Drawing.Point(109, 96);
            this.shibbutton3.Name = "shibbutton3";
            this.shibbutton3.Size = new System.Drawing.Size(105, 34);
            this.shibbutton3.TabIndex = 6;
            this.shibbutton3.Text = "shibbutton3";
            this.shibbutton3.UseVisualStyleBackColor = true;
            // 
            // shibbutton4
            // 
            this.shibbutton4.Location = new System.Drawing.Point(3, 136);
            this.shibbutton4.Name = "shibbutton4";
            this.shibbutton4.Size = new System.Drawing.Size(105, 34);
            this.shibbutton4.TabIndex = 8;
            this.shibbutton4.Text = "shibbutton4";
            this.shibbutton4.UseVisualStyleBackColor = true;
            // 
            // shibbutton5
            // 
            this.shibbutton5.Location = new System.Drawing.Point(109, 136);
            this.shibbutton5.Name = "shibbutton5";
            this.shibbutton5.Size = new System.Drawing.Size(102, 34);
            this.shibbutton5.TabIndex = 7;
            this.shibbutton5.Text = "shibbutton5";
            this.shibbutton5.UseVisualStyleBackColor = true;
            // 
            // manaPanel1
            // 
            this.manaPanel1.BackColor = System.Drawing.Color.DarkGray;
            this.manaPanel1.Location = new System.Drawing.Point(3, 176);
            this.manaPanel1.Name = "manaPanel1";
            this.manaPanel1.Size = new System.Drawing.Size(208, 158);
            this.manaPanel1.TabIndex = 9;
            // 
            // stackPanel
            // 
            this.stackPanel.BackColor = System.Drawing.Color.Navy;
            this.stackPanel.Location = new System.Drawing.Point(4, 341);
            this.stackPanel.Name = "stackPanel";
            this.stackPanel.Size = new System.Drawing.Size(147, 235);
            this.stackPanel.TabIndex = 10;
            // 
            // GamePanel
            // 
            this.BackColor = System.Drawing.Color.Fuchsia;
            this.Controls.Add(this.stackPanel);
            this.Controls.Add(this.manaPanel1);
            this.Controls.Add(this.shibbutton4);
            this.Controls.Add(this.shibbutton5);
            this.Controls.Add(this.shibbutton3);
            this.Controls.Add(this.shibbutton2);
            this.Controls.Add(this.promtText);
            this.Controls.Add(this.consolePanel);
            this.Controls.Add(this.cardsPanel1);
            this.Controls.Add(this.hexPanel);
            this.Name = "GamePanel";
            this.Size = new System.Drawing.Size(1086, 669);
            this.ResumeLayout(false);

        }
    }
}
