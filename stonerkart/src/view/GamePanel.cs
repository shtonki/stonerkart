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
        public AutoFontTextBox promtText;
        public Shibbutton shibbutton2;
        public Shibbutton shibbutton3;
        public Shibbutton shibbutton4;
        public Shibbutton shibbutton5;
        private CardsPanel stackPanel;
        private PlayerPanel heroPanel;
        private PlayerPanel villainPanel;
        private StepPanel stepPanel1;
        private CardView highlightCard;
        public HexPanel hexPanel;

        public GamePanel(Game g)
        {
            InitializeComponent();
            if (g == null) return;
            hexPanel.setMap(g.map);
            hexPanel.tileClicked.Add(g.clicked);
            hexPanel.tileEntered.Add(clickable => setHightlight(clickable?.getStuff()));
            

            cardsPanel1.setPile(g.hero.hand);
            cardsPanel1.clickedCallbacks.Add(g.clicked);
            cardsPanel1.mouseEnteredCallbacks.Add(clickable => setHightlight(clickable.getStuff()));

            stackPanel.vertical = true;
            stackPanel.setPile(g.stack);
            stackPanel.clickedCallbacks.Add(g.clicked);
            stackPanel.mouseEnteredCallbacks.Add(clickable => setHightlight(clickable.getStuff()));

            shibbutton2.MouseDown += (_, __) => g.clicked(shibbutton2);
            shibbutton3.MouseDown += (_, __) => g.clicked(shibbutton3);
            shibbutton4.MouseDown += (_, __) => g.clicked(shibbutton4);
            shibbutton5.MouseDown += (_, __) => g.clicked(shibbutton5);

            heroPanel.manaPanel1.callbacks.Add(g.clicked);
            g.hero.addObserver(heroPanel);

            g.villain.addObserver(villainPanel);
            

            g.stepHandler.addObserver(stepPanel1);
        }

        public GamePanel() : this(null)
        {
        }

        private void setHightlight(Stuff stuff)
        {
            if (stuff == null) return;
            Card newCard = null;
            if (stuff is Tile)
            {
                Tile tile = (Tile)stuff;
                if (tile.card != null)
                {
                    newCard = tile.card;
                }
            }
            else if (stuff is Card)
            {
                newCard = (Card)stuff;
            }
            if (newCard != null && newCard != highlightCard.card)
            {
                highlightCard.setCard(newCard);
            }
        }

        private void InitializeComponent()
        {
            this.hexPanel = new stonerkart.HexPanel();
            this.cardsPanel1 = new stonerkart.CardsPanel();
            this.promtText = new stonerkart.AutoFontTextBox();
            this.shibbutton2 = new stonerkart.Shibbutton();
            this.shibbutton3 = new stonerkart.Shibbutton();
            this.shibbutton4 = new stonerkart.Shibbutton();
            this.shibbutton5 = new stonerkart.Shibbutton();
            this.stackPanel = new stonerkart.CardsPanel();
            this.heroPanel = new stonerkart.PlayerPanel();
            this.villainPanel = new stonerkart.PlayerPanel();
            this.stepPanel1 = new stonerkart.StepPanel();
            this.highlightCard = new stonerkart.CardView();
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
            // stackPanel
            // 
            this.stackPanel.BackColor = System.Drawing.Color.Navy;
            this.stackPanel.Location = new System.Drawing.Point(7, 314);
            this.stackPanel.Name = "stackPanel";
            this.stackPanel.Size = new System.Drawing.Size(147, 213);
            this.stackPanel.TabIndex = 10;
            // 
            // heroPanel
            // 
            this.heroPanel.BackColor = System.Drawing.Color.DarkViolet;
            this.heroPanel.Location = new System.Drawing.Point(4, 533);
            this.heroPanel.Name = "heroPanel";
            this.heroPanel.Size = new System.Drawing.Size(139, 132);
            this.heroPanel.TabIndex = 11;
            // 
            // villainPanel
            // 
            this.villainPanel.BackColor = System.Drawing.Color.DarkViolet;
            this.villainPanel.Location = new System.Drawing.Point(7, 176);
            this.villainPanel.Name = "villainPanel";
            this.villainPanel.Size = new System.Drawing.Size(136, 132);
            this.villainPanel.TabIndex = 12;
            // 
            // stepPanel1
            // 
            this.stepPanel1.Location = new System.Drawing.Point(161, 314);
            this.stepPanel1.Name = "stepPanel1";
            this.stepPanel1.Size = new System.Drawing.Size(53, 213);
            this.stepPanel1.TabIndex = 13;
            // 
            // highlightCard
            // 
            this.highlightCard.BackColor = System.Drawing.Color.DarkViolet;
            this.highlightCard.Location = new System.Drawing.Point(911, 256);
            this.highlightCard.Name = "highlightCard";
            this.highlightCard.Size = new System.Drawing.Size(172, 254);
            this.highlightCard.TabIndex = 14;
            // 
            // GamePanel
            // 
            this.BackColor = System.Drawing.Color.Fuchsia;
            this.Controls.Add(this.highlightCard);
            this.Controls.Add(this.stepPanel1);
            this.Controls.Add(this.villainPanel);
            this.Controls.Add(this.heroPanel);
            this.Controls.Add(this.stackPanel);
            this.Controls.Add(this.shibbutton4);
            this.Controls.Add(this.shibbutton5);
            this.Controls.Add(this.shibbutton3);
            this.Controls.Add(this.shibbutton2);
            this.Controls.Add(this.promtText);
            this.Controls.Add(this.cardsPanel1);
            this.Controls.Add(this.hexPanel);
            this.Name = "GamePanel";
            this.Size = new System.Drawing.Size(1086, 669);
            this.ResumeLayout(false);

        }
    }
}
