using System;
using System.Drawing;
using System.Windows.Forms;

namespace stonerkart
{


    class CardView : StickyPanel, Clickable, Observer<CardChangedMessage>
    {
        private PictureBox art;
        private AutoFontTextBox autoFontTextBox1;
        private AutoFontTextBox nameBox;
        private AutoFontTextBox toughnessBox;
        private AutoFontTextBox powerBox;
        public readonly Card card;
        
        public CardView()
        {
            InitializeComponent();
            DoubleBuffered = true;
            BackColor = Color.ForestGreen;
            MouseDown += (_, __) => { Controller.clicked(this); };
            autoFontTextBox1.Text = "Bush did it if I do say so myself";
        }

        public CardView(Card c) : this()
        {
            c.addObserver(this);
            card = c;
            art.Image = card.image;
            this.memeout(() =>
            {
                nameBox.Text = card.name;
                powerBox.Text = "1";
                toughnessBox.Text = "1";
            });
        }

        public void notify(CardChangedMessage t)
        {
            throw new NotImplementedException();
        }

        public object getStuff()
        {
            return card;
        }

        private void InitializeComponent()
        {
            this.art = new System.Windows.Forms.PictureBox();
            this.autoFontTextBox1 = new stonerkart.AutoFontTextBox();
            this.nameBox = new stonerkart.AutoFontTextBox();
            this.toughnessBox = new stonerkart.AutoFontTextBox();
            this.powerBox = new stonerkart.AutoFontTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.art)).BeginInit();
            this.SuspendLayout();
            // 
            // art
            // 
            this.art.Enabled = false;
            this.art.Image = global::stonerkart.Properties.Resources.jordanno;
            this.art.InitialImage = null;
            this.art.Location = new System.Drawing.Point(31, 46);
            this.art.Name = "art";
            this.art.Size = new System.Drawing.Size(473, 401);
            this.art.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.art.TabIndex = 0;
            this.art.TabStop = false;
            // 
            // autoFontTextBox1
            // 
            this.autoFontTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.autoFontTextBox1.Location = new System.Drawing.Point(155, 521);
            this.autoFontTextBox1.Name = "autoFontTextBox1";
            this.autoFontTextBox1.Size = new System.Drawing.Size(221, 119);
            this.autoFontTextBox1.TabIndex = 1;
            this.autoFontTextBox1.Text = "autoFontTextBox1";
            // 
            // nameBox
            // 
            this.nameBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.nameBox.Location = new System.Drawing.Point(50, 0);
            this.nameBox.Name = "nameBox";
            this.nameBox.Size = new System.Drawing.Size(454, 43);
            this.nameBox.TabIndex = 2;
            this.nameBox.Text = "autoFontTextBox2";
            // 
            // toughnessBox
            // 
            this.toughnessBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.toughnessBox.Location = new System.Drawing.Point(459, 629);
            this.toughnessBox.Name = "toughnessBox";
            this.toughnessBox.Size = new System.Drawing.Size(61, 68);
            this.toughnessBox.TabIndex = 3;
            this.toughnessBox.Text = "autoFontTextBox2";
            // 
            // powerBox
            // 
            this.powerBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.powerBox.Location = new System.Drawing.Point(27, 629);
            this.powerBox.Name = "powerBox";
            this.powerBox.Size = new System.Drawing.Size(61, 68);
            this.powerBox.TabIndex = 4;
            this.powerBox.Text = "autoFontTextBox3";
            // 
            // CardView
            // 
            this.Controls.Add(this.powerBox);
            this.Controls.Add(this.toughnessBox);
            this.Controls.Add(this.nameBox);
            this.Controls.Add(this.autoFontTextBox1);
            this.Controls.Add(this.art);
            this.Name = "CardView";
            this.Size = new System.Drawing.Size(535, 711);
            ((System.ComponentModel.ISupportInitialize)(this.art)).EndInit();
            this.ResumeLayout(false);

        }
    }

    struct CardChangedMessage
    {

    }
}
