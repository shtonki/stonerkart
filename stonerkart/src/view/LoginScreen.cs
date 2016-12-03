using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class LoginScreen : StickyPanel, Screen
    {
        public LoginScreen()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // LoginScreen
            // 
            this.Name = "LoginScreen";
            this.Size = new System.Drawing.Size(694, 606);
            this.ResumeLayout(false);

        }
    }
}
