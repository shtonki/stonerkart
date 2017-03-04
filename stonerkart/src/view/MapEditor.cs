using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart
{
    class MapEditor : StickyPanel, Screen
    {
        private Button button1;
        private HexPanel hexPanel2;

        public MapEditor() 
        {
            InitializeComponent();
            //hexPanel2.callbacks.add(tileClicked);
        }

        public IEnumerable<MenuItem> getMenuPanel()
        {
            return new List<MenuItem>();
        }

        private void tileClicked(TileView tv)
        {
        }

        private void InitializeComponent()
        {
            this.hexPanel2 = new stonerkart.HexPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // hexPanel2
            // 
            this.hexPanel2.BackColor = System.Drawing.Color.Aqua;
            this.hexPanel2.Location = new System.Drawing.Point(12, 3);
            this.hexPanel2.Name = "hexPanel2";
            this.hexPanel2.Size = new System.Drawing.Size(843, 503);
            this.hexPanel2.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(390, 611);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MapEditor
            // 
            this.Controls.Add(this.button1);
            this.Controls.Add(this.hexPanel2);
            this.Name = "MapEditor";
            this.Size = new System.Drawing.Size(868, 731);
            this.ResumeLayout(false);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Controller.transitionToMainMenu();
        }
    }
}
