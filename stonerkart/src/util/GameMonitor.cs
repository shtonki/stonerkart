using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart
{
    partial class GameMonitor : Form
    {
        private Game game;

        public GameMonitor(Game g)
        {
            InitializeComponent();

            Timer t = new Timer();
            t.Interval = 100;
            t.Tick += (_, __) => memeout(g);
            t.Start();
        }

        private void memeout(Game g)
        {
            Console.WriteLine("xd");
            StringBuilder sb = new StringBuilder();

            foreach (var c in g.gameState.hero.deck)
            {
                sb.AppendLine(c.name);
            }

            richTextBox1.Text = sb.ToString();
        }
    }
}
