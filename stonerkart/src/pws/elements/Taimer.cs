using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{

    struct TimerSetting
    {
        public double baseTime { get; set; }
        public double timeBank { get; set; }
        public System.Drawing.Color barColor { get; set; }
        public System.Drawing.Color backColor { get; set; }

        public TimerSetting(double baseTime, double timeBank, System.Drawing.Color barColor, System.Drawing.Color backColor)
        {
            this.baseTime = baseTime;
            this.timeBank = timeBank;
            this.barColor = barColor;
            this.backColor = backColor;
        }
    }

    //todo: decide whether to use s or ms
    //todo: allow upright timer
    class Taimer : Square
    {
        const int GRAPHICAL_UPDATE_INTERVAL = 100;
        const int REAL_UPDATE_INTERVAL = 100;
        double barWidthReduction = 0;
        double barWidthReductionRemainder = 0;
        Square timerBar;
        double timeLeft = -1;
        double timeBankLeft = -1;
        TimerSetting timerSetting;

        public Taimer(int x, int y, int width, int height, TimerSetting ts) : base(x, y, width, height)
        {
            timerSetting = ts;
            timerBar = new Square(width/2, height/2, width/2, height/2, ts.barColor);
            this.addChild(timerBar);

            this.Backcolor = ts.backColor;
            this.timeLeft = ts.baseTime*1000;
            this.timeBankLeft = ts.timeBank;


            barWidthReduction = (timerBar.Width / (ts.baseTime * 1000 / GRAPHICAL_UPDATE_INTERVAL)) + barWidthReductionRemainder;
            Console.WriteLine(barWidthReduction);
            System.Timers.Timer visualTimer = new System.Timers.Timer(GRAPHICAL_UPDATE_INTERVAL);
            visualTimer.Start();
            visualTimer.Elapsed += (_,__) =>
            {
                //barWidthReduction = (timerBar.Width / (ts.baseTime * 1000 / GRAPHICAL_UPDATE_INTERVAL)) + barWidthReductionRemainder; LOL
                if (updateBar() == false) visualTimer.Stop();
            };

            System.Timers.Timer realTimer = new System.Timers.Timer(REAL_UPDATE_INTERVAL);
            realTimer.Start();
            realTimer.Elapsed += (_, __) =>
            {
                if (updateTimeLeft() == false)
                {
                    realTimer.Stop();
                    Console.WriteLine("Times up! Activating TimeBank!");
                }
            };
        }

        private bool updateTimeLeft()
        {
            timeLeft -= REAL_UPDATE_INTERVAL;
            Console.WriteLine("time left: " + timeLeft);
            if (timeLeft < 0) return false;
            return true;
        }

        private bool updateBar()
        {
            int reduction = (int)Math.Floor(barWidthReduction + barWidthReductionRemainder);
            barWidthReductionRemainder = barWidthReduction + barWidthReductionRemainder - reduction;
            if (timerBar.Width - reduction < 0)
            {
                timerBar.Width = 0;
                return false;
            }
            timerBar.Width -= reduction;
            return true;
        }

        public void resetToBaseTime()
        {
            timeLeft = timerSetting.baseTime;
        }

        public void addTimeBank(double amount)
        {
            timeBankLeft += amount;
        }
    }
}
