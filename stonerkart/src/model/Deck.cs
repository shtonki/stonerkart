using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    public enum Format
    {
        Test,
        Standard,
    }

    [Serializable]
    public class Deck
    {
        public CardTemplate hero;
        public CardTemplate[] templates;
        public string name { get; set; }

        public Deck(CardTemplate hero, CardTemplate[] templates)
        {
            this.hero = hero;
            this.templates = templates;
        }

        public Deck(CardTemplate hero, CardTemplate[] templates, string name)
        {
            this.hero = hero;
            this.templates = templates;
            this.name = name;
        }

        public Deck()
        {
            templates = new CardTemplate[0];
        }

        private CardTemplate parse(string s)
        {
            CardTemplate xd;

            if (!(CardTemplate.TryParse(s, out xd)))
            {
                throw new Exception("Deck contains invalid card name: " + s);
            }

            return xd;
        }

    }


    public class DeckContraints
    {
        public int cardMin { get; }
        public int this[Rarity r]
        {
            get
            {
                return limits[(int)r];
            }

            set
            {
                limits[(int)r] = value;
            }
        }

        private int[] limits;

        public DeckContraints(Format format)
        {
            limits = Enum.GetValues(typeof(Rarity)).Cast<Rarity>().Select(r => -1).ToArray();

            switch (format)
            {
                case Format.Test:
                {
                    cardMin = 5;

                    this[Rarity.Common]     = Int32.MaxValue;
                    this[Rarity.Uncommon]   = Int32.MaxValue;
                    this[Rarity.Rare]       = Int32.MaxValue;
                    this[Rarity.Legendary]  = Int32.MaxValue;
                    this[Rarity.None]  = Int32.MaxValue;
                } break;

                case Format.Standard:
                {
                    cardMin = 40;

                    this[Rarity.Common] = 4;
                    this[Rarity.Uncommon] = 3;
                    this[Rarity.Rare] = 2;
                    this[Rarity.Legendary] = 1;
                } break;

                default: throw new Exception();
            }
        }

        public bool testLegal(Deck d)
        {
            return testLegal(d.hero, d.templates);
        }

        public bool testLegal(CardTemplate heroic, CardTemplate[] deck, bool checkSize = true)
        {
            if (!Card.fromTemplate(heroic).isHeroic) return false;
            if (checkSize && deck.Length < cardMin) return false;

            ReduceResult<CardTemplate> rr = deck.Reduce();

            foreach (var ct in rr.values)
            {
                int cc = rr[ct];
                Card card = Card.fromTemplate(ct);

                if (card.isHeroic) return false;
                if (this[card.rarity] < cc) return false;
            }

            return true;
        }

        public bool willBeLegal(CardTemplate heroic, CardTemplate[] templates, CardTemplate add)
        {
            CardTemplate[] ts = templates.Select(_ => _).Concat(new[] { add }).ToArray();
            return testLegal(heroic, ts, false);
        }

        public bool willBeLegal(Deck d, CardTemplate add)
        {
            return willBeLegal(d.hero, d.templates, add);
        }
    }
}
