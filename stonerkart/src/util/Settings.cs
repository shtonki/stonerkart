﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{

    abstract class Setting
    {
        public bool isDefault;
        public string settingName => settingname();


        public Setting()
        {
            isDefault = true;
        }

        public override string ToString()
        {
            return tostring();
        }

        public abstract void FromString(string s);
        protected abstract string tostring();
        protected abstract string settingname();
    }

    class StopTurnSetting : Setting
    {
        private bool[] heroTurnStop { get; set; }
        private bool[] villainTurnStop { get; set; }

        public override void FromString(string v)
        {
            setupArrays(false);

            string[] ss = v.Split(',');
            if (ss[0].Length == 0) return;

            foreach (string s in ss)
            {
                if (s.Length != 2) throw new Exception();

                bool[] a;

                if (s[0] == 'h')
                {
                    a = heroTurnStop;
                }
                else if (s[0] == 'v')
                {
                    a = villainTurnStop;
                }
                else throw new Exception();

                int ix = Int32.Parse("" + s[1]);

                a[ix] = true;
            }
        }

        protected override string tostring()
        {
            if (heroTurnStop.Length != villainTurnStop.Length) throw new Exception();

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < heroTurnStop.Length; i++)
            {
                if (heroTurnStop[i]) sb.Append("h" + i + ",");
                if (villainTurnStop[i]) sb.Append("v" + i + ",");
            }

            if (sb.Length > 0) sb.Length--;

            return sb.ToString();
        }

        protected override string settingname()
        {
            return "STOPTURN";
        }

        public void setHeroTurnStop(Steps step, bool stop)
        {
            heroTurnStop[(int)step] = stop;
        }

        public void setVillainTurnStop(Steps step, bool stop)
        {
            villainTurnStop[(int)step] = stop;
        }

        public bool getHeroTurnStop(Steps step)
        {
            return heroTurnStop[(int)step];
        }

        public bool getVillainTurnStop(Steps step)
        {
            return villainTurnStop[(int)step];
        }

        public bool getTurnStop(Steps step, bool isHero)
        {
            bool[] a;
            if (isHero)
            {
                a = heroTurnStop;
            }
            else
            {
                a = villainTurnStop;
            }
            return a[(int)step];
        }

        private void setupArrays(bool initv)
        {
            List<Steps> l = Enum.GetValues(typeof(Steps)).Cast<Steps>().ToList(); //autogenerated resharper fuckery
            heroTurnStop = l.Select(s => initv).ToArray();
            villainTurnStop = l.Select(s => initv).ToArray();
        }
    }

    internal static class Settings
    {
        public static string decksPath { get; set; } = "./";

        public static StopTurnSetting stopTurnSetting { get; } = new StopTurnSetting();

        private static Dictionary<string, Setting> settingDict;
        private static Setting[] settings =
        {
            stopTurnSetting,
        };

        private static string saveFileName = "settings";

        static Settings()
        {
            settingDict = new Dictionary<string, Setting>();

            foreach (Setting s in settings)
            {
                settingDict[s.settingName] = s;
            }

            loadSettings();
        }

        public static void loadSettings()
        {
            if (!File.Exists(saveFileName))
            {
                FileStream fs = File.Create(saveFileName);
                byte[] bs = Encoding.ASCII.GetBytes("# lines starting with a # are treated as commens.");
                fs.Write(bs, 0, bs.Length);
            }
            string s = File.ReadAllText(saveFileName);
            string[] ss = s.Split('\n');

            foreach (string st in ss)
            {
                string setting = st.Trim();
                if (setting.Length == 0 || setting[0] == '#') continue;
                string[] w = setting.Split('=');
                string settingName = w[0];
                string settingValue = w[1];
                settingDict[settingName].FromString(settingValue);
            }
        }

        public static void saveSettings()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Setting s in settings)
            {
                sb.Append(s.settingName);
                sb.Append("=");
                sb.Append(s);
                sb.Append("\n");
            }

            File.WriteAllText(saveFileName, sb.ToString());
        }

        private static void loadSettingsEx(string s)
        {
            throw new NotImplementedException();
        }
    }
}
