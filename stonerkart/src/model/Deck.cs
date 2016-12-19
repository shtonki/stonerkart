using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    enum Format
    {
        Test,
        Standard,
    }

    class Deck
    {
        public CardTemplate heroic;
        public CardTemplate[] deck;

        public Deck(CardTemplate heroic, CardTemplate[] deck)
        {
            this.heroic = heroic;
            this.deck = deck;
        }

        private bool testLegal(Format format)
        {
            DeckContraints limit = new DeckContraints(format);

            if (deck.Length < limit.cardMin) return false;

            ReduceResult<CardTemplate> rr = deck.Reduce();

            foreach (var ct in rr.values)
            {
                int cc = rr[ct];
                Card card = Card.fromTemplate(ct);

                if (card.isHeroic) return false;
                if (limit[card.rarity] > cc) return false;
            }

            return true;
        }

        private class DeckContraints
        {
            public int cardMin;
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

                    } break;

                    case Format.Standard:
                    {
                        cardMin = 30;

                        this[Rarity.Common] = 4;
                        this[Rarity.Uncommon] = 3;
                        this[Rarity.Rare] = 2;
                        this[Rarity.Legendary] = 1;
                    } break;

                    default: throw new Exception();
                }
            }
        }
    }
}
