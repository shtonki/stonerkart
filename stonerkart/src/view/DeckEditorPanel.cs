using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart.src.view
{
    class DeckEditorPanel : StickyPanel, Screen
    {
        private CardView cardView1;
        private CardView cardView2;
        private CardView cardView3;
        private CardView cardView4;
        private CardView cardView5;
        private CardView cardView6;
        private CardView cardView7;
        private CardView cardView8;
        private CardView cardView9;
        private CardView cardView10;
        private TextBox searchBox;
        private CardView[] cardViews;

        public DeckEditorPanel()
        {
            InitializeComponent();
            cardViews = new CardView[]
            {
                cardView1,
                cardView2,
                cardView3,
                cardView4,
                cardView5,
                cardView6,
                cardView7,
                cardView8,
                cardView9,
                cardView10,
            };            
            drawCards(x => true);                                                
        }
                                                                        
        private void InitializeComponent()                              
        {                                                               
            this.cardView1 = new stonerkart.CardView();
            this.cardView2 = new stonerkart.CardView();
            this.cardView3 = new stonerkart.CardView();
            this.cardView4 = new stonerkart.CardView();
            this.cardView5 = new stonerkart.CardView();
            this.cardView6 = new stonerkart.CardView();
            this.cardView7 = new stonerkart.CardView();
            this.cardView8 = new stonerkart.CardView();
            this.cardView9 = new stonerkart.CardView();
            this.cardView10 = new stonerkart.CardView();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cardView1
            // 
            this.cardView1.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView1.Location = new System.Drawing.Point(82, 105);
            this.cardView1.Name = "cardView1";
            this.cardView1.Size = new System.Drawing.Size(161, 193);
            this.cardView1.TabIndex = 0;
            // 
            // cardView2
            // 
            this.cardView2.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView2.Location = new System.Drawing.Point(249, 105);
            this.cardView2.Name = "cardView2";
            this.cardView2.Size = new System.Drawing.Size(161, 193);
            this.cardView2.TabIndex = 7;
            // 
            // cardView3
            // 
            this.cardView3.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView3.Location = new System.Drawing.Point(416, 105);
            this.cardView3.Name = "cardView3";
            this.cardView3.Size = new System.Drawing.Size(161, 193);
            this.cardView3.TabIndex = 7;
            // 
            // cardView4
            // 
            this.cardView4.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView4.Location = new System.Drawing.Point(750, 105);
            this.cardView4.Name = "cardView4";
            this.cardView4.Size = new System.Drawing.Size(161, 193);
            this.cardView4.TabIndex = 7;
            // 
            // cardView5
            // 
            this.cardView5.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView5.Location = new System.Drawing.Point(583, 105);
            this.cardView5.Name = "cardView5";
            this.cardView5.Size = new System.Drawing.Size(161, 193);
            this.cardView5.TabIndex = 7;
            // 
            // cardView6
            // 
            this.cardView6.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView6.Location = new System.Drawing.Point(583, 304);
            this.cardView6.Name = "cardView6";
            this.cardView6.Size = new System.Drawing.Size(161, 193);
            this.cardView6.TabIndex = 9;
            // 
            // cardView7
            // 
            this.cardView7.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView7.Location = new System.Drawing.Point(750, 304);
            this.cardView7.Name = "cardView7";
            this.cardView7.Size = new System.Drawing.Size(161, 193);
            this.cardView7.TabIndex = 10;
            // 
            // cardView8
            // 
            this.cardView8.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView8.Location = new System.Drawing.Point(416, 304);
            this.cardView8.Name = "cardView8";
            this.cardView8.Size = new System.Drawing.Size(161, 193);
            this.cardView8.TabIndex = 11;
            // 
            // cardView9
            // 
            this.cardView9.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView9.Location = new System.Drawing.Point(249, 304);
            this.cardView9.Name = "cardView9";
            this.cardView9.Size = new System.Drawing.Size(161, 193);
            this.cardView9.TabIndex = 12;
            // 
            // cardView10
            // 
            this.cardView10.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView10.Location = new System.Drawing.Point(82, 304);
            this.cardView10.Name = "cardView10";
            this.cardView10.Size = new System.Drawing.Size(161, 193);
            this.cardView10.TabIndex = 8;
            // 
            // searchBox
            // 
            this.searchBox.Location = new System.Drawing.Point(82, 70);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(829, 20);
            this.searchBox.TabIndex = 13;
            this.searchBox.TextChanged += new System.EventHandler(this.newSearch);
            // 
            // DeckEditorPanel
            // 
            this.BackColor = System.Drawing.Color.Aqua;
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.cardView6);
            this.Controls.Add(this.cardView5);
            this.Controls.Add(this.cardView7);
            this.Controls.Add(this.cardView4);
            this.Controls.Add(this.cardView8);
            this.Controls.Add(this.cardView9);
            this.Controls.Add(this.cardView3);
            this.Controls.Add(this.cardView10);
            this.Controls.Add(this.cardView2);
            this.Controls.Add(this.cardView1);
            this.Name = "DeckEditorPanel";
            this.Size = new System.Drawing.Size(1020, 652);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void drawCards(Func<CardTemplate, bool> filter)
        {
            var cards = Enum.GetValues(typeof(CardTemplate)).Cast<CardTemplate>().Where(x => x != CardTemplate.Belwas && x != CardTemplate.AlterTime).Where(filter);

            for (int i = 0; i < cardViews.Length; i++)
            {
                if (i < cards.Count())
                {
                    cardViews[i].setCard(cards.ElementAt(i));
                    cardViews[i].Visible = true;
                }
                else
                    cardViews[i].Visible = false;
            }

        }

        private void newSearch(object sender, EventArgs e)
        {
            drawCards(x => x.ToString().ToLower().Contains(searchBox.Text));
        }
    }
}
