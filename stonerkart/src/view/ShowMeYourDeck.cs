using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using stonerkart;
using MenuItem = System.Windows.Forms.MenuItem;
using Screen = System.Windows.Forms.Screen;

namespace stonerkart.src.view
{
    public partial class ShowMeYourDeck : UserControl, Screen
    {

        public IEnumerable<MenuItem> getMenuPanel()
        {
            return new MenuItem[0];
        }

        public ShowMeYourDeck()
        {
            InitializeComponent();
        }

        private void backButtonClick(object sender, EventArgs e)
        {
            ScreenController.transitionToMainMenu();
        }

        private void refreshButtonClick(object sender, EventArgs e)
        {
            //requestDeckFromServer(List<Filters>, int rangeA, int rangeB)

        }
    }
}
