using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart
{
    class LoginScreen : StickyPanel, Screen
    {
        private System.Windows.Forms.TextBox usernameBox;
        private System.Windows.Forms.TextBox passwordBox;
        private AutoFontTextBox autoFontTextBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private AutoFontTextBox autoFontTextBox1;

        public LoginScreen()
        {
            InitializeComponent();
            autoFontTextBox1.Text = "Username";
            autoFontTextBox2.Text = "Password";
        }

        public IEnumerable<MenuItem> getMenuPanel()
        {
            return new List<MenuItem>();
        }

        private void InitializeComponent()
        {
            this.usernameBox = new System.Windows.Forms.TextBox();
            this.passwordBox = new System.Windows.Forms.TextBox();
            this.autoFontTextBox1 = new stonerkart.AutoFontTextBox();
            this.autoFontTextBox2 = new stonerkart.AutoFontTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // usernameBox
            // 
            this.usernameBox.Location = new System.Drawing.Point(175, 176);
            this.usernameBox.Name = "usernameBox";
            this.usernameBox.Size = new System.Drawing.Size(322, 20);
            this.usernameBox.TabIndex = 0;
            // 
            // passwordBox
            // 
            this.passwordBox.Location = new System.Drawing.Point(175, 308);
            this.passwordBox.Name = "passwordBox";
            this.passwordBox.PasswordChar = '*';
            this.passwordBox.Size = new System.Drawing.Size(322, 20);
            this.passwordBox.TabIndex = 1;
            // 
            // autoFontTextBox1
            // 
            this.autoFontTextBox1.BackColor = System.Drawing.Color.Transparent;
            this.autoFontTextBox1.Location = new System.Drawing.Point(175, 110);
            this.autoFontTextBox1.Name = "autoFontTextBox1";
            this.autoFontTextBox1.Opacity = 100;
            this.autoFontTextBox1.Size = new System.Drawing.Size(322, 60);
            this.autoFontTextBox1.TabIndex = 2;
            // 
            // autoFontTextBox2
            // 
            this.autoFontTextBox2.BackColor = System.Drawing.Color.Transparent;
            this.autoFontTextBox2.Location = new System.Drawing.Point(175, 242);
            this.autoFontTextBox2.Name = "autoFontTextBox2";
            this.autoFontTextBox2.Opacity = 100;
            this.autoFontTextBox2.Size = new System.Drawing.Size(322, 60);
            this.autoFontTextBox2.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(175, 383);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(151, 74);
            this.button1.TabIndex = 4;
            this.button1.Text = "Log In";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(346, 383);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(151, 74);
            this.button2.TabIndex = 5;
            this.button2.Text = "Register";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(175, 475);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(322, 76);
            this.button3.TabIndex = 6;
            this.button3.Text = "Play Offwayne";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // LoginScreen
            // 
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.autoFontTextBox2);
            this.Controls.Add(this.autoFontTextBox1);
            this.Controls.Add(this.passwordBox);
            this.Controls.Add(this.usernameBox);
            this.Name = "LoginScreen";
            this.Size = new System.Drawing.Size(694, 606);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Network.login(usernameBox.Text, passwordBox.Text))
            {
                ScreenController.transitionToMainMenu();
            }
            else
            {
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Network.register(usernameBox.Text, passwordBox.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ScreenController.transitionToMainMenu();
        }
    }
}
