using System;
using System.Drawing;
using System.Windows.Forms;

namespace stonerkart
{
    class CardView : UserControl
    {
        private PictureBox pictureBox1;
        private static Random r = new Random();

        public CardView()
        {
            InitializeComponent();

            var v = new Color[]
            {
                Color.DimGray, Color.Gold,      Color.Gainsboro,
                Color.Wheat,   Color.Red,       Color.Violet,
                Color.Yellow,  Color.IndianRed, Color.ForestGreen,
            };


            BackColor = v[r.Next(8)];
            DoubleBuffered = true;

            Resize += (_, __) => layout();
            layout();
        }

        private void layout()
        {
            pictureBox1.Image = G.ResizeImage(pictureBox1.Image, Size.Width, Size.Height);
        }

        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::stonerkart.Properties.Resources.jordanno;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(144, 144);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // CardView
            // 
            this.Controls.Add(this.pictureBox1);
            this.Name = "CardView";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
