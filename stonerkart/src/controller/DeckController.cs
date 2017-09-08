using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace stonerkart
{
    class DeckController
    {
        public static void saveDeck(Deck d, string name)
        {
            using (FileStream f = File.Create(Settings.decksPath + name + ".jas"))
            {
                byte[] xd = Encoding.ASCII.GetBytes(d.toSaveText());
                f.Write(xd, 0, xd.Length);
                f.Close();
            }
        }

        public static IEnumerable<Deck> getDecks()
        {
            var localdecks = Directory.EnumerateFiles(Settings.decksPath)
                .Where(s => s.EndsWith(".jas"))
                .Select(s => s.Substring(s.LastIndexOf('/') + 1, s.Length - 6)).Select(loadDeck);
            return localdecks.Concat(Deck.basicDecks);

        }

        public static Deck loadDeck(string name)
        {
            using (StreamReader r = new StreamReader(File.Open(Settings.decksPath + name + ".jas", FileMode.Open)))
            {
                string s = r.ReadToEnd();
                Deck d =  new Deck(s);
                d.name = name;
                r.Close();
                return d;
            }
            
        }

        public static Deck chooseDeck()
        {
            return new Deck(CardTemplate.Prince_sIla,
                new[]
                {
                    CardTemplate.Solemn_sLotus,
                    CardTemplate.Solemn_sLotus,
                    CardTemplate.Solemn_sLotus,
                    CardTemplate.Solemn_sLotus,
                    CardTemplate.Solemn_sLotus,
                    CardTemplate.Rockhand_sEchion, 
                    CardTemplate.Rockhand_sEchion, 
                    CardTemplate.Rockhand_sEchion, 
                    CardTemplate.Rockhand_sEchion, 
                    CardTemplate.Rockhand_sEchion, 
                    CardTemplate.Rockhand_sEchion, 
                    CardTemplate.Rockhand_sEchion, 
                    CardTemplate.Rockhand_sEchion, 
                });

            Deck[] decks = getDecks().ToArray();
            PublicSaxophone sax = new PublicSaxophone(o => o is Deck);

            int panelheight = 800;
            int buttonheight = Math.Min(100, panelheight / decks.Length);

            Square allofit = new Square(400, panelheight);

            for (int i = 0; i < decks.Length; i++)
            {
                Deck d = decks[i];
                Button b = new Button(400, buttonheight);
                b.Text = d.name;
                b.Backcolor = Color.Silver;;
                b.Border = new SolidBorder(4, Color.Black);
                b.Y = i*buttonheight;
                b.clicked += a => sax.answer(d);
                allofit.addChild(b);
            }

            var winduh = new Winduh(allofit);
            GUI.frame.activeScreen.addWinduh(winduh);
            var deck = (Deck)sax.call();
            winduh.close();
            return deck;
        }

    }
}
