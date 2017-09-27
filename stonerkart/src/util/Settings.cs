using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart
{
    abstract class Setting
    {
        string Name { get; }

        public Setting(string name, string config)
        {
            Name = name;
            Configure(config);
        }

        public abstract void Configure(string s);
        public abstract void ToConfigurationString();
    }

    class StopTurnSetting : Setting
    {

        private bool[] HerosTurnStops;
        private bool[] VillainsTurnStops;

        public bool HasStop(TurnCounter tc) => hasStopEx(tc);

        public StopTurnSetting(string config) : base("StopTurn", config)
        {
            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
        }

        public override void Configure(string s)
        {
            throw new NotImplementedException();
        }

        public override void ToConfigurationString()
        {
            throw new NotImplementedException();
        }

        public bool hasStopEx(TurnCounter sc)
        {
            int ix = (int)sc.Step;
            bool[] stops = sc.ActivePlayer.isHero ? HerosTurnStops : VillainsTurnStops;
            return stops[ix];
        }
    }

    struct SettingJist
    {
        public string Name { get; }
        public string Content { get; }

        public SettingJist(string name, string content)
        {
            Name = name;
            Content = content;
        }
    }

    internal static class Settings
    {
        public static string decksPath { get; set; } = "./";

        public static StopTurnSetting stopTurnSetting { get; } = new StopTurnSetting(null);

        private static Setting[] settings =
        {
            stopTurnSetting,
        };

        private static string saveFileName = "settings";

        static Settings()
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
            {
                Logger.ShoutLine("wtf");
                return;
            }

            loadSettings();
        }

        public static IEnumerable<SettingJist> loadSettings()
        {
            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
        }

        public static void saveSettings()
        {
            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
        }
    }
}
