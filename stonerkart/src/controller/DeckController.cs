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
        #region fuck
        private static CardTemplate[] test = new[]
        {
            CardTemplate.Confuse,
            CardTemplate.Confuse,
            CardTemplate.Confuse,
            CardTemplate.Confuse,
            CardTemplate.Confuse,
            CardTemplate.Confuse,
            CardTemplate.Hungry_sFelhound,
            CardTemplate.Hungry_sFelhound,
            CardTemplate.Hungry_sFelhound,
            CardTemplate.Hungry_sFelhound,
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
        #endregion

        public static List<Deck> Decks => Settings.DecksSetting.Decks;

        public static void saveDeck(Deck deck, string deckname)
        {
            //todo 260917 confirm before overwriting
            deck.name = deckname;
            if (1 < Decks.RemoveAll(d => d.name == deckname)) throw new Exception();
            Decks.Add(deck);
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
