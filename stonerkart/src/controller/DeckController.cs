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
    static class DeckController
    {
        private static List<Deck> decks;
        public static IEnumerable<Deck> Decks => decks;

        static DeckController()
        {
            decks = new List<Deck>();
            decks.AddRange(Deck.basicDecks);
        }

        public static void saveDeck(Deck deck, string deckname)
        {
            //todo 260917 confirm before overwriting
            deck.name = deckname;
            if (1 < decks.RemoveAll(d => d.name == deckname)) throw new Exception();
            decks.Add(deck);
        }

        public static Deck chooseDeck()
        {
            Deck[] decks = Decks.ToArray();
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
