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
                return d;
            }
        }

        public static Deck chooseDeck()
        {
            ManualResetEventSlim re = new ManualResetEventSlim(false);
            Deck s = null;
            Thread t = new Thread(() => chooseDeck(v =>
            {
                s = v;
                re.Set();
            }));
            t.Start();
            re.Wait();

            return s;
        }

        public static void chooseDeck(Action<Deck> cb)
        {
            Deck d = new Deck(CardTemplate.Shibby_sShtank, 
                new []
                {
                    CardTemplate.Bubastis, 
                    CardTemplate.Bubastis, 
                    CardTemplate.Bubastis, 
                    CardTemplate.Bubastis, 
                    CardTemplate.Bubastis, 
                    CardTemplate.Bubastis, 
                    CardTemplate.Bubastis, 
                    CardTemplate.Bubastis, 
                    CardTemplate.Bubastis, 
                    CardTemplate.Bubastis, 
                    CardTemplate.Bubastis, 
                    CardTemplate.Bubastis, 
                    CardTemplate.Bubastis, 
                    CardTemplate.Bubastis, 
                }
                );
            cb(d);
            return;
            throw new Exception();
            /*
            Panel p = new Panel();
            p.BackColor = Color.Tomato;
            Deck[] decks = getDecks().ToArray();
            p.Size = new Size(100, 20 * decks.Length);
            DraggablePanel dp = null;
            for (int i = 0; i < decks.Length; i++)
            {
                Deck d = decks[i];
                Button b = new Button();
                b.Text = d.name;
                b.Click += (_, __) =>
                {
                    cb(d);
                    dp.close();
                };
                p.Controls.Add(b);
                b.BackColor = Color.Violet;
                b.SetBounds(0, i * 20, p.Size.Width, 20);
            }

            dp = UIController.showControl(p, true, false);
            */
        }

    }
}
