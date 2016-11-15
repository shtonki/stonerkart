using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using kappa = System.Tuple<System.Windows.Forms.Control, float, float, float, float>;

namespace stonerkart
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof (IDesigner))]
    class StickyPanel : UserControl
    {
        public StickyPanel()
        {
            //Dock = DockStyle.Fill;
            BackColor = Color.DarkViolet;
        }

        private List<kappa> t;

        public new void ResumeLayout(bool b)
        {
            t = new List<kappa>();
            foreach (Control c in Controls) xd(c);
        }

        private void xd(Control v)
        {
            float w = Size.Width;
            float h = Size.Height;
            t.Add(new kappa(
                v,
                v.Left / w,
                v.Width / w,
                v.Top / h,
                v.Height / h
                ));
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (t == null) return;
            int w = Size.Width;
            int h = Size.Height;
            for (int i = 0; i < t.Count; i++)
            {
                if (t[i].Item1 != Controls[i]) throw new Exception();
                Controls[i].Left = (int)(t[i].Item2*w);
                Controls[i].Width = (int)(t[i].Item3*w);
                Controls[i].Top =  (int)(t[i].Item4*h);
                Controls[i].Height = (int)(t[i].Item5*h);
            }
        }
    }
}
