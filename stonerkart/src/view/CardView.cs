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
            this.memeout(() => nameBox.Text = card.name);
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
            // CardView
            // 
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
