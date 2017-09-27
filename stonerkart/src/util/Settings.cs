using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace stonerkart
{
    [Serializable]
    public abstract class Setting
    {
        public string Name { get; protected set; }
    }

    [Serializable]
    public class DecksSetting : Setting
    {
        public List<Deck> Decks { get; private set; }

        public DecksSetting()
        {
            Name = "Decks";
            Decks = new List<Deck>();
        }

    }

    internal static class Settings
    {
        private static Setting[] settings;
        private static BinaryFormatter bf = new BinaryFormatter();
        public static DecksSetting DecksSetting => (DecksSetting)settings.First(setting => setting is DecksSetting);

        private static string saveFileName = "settings";

        static Settings()
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
            {
                Logger.ShoutLine("wtf");
                return;
            }

            var bytes = File.ReadAllBytes(saveFileName);
            MemoryStream ms = new MemoryStream(bytes);
            var settingsarray = (Setting[])bf.Deserialize(ms);
            settings = settingsarray;
        }

        public static void SaveSettings()
        {
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, settings);
            ms.Seek(0, SeekOrigin.Begin);
            File.WriteAllBytes(saveFileName, ms.ToArray());
        }
    }
}
