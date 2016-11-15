﻿using System;
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
        private PictureBox pictureBox1;
        public readonly Card card;
        
        public CardView()
        {
            InitializeComponent();
            DoubleBuffered = true;
            MouseDown += (_, __) => { Controller.clicked(this); };
            autoFontTextBox1.Text = "Bush did it if I do say so myself kappa keepo 420 swag it up ranch all the way";
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
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
            this.art.Size = new System.Drawing.Size(376, 287);
            this.art.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.art.TabIndex = 0;
            this.art.TabStop = false;
            // 
            // autoFontTextBox1
            // 
            this.autoFontTextBox1.BackColor = System.Drawing.Color.Transparent;
            this.autoFontTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.autoFontTextBox1.Location = new System.Drawing.Point(41, 394);
            this.autoFontTextBox1.Name = "autoFontTextBox1";
            this.autoFontTextBox1.Opacity = 100;
            this.autoFontTextBox1.Size = new System.Drawing.Size(400, 150);
            this.autoFontTextBox1.TabIndex = 1;
            // 
            // nameBox
            // 
            this.nameBox.BackColor = System.Drawing.Color.Transparent;
            this.nameBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.nameBox.Location = new System.Drawing.Point(31, 15);
            this.nameBox.Name = "nameBox";
            this.nameBox.Opacity = 100;
            this.nameBox.Size = new System.Drawing.Size(391, 33);
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
            // CardView
            // 
            this.Controls.Add(this.powerBox);
            this.Controls.Add(this.toughnessBox);
            this.Controls.Add(this.nameBox);
            this.Controls.Add(this.autoFontTextBox1);
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
