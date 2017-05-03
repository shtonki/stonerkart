using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart
{
    public partial class RankedScreen : UserControl, Screen
    {
        public RankedScreen()
        {
            InitializeComponent();
        }

        public IEnumerable<MenuItem> getMenuPanel()
        {
            return new MenuItem[0];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Network.matchmake();
            button1.Enabled = false;
        }
    }
}
