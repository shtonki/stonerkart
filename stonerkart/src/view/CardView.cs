using System;
using System.Drawing;
using System.Windows.Forms;

namespace stonerkart
{
    class CardView : UserControl, Clickable, Observer<CardChangedMessage>
    {
        private PictureBox pictureBox1;
        
        public CardView()
        {
            InitializeComponent();
            DoubleBuffered = true;
            BackColor = Color.ForestGreen;
            MouseDown += (_, __) => { Controller.clicked(this); };
        }

        public CardView(Card c) : base()
        {
            c.addObserver(this);
        }

        public void notify(CardChangedMessage t)
        {
            throw new NotImplementedException();
        }

        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Enabled = false;
            this.pictureBox1.Image = global::stonerkart.Properties.Resources.jordanno;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(741, 616);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // CardView
            // 
            this.Controls.Add(this.pictureBox1);
            this.Name = "CardView";
            this.Size = new System.Drawing.Size(741, 616);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
