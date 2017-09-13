using System;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace stonerkart
{
    class Designer : Form
    {

        public Designer()
        {
            InitializeComponent();
            wallabar1.f = i =>
            {
                active?.setLocation(active.X+i, active.Y);
                updatememe();
            };

            wallabar2.f = i =>
            {
                active?.setLocation(active.X, active.Y + i);
                updatememe();
            };

            wallabar3.f = i =>
            {
                active?.setSize(active.Width + i, active.Height);
                updatememe();
            };

            wallabar4.f = i =>
            {
                active?.setSize(active.Width, active.Height + i);
                updatememe();
            };
        }

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.wallabar4 = new wallabar();
            this.wallabar3 = new wallabar();
            this.wallabar2 = new wallabar();
            this.wallabar1 = new wallabar();
            ((System.ComponentModel.ISupportInitialize)(this.wallabar4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wallabar3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wallabar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wallabar1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "label2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 25);
            this.label3.TabIndex = 2;
            this.label3.Text = "label3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 190);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 25);
            this.label4.TabIndex = 3;
            this.label4.Text = "label4";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(17, 299);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // wallabar4
            // 
            this.wallabar4.f = null;
            this.wallabar4.Location = new System.Drawing.Point(128, 190);
            this.wallabar4.Maximum = 1000;
            this.wallabar4.Name = "wallabar4";
            this.wallabar4.Size = new System.Drawing.Size(414, 45);
            this.wallabar4.TabIndex = 7;
            this.wallabar4.Value = 500;
            // 
            // wallabar3
            // 
            this.wallabar3.f = null;
            this.wallabar3.Location = new System.Drawing.Point(128, 119);
            this.wallabar3.Maximum = 1000;
            this.wallabar3.Name = "wallabar3";
            this.wallabar3.Size = new System.Drawing.Size(414, 45);
            this.wallabar3.TabIndex = 6;
            this.wallabar3.Value = 500;
            // 
            // wallabar2
            // 
            this.wallabar2.f = null;
            this.wallabar2.Location = new System.Drawing.Point(128, 68);
            this.wallabar2.Maximum = 1000;
            this.wallabar2.Name = "wallabar2";
            this.wallabar2.Size = new System.Drawing.Size(414, 45);
            this.wallabar2.TabIndex = 5;
            this.wallabar2.Value = 500;
            // 
            // wallabar1
            // 
            this.wallabar1.f = null;
            this.wallabar1.Location = new System.Drawing.Point(128, 9);
            this.wallabar1.Maximum = 1000;
            this.wallabar1.Name = "wallabar1";
            this.wallabar1.Size = new System.Drawing.Size(414, 45);
            this.wallabar1.TabIndex = 4;
            this.wallabar1.Value = 500;
            // 
            // Designer
            // 
            this.ClientSize = new System.Drawing.Size(554, 349);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.wallabar4);
            this.Controls.Add(this.wallabar3);
            this.Controls.Add(this.wallabar2);
            this.Controls.Add(this.wallabar1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Designer";
            ((System.ComponentModel.ISupportInitialize)(this.wallabar4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wallabar3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wallabar2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wallabar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public GuiElement active;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private wallabar wallabar1;
        private wallabar wallabar2;
        private wallabar wallabar3;
        private wallabar wallabar4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button activeButton;

        public void setActive(GuiElement ge)
        {
            active = ge;
            updatememe();
        }

        public void updatememe()
        {
            if (active == null) return;

            label1.Text = "X: " + active.X.ToString();
            label2.Text = "Y: " + active.Y.ToString();
            label3.Text = "W: " + active.Width.ToString();
            label4.Text = "H: " + active.Height.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (active == null) return;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(".X = " + active.X + ";");
            sb.AppendLine(".Y = " + active.Y + ";");
            sb.AppendLine(".Width = " + active.Width + ";");
            sb.AppendLine(".Height = " + active.Height + ";");

            
            Thread thread = new Thread(() => Clipboard.SetText(sb.ToString()));
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join(); //Wait for the thread to end
        }
    }

    class wallabar : TrackBar
    {
        public wallabar()
        {
            Maximum = 1000;
            cached = Value = 500;

            Scroll += (sender, args) =>
            {
                f?.Invoke(Value - cached);
                cached = Value;
            };
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Value = cached = 500;
        }

        public Action<int> f { get; set; }
        private int cached;
    }
}