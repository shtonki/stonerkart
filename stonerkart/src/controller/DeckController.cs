using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        public static string[] getDeckNames()
        {
            return Directory.EnumerateFiles(Settings.decksPath)
                .Where(s => s.EndsWith(".jas"))
                .Select(s => s.Substring(s.LastIndexOf('/') + 1, s.Length - 6))
                .ToArray();
        }

        public static Deck loadDeck(string name)
        {
            using (StreamReader r = new StreamReader(File.Open(Settings.decksPath + name + ".jas", FileMode.Open)))
            {
                string s = r.ReadToEnd();
                return new Deck(s);
            }
        }

        public static Deck chooseDeck()
        {
            ManualResetEventSlim re = new ManualResetEventSlim(false);
            string s = null;
            Thread t = new Thread(() => chooseDeck(v =>
            {
                s = v;
                re.Set();
            }));
            t.Start();
            re.Wait();

            return loadDeck(s);
        }

        public static void chooseDeck(Action<string> cb)
        {
            Panel p = new Panel();
            p.BackColor = Color.Tomato;
            string[] decknames = getDeckNames();
            p.Size = new Size(100, 20 * decknames.Length);
            DraggablePanel dp = null;
            for (int i = 0; i < decknames.Length; i++)
            {
                string s = decknames[i];
                Button b = new Button();
                b.Text = s;
                b.Click += (_, __) =>
                {
                    cb(s);
                    dp.close();
                };
                p.Controls.Add(b);
                b.BackColor = Color.Violet;
                b.SetBounds(0, i * 20, p.Size.Width, 20);
            }

            dp = UIController.showControl(p, true, false);
        }
    }
}
