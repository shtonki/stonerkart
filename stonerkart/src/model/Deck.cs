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

        public Deck(string saveText)
        {
            string[] ss = saveText.Split(',');

            hero = parse(ss[0]);
            
            templates = new CardTemplate[ss.Length - 1];
            for (int i = 1; i < ss.Length; i++)
            {
                templates[i-1] = parse(ss[i]);
            }
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

        public string toSaveText()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(hero);
            sb.Append(',');

            foreach (var ct in templates)
            {
                sb.Append(ct);
                sb.Append(',');
            }
            sb.Length--;

            return sb.ToString();
        }

        private static CardTemplate[] test = new[]
        {
            CardTemplate.Alter_sFate, 
            CardTemplate.Alter_sFate, 
            CardTemplate.Alter_sFate, 
            CardTemplate.Alter_sFate, 
            CardTemplate.Primal_sChopter, 
            CardTemplate.Overgrow, 
            CardTemplate.Marilith, 
            CardTemplate.One_sWith_sNature, 
            CardTemplate.Water_sBolt, 
            CardTemplate.Wilt, 
        };

        #region basicDNZombies


        private static CardTemplate[] basicDNZombiesTs = new[]
        {
            CardTemplate.Risen_sAbberation,
            CardTemplate.Risen_sAbberation,
            CardTemplate.Risen_sAbberation,
            CardTemplate.Risen_sAbberation,
            CardTemplate.Chains_sof_sSin,
            CardTemplate.Chains_sof_sSin,
            CardTemplate.Ilas_sBargain,
            CardTemplate.Ilas_sBargain,
            CardTemplate.Ilas_sGravekeeper,
            CardTemplate.Ilas_sGravekeeper,
            CardTemplate.Ilas_sGravekeeper,
            CardTemplate.Ilatian_sHaunter,
            CardTemplate.Ilatian_sHaunter,
            CardTemplate.Ilatian_sHaunter,
            CardTemplate.Wilt,
            CardTemplate.Wilt,
            CardTemplate.Gleeful_sDuty,
            CardTemplate.Gleeful_sDuty,
            CardTemplate.Gleeful_sDuty,
            CardTemplate.Rider_sof_sDeath,
            CardTemplate.Ilatian_sHaunter,
            CardTemplate.Frenzied_sPirhana,
            CardTemplate.Frenzied_sPirhana,
            CardTemplate.Frenzied_sPirhana,
            CardTemplate.Frenzied_sPirhana,
            CardTemplate.Invigorate,
            CardTemplate.Invigorate,
            CardTemplate.Rider_sof_sFamine,
            CardTemplate.Survival_sInstincts,
            CardTemplate.Survival_sInstincts,
        };
        #endregion

        #region basicMLNZoo
        public static CardTemplate[] basicMLNZooTs = new[]{CardTemplate.Rockhand_sEchion,
CardTemplate.Rockhand_sEchion,
CardTemplate.Rockhand_sEchion,
CardTemplate.Primordial_sChimera,
CardTemplate.Primordial_sChimera,
CardTemplate.Primordial_sChimera,
CardTemplate.Primordial_sChimera,
CardTemplate.Marilith,
CardTemplate.Marilith,
CardTemplate.Rider_sof_sWar,
CardTemplate.Call_sTo_sArms,
CardTemplate.Call_sTo_sArms,
CardTemplate.Call_sTo_sArms,
CardTemplate.Call_sTo_sArms,
CardTemplate.Damage_sWard,
CardTemplate.Damage_sWard,
CardTemplate.Damage_sWard,
CardTemplate.Chains_sof_sVirtue,
CardTemplate.Chains_sof_sVirtue,
CardTemplate.Rapture,
CardTemplate.Rapture,
CardTemplate.Temple_sHealer,
CardTemplate.Temple_sHealer,
CardTemplate.Frenzied_sPirhana,
CardTemplate.Frenzied_sPirhana,
CardTemplate.Frenzied_sPirhana,
CardTemplate.Frenzied_sPirhana,
CardTemplate.Invigorate,
CardTemplate.Invigorate,
CardTemplate.One_sWith_sNature,
CardTemplate.One_sWith_sNature,
CardTemplate.Fresh_sFox,
CardTemplate.Fresh_sFox,
CardTemplate.Fresh_sFox,
CardTemplate.Overgrow,
CardTemplate.Survival_sInstincts,
CardTemplate.Survival_sInstincts,
};
        #endregion

        #region basicOLDRiders

        public static CardTemplate[] basicOLDRidersTs = new[]
        {
            CardTemplate.Chains_sof_sSin,
            CardTemplate.Ilas_sBargain,
            CardTemplate.Ilas_sBargain,
            CardTemplate.Wilt,
            CardTemplate.Wilt,
            CardTemplate.Gleeful_sDuty,
            CardTemplate.Gleeful_sDuty,
            CardTemplate.Raise_sDead,
            CardTemplate.Raise_sDead,
            CardTemplate.Rider_sof_sDeath,
            CardTemplate.Rider_sof_sWar,
            CardTemplate.Sinister_sPact,
            CardTemplate.Sinister_sPact,
            CardTemplate.Cantrip,
            CardTemplate.Cantrip,
            CardTemplate.Counterspell,
            CardTemplate.Counterspell,
            CardTemplate.Unmake,
            CardTemplate.Unmake,
            CardTemplate.Counterspell,
            CardTemplate.Suspicious_sVortex,
            CardTemplate.Kappa,
            CardTemplate.Chains_sof_sVirtue,
            CardTemplate.Rapture,
            CardTemplate.Abolish,
            CardTemplate.Gotterdammerung,
            CardTemplate.Yung_sLich,
            CardTemplate.Yung_sLich,
            CardTemplate.Graverobber_sSyrdin,
            CardTemplate.Rider_sof_sFamine,
            CardTemplate.Rider_sof_sPestilence,
        };
        #endregion

        #region basicJasinHomebrew

        public static CardTemplate[] basicJasinHomebrewTs = new[]
        {
            CardTemplate.Magma_sVents,
            CardTemplate.Magma_sVents,
            CardTemplate.Magma_sVents,
            CardTemplate.Magma_sVents,
            CardTemplate.Cleansing_sFire,
            CardTemplate.Cleansing_sFire,
            CardTemplate.Zap,
            CardTemplate.Zap,
            CardTemplate.Zap,
            CardTemplate.Zap,
            CardTemplate.Goblin_sGrenade,
            CardTemplate.Goblin_sGrenade,
            CardTemplate.Goblin_sGrenade,
            CardTemplate.Illegal_sGoblin_sLaboratory,
            CardTemplate.Illegal_sGoblin_sLaboratory,
            CardTemplate.Illegal_sGoblin_sLaboratory,
            CardTemplate.Sinister_sPact,
            CardTemplate.Sinister_sPact,
            CardTemplate.Sinister_sPact,
            CardTemplate.Sinister_sPact,
            CardTemplate.Cantrip,
            CardTemplate.Cantrip,
            CardTemplate.Cantrip,
            CardTemplate.Cantrip,
            CardTemplate.Unmake,
            CardTemplate.Unmake,
            CardTemplate.Unmake,
            CardTemplate.Unmake,
            CardTemplate.Counterspell,
            CardTemplate.Counterspell,
            CardTemplate.Counterspell,
            CardTemplate.Counterspell,
            CardTemplate.Kappa,
            CardTemplate.Kappa,
        };
        #endregion

        public static Deck[] basicDecks = new[]
        {
            new Deck(CardTemplate.Prince_sIla, basicDNZombiesTs, "Basic DN Zombies"),
            new Deck(CardTemplate.Chieftain_sZ_aloot_aboks, basicMLNZooTs, "Basic MLN Zoo"),
            new Deck(CardTemplate.Shibby_sShtank, basicOLDRidersTs, "Basic OLD Riders"),
            new Deck(CardTemplate.Chieftain_sZ_aloot_aboks, basicJasinHomebrewTs, "Basic Jasin Homebrew"),
            new Deck(CardTemplate.Shibby_sShtank, test, "Test Me"),
        };
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
