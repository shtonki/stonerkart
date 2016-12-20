﻿using System;
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
        public CardTemplate[] templates;

        public Deck(CardTemplate heroic, CardTemplate[] templates)
        {
            this.heroic = heroic;
            this.templates = templates;
        }
    }


    class DeckContraints
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

        public bool testLegal(Deck d)
        {
            return testLegal(d.heroic, d.templates);
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
            return willBeLegal(d.heroic, d.templates, add);
        }
    }
}
