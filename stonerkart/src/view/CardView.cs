using System;
using System.Drawing;
using System.Windows.Forms;

namespace stonerkart
{


    class CardView : StickyPanel, Clickable, Observer<CardChangedMessage>
    {
        private PictureBox art;
        private AutoFontTextBox breadText;
        private AutoFontTextBox nameBox;
        private AutoFontTextBox toughnessBox;
        private AutoFontTextBox powerBox;
        private PictureBox pictureBox1;
        private AutoFontTextBox cardTypeText;
        private AutoFontTextBox castRangeSlashMovementBox;
        private ManaCostPanel manaCostPanel1;
        public readonly Card card;
        
        public CardView()
        {
            InitializeComponent();
            DoubleBuffered = true;
            breadText.Text = "Bush did it if I do say so myself kappa keepo 420 swag it up ranch all the way";
            nameBox.Text = "x";
            powerBox.Text = "x";
            toughnessBox.Text = "x";
        }

        public CardView(Card c) : this()
        {
            c.addObserver(this);
            card = c;
            ihavedowns();
        }

        private void ihavedowns()
        {
            this.memeout(() =>
            {
                art.Image = card.image;

                nameBox.Text = card.name;
                cardTypeText.Text = card.cardType.ToString();
                breadText.Text = card.breadText;
                manaCostPanel1.setCost(card.castManaCost);

                if (card.cardType == CardType.Creature || card.cardType == CardType.Hero)
                {
                    powerBox.Text = card.power.ToString();
                    toughnessBox.Text = card.toughness.ToString();
                    castRangeSlashMovementBox.Text = card.movement.ToString();
                }
                else
                {
                    castRangeSlashMovementBox.Text = card.castRange.ToString();
                    powerBox.Text = "";
                    toughnessBox.Text = "";
                }
            });
        }

        public void notify(CardChangedMessage t)
        {
            throw new NotImplementedException();
        }

        public Stuff getStuff()
        {
            return card;
        }

        private void InitializeComponent()
        {
            this.art = new System.Windows.Forms.PictureBox();
            this.breadText = new stonerkart.AutoFontTextBox();
            this.nameBox = new stonerkart.AutoFontTextBox();
            this.toughnessBox = new stonerkart.AutoFontTextBox();
            this.powerBox = new stonerkart.AutoFontTextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cardTypeText = new stonerkart.AutoFontTextBox();
            this.castRangeSlashMovementBox = new stonerkart.AutoFontTextBox();
            this.manaCostPanel1 = new stonerkart.ManaCostPanel();
            ((System.ComponentModel.ISupportInitialize)(this.art)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // art
            // 
            this.art.Enabled = false;
            this.art.Image = global::stonerkart.Properties.Resources.jordanno;
            this.art.InitialImage = null;
            this.art.Location = new System.Drawing.Point(46, 64);
            this.art.Name = "art";
            this.art.Size = new System.Drawing.Size(386, 291);
            this.art.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.art.TabIndex = 0;
            this.art.TabStop = false;
            // 
            // breadText
            // 
            this.breadText.BackColor = System.Drawing.Color.Transparent;
            this.breadText.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.breadText.Location = new System.Drawing.Point(41, 394);
            this.breadText.Name = "breadText";
            this.breadText.Opacity = 100;
            this.breadText.Size = new System.Drawing.Size(400, 150);
            this.breadText.TabIndex = 1;
            // 
            // nameBox
            // 
            this.nameBox.BackColor = System.Drawing.Color.Transparent;
            this.nameBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.nameBox.Location = new System.Drawing.Point(31, 15);
            this.nameBox.Name = "nameBox";
            this.nameBox.Opacity = 100;
            this.nameBox.Size = new System.Drawing.Size(242, 33);
            this.nameBox.TabIndex = 2;
            // 
            // toughnessBox
            // 
            this.toughnessBox.BackColor = System.Drawing.Color.Transparent;
            this.toughnessBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.toughnessBox.Location = new System.Drawing.Point(407, 545);
            this.toughnessBox.Name = "toughnessBox";
            this.toughnessBox.Opacity = 100;
            this.toughnessBox.Size = new System.Drawing.Size(53, 57);
            this.toughnessBox.TabIndex = 3;
            // 
            // powerBox
            // 
            this.powerBox.BackColor = System.Drawing.Color.Transparent;
            this.powerBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.powerBox.Location = new System.Drawing.Point(21, 545);
            this.powerBox.Name = "powerBox";
            this.powerBox.Opacity = 100;
            this.powerBox.Size = new System.Drawing.Size(53, 57);
            this.powerBox.TabIndex = 4;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Enabled = false;
            this.pictureBox1.Image = global::stonerkart.Properties.Resources.white3;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(478, 618);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // cardTypeText
            // 
            this.cardTypeText.BackColor = System.Drawing.Color.Transparent;
            this.cardTypeText.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.cardTypeText.Location = new System.Drawing.Point(46, 370);
            this.cardTypeText.Name = "cardTypeText";
            this.cardTypeText.Opacity = 100;
            this.cardTypeText.Size = new System.Drawing.Size(392, 28);
            this.cardTypeText.TabIndex = 5;
            // 
            // castRangeSlashMovementBox
            // 
            this.castRangeSlashMovementBox.BackColor = System.Drawing.Color.Transparent;
            this.castRangeSlashMovementBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.castRangeSlashMovementBox.Location = new System.Drawing.Point(212, 550);
            this.castRangeSlashMovementBox.Name = "castRangeSlashMovementBox";
            this.castRangeSlashMovementBox.Opacity = 100;
            this.castRangeSlashMovementBox.Size = new System.Drawing.Size(53, 57);
            this.castRangeSlashMovementBox.TabIndex = 5;
            // 
            // manaCostPanel1
            // 
            this.manaCostPanel1.Location = new System.Drawing.Point(280, 15);
            this.manaCostPanel1.Name = "manaCostPanel1";
            this.manaCostPanel1.Size = new System.Drawing.Size(161, 33);
            this.manaCostPanel1.TabIndex = 6;
            // 
            // CardView
            // 
            this.Controls.Add(this.manaCostPanel1);
            this.Controls.Add(this.castRangeSlashMovementBox);
            this.Controls.Add(this.cardTypeText);
            this.Controls.Add(this.powerBox);
            this.Controls.Add(this.toughnessBox);
            this.Controls.Add(this.nameBox);
            this.Controls.Add(this.breadText);
            this.Controls.Add(this.art);
            this.Controls.Add(this.pictureBox1);
            this.Name = "CardView";
            this.Size = new System.Drawing.Size(478, 618);
            ((System.ComponentModel.ISupportInitialize)(this.art)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }
    }

    struct CardChangedMessage
    {

    }
}
