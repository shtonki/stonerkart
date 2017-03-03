using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart
{
    class ToggleStopButton : Button
    {
        public bool stop;

        public ToggleStopButton(bool init, Action<bool> f)
        {
            stop = init;
            figureOutColour();

            Click += (_, __) =>
            {
                stop = !stop;
                figureOutColour();
                f(stop);
            };
        }

        public void figureOutColour()
        {
            BackColor = stop ? StepPanel.on : StepPanel.inactive;
            Invalidate();
        }
    }

    class StepPanelx : Panel
    {
        private Label label;
        public ToggleStopButton leftButton;
        public ToggleStopButton rightButton;

        public StepPanelx(Steps step, bool leftInit, bool rightInit)
        {
            label = new Label();
            label.BackColor = Color.FloralWhite;
            label.Text = step.ToString();
            label.TextAlign = ContentAlignment.TopCenter;
            Controls.Add(label);

            leftButton = new ToggleStopButton(leftInit, b => Settings.stopTurnSetting.setHeroTurnStop(step, b));
            Controls.Add(leftButton);

            rightButton = new ToggleStopButton(rightInit, b => Settings.stopTurnSetting.setVillainTurnStop(step, b));
            Controls.Add(rightButton);

            BackColorChanged += (_, __) => label.BackColor = BackColor;
            Resize += (_, __) => xd();
        }

        private void xd()
        {
            int w = Size.Width;
            int h = Size.Height;

            label.SetBounds(w/6, 0, 4*w/6, h);
            leftButton.SetBounds(0, 0, w/6, h);
            rightButton.SetBounds(5*w/6, 0, w/6, h);
        }
    }

    class StepPanel : UserControl, Observer<Steps>
    {
        public static Color on = Color.ForestGreen;
        public static Color off = Color.DarkRed;
        public static Color inactive = Color.DimGray;

        private StepPanelx[] panels;

        public StepPanel()
        {
            panels = new StepPanelx[(int)Steps.End + 1];
            StopTurnSetting v = Settings.stopTurnSetting;
            for (int i = 0; i < panels.Length; i++)
            {
                StepPanelx p = new StepPanelx((Steps)i, v.getHeroTurnStop((Steps)i), v.getVillainTurnStop((Steps)i));

                Controls.Add(p);
                panels[i] = p;
            }
            
            Resize += (_, __) => xd();
            notify(null, Steps.Replenish);
        }

        public void notify(object o, Steps t)
        {
            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].BackColor = i == (int) t ? StepPanel.on : StepPanel.inactive;
            }
        }

        private void xd()
        {
            int w = Size.Width;
            int h = Size.Height;
            int dh = h/panels.Length;

            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].SetBounds(0, i*dh, w, dh);
            }
        }

    }
}
