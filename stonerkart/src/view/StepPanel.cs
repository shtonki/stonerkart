using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart
{

    class StepPanel : UserControl, Observer<Steps>
    {
        private Label[] labels;

        public StepPanel()
        {
            labels = new Label[(int)Steps.End + 1];

            for (int i = 0; i < labels.Length; i++)
            {
                Label p = new Label();
                p.BackColor = Color.Maroon;
                p.Text = ((Steps)i).ToString();
                p.TextAlign = ContentAlignment.TopCenter;
                Controls.Add(p);
                labels[i] = p;
            }

            Resize += (_, __) => xd();
        }

        public void notify(Steps t)
        {
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i].BackColor = i == (int) t ? Color.ForestGreen : Color.Maroon;
            }
        }

        private void xd()
        {
            int w = Size.Width;
            int h = Size.Height;
            int dh = h/labels.Length;

            for (int i = 0; i < labels.Length; i++)
            {
                labels[i].SetBounds(0, i*dh, w, dh);
            }
        }

    }
}
