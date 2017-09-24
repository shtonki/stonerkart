using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using stonerkart.Properties;

namespace stonerkart
{
    public enum Textures
    {
        font0,
        font1,

        background3,
        background0,

        buttonbg0,
        buttonbg2,

        iconChallenge,
        iconFriends,
        iconAddFriends,
        iconShekel,
        iconCheck,
        iconCross,

        packFirstEdition12Pack,
        packFirstEdition40Pack,

        border0,

        table0,

        cardframegrey,

        setFirstedition,

        deckButton,
        handButton,
        displaceButton,
        graveyardButton,

        orbChaos,
        orbNature,
        orbOrder,
        orbLife,
        orbDeath,
        orbColourless,
        orbMight,
        orbColourless0,
        orbColourless1,
        orbColourless2,
        orbColourless3,
        orbColourless4,
        orbColourless5,
        orbColourless6,
        orbColourless7,
        orbColourless8,
        orbColourless9,
        orbColourless10,
        orbColourless11,
        orbColourless12,

        artBigMonkey,
        artSinisterPact,
        artGreatWhiteBuffalo,
        artAlterFate2,
        artSoothsay,
        artAberrantSacrifice,
        artAbolish,
        artAlterFate,
        artAlterTime,
        artAncientChopter,
        artAncientDruid,
        artBearCavalary,
        artBelwas,
        artBenedictor,
        artBloodclaw,
        artBubastis,
        artCallToArms,
        artCallToArmsAlternate,
        artCerebus,
        artChainsofSin,
        artChainsofVirtue,
        artChieftainZlootbox,
        artChimera,
        artChromaticUnicorn,
        artCleansingFire,
        artCounterspell,
        artCoupDeGrace,
        artDamageWard,
        artDaringPoppy,
        artDeath,
        artDecayingZombie,
        artDragonHatchling,
        artElderTreeant,
        artElvenCultivator,
        artElvenDruid,
        artEnragedDragon,
        artEssenceOfClarity,
        artEssenceOfDemise,
        artEssenceOfRage,
        artEvolveFangs,
        artExtinguish,
        artFacelessSphinx,
        artFamine,
        artFeralImp,
        artFigment,
        artFireheartPheonix,
        artFlamekindler,
        artFlamemane,
        artForkedLightning,
        artFrenziedPiranha,
        artFreshFox,
        artFrothingGoblin,
        artFuryOfTheRighteous,
        artGleefulDuty,
        artGoblinGrenade,
        artGottedammerung,
        artGraverobberSyrdin,
        artGrazingBison,
        artGrizzlyBear,
        artGrizzlyCub,
        artGryphon,
        artGryphonRider,
        artHauntedChapel,
        artHeroicMight,
        artHoundmaster,
        artHourOfTheWolf,
        artHuntress,
        artHypnotist,
        artIlasGambit,
        artIlasGravekeeper,
        artIlatianFlutePlayer,
        artIlatianHaunter,
        artIlatianWineMerchant,
        artInfiltrator,
        artInvigorate,
        artKappa,
        artKraken,
        artLoneRanger,
        artLoneWolf,
        artLordIla,
        artLordPlevin,
        artMagmaVents,
        artMaleficentSpirit,
        artMarilith,
        artMattysGambit,
        artMeteorRain,
        artMoratianBattleStandard,
        artMorenianMedic,
        artMysteriousLilac,
        artNothing,
        artOneWithNature,
        artOvergrow,
        artPestilence,
        artPropheticVision,
        artPyrostorm,
        artRaiseDead,
        artRapture,
        artRavagingGreed,
        artRelentlessConscriptor,
        artResoundingBlast,
        artRiderOfDeath,
        artRockhandOgre,
        artRockToken,
        artRottingZombie,
        artSanguineArtisan,
        artScrollOfEarth,
        artSebasGambit,
        artSeethingRage,
        artSeraph,
        artSereneDandelion,
        artShimmeringKoi,
        artShottyContruct,
        artSkeltal,
        artSolemnAberration,
        artSolemnLotus,
        artSpark,
        artSparryz,
        artSpirit,
        artSpiritOfSalvation,
        artSquire,
        artStampedingDragon,
        artStarkLily,
        artSteamBolt,
        artSurvivalInstincts,
        artSuspiciousVortex,
        artTaouyRuins,
        artTarantula,
        artTeleport,
        artTempleHealer,
        artTerminate,
        artUnmake,
        artUnstableMemeExperiment,
        artUnyeildingStalwart,
        artVibrantZinnia,
        artWar,
        artWaterBolt,
        artWilt,
        artWolf,
        artWolf1,
        artYungLich,
        artZap,
        artZombieToken,
        artIradj,
        artSeblastian,
        artArachosa,
        artSpiderling,
        artCount_sFera_sII,
        artWarp,
        artHosro,
        artJabroni,
        artParalyzing_sSpider,
        artMakaroni,
        artArchfather,
        artHungry_sFelhound,
        artVincennes,
        artIlatian_sGhoul,
    };

    public static class TextureLoader
    {
        public static Dictionary<Textures, Image> images { get; }
        public static Dictionary<Textures, Size> sizes { get; }

        static TextureLoader()
        {
            images = new Dictionary<Textures, Image>();

            images[Textures.font0] = Resources.font0;
            images[Textures.font1] = Resources.font11;

            images[Textures.background3] = Resources.background3;
            images[Textures.background0] = Resources.background0;

            images[Textures.buttonbg0] = Resources.buttonbg0;
            images[Textures.buttonbg2] = Resources.buttonbg2;

            images[Textures.iconChallenge] = Resources.buttonChallenge;
            images[Textures.iconFriends] = Resources.buttonFriends;
            images[Textures.iconAddFriends] = Resources.buttonAddFriend;
            images[Textures.iconShekel] = Resources.iconShekel;
            images[Textures.iconCheck] = Resources.iconCheck;
            images[Textures.iconCross] = Resources.iconCross;

            images[Textures.packFirstEdition12Pack] = Resources.firstedition12cardpack;
            images[Textures.packFirstEdition40Pack] = Resources.firstedition40cardpack;

            images[Textures.border0] = Resources.border0;

            images[Textures.table0] = Resources.table0;

            images[Textures.graveyardButton] = Resources.buttonGraveyard;
            images[Textures.displaceButton] = Resources.buttonExile;
            images[Textures.handButton] = Resources.buttonHand;
            images[Textures.deckButton] = Resources.buttonDeck;

            images[Textures.cardframegrey] = Resources.whiteframe4;

            images[Textures.setFirstedition] = Resources.setFirstedition;

            images[Textures.orbChaos] = Resources.orbChaos;
            images[Textures.artBigMonkey] = Resources.artBigMonkey;
            images[Textures.orbNature] = Resources.orbNature;
            images[Textures.orbOrder] = Resources.orbOrder;
            images[Textures.orbLife] = Resources.orbLife;
            images[Textures.orbDeath] = Resources.orbDeath;
            images[Textures.orbColourless] = Resources.orbColourless;
            images[Textures.orbMight] = Resources.orbMight;
            images[Textures.orbColourless0] = Resources.cl0;
            images[Textures.orbColourless1] = Resources.cl1;
            images[Textures.orbColourless2] = Resources.cl2;
            images[Textures.orbColourless3] = Resources.cl3;
            images[Textures.orbColourless4] = Resources.cl4;
            images[Textures.orbColourless5] = Resources.cl5;
            images[Textures.orbColourless6] = Resources.cl6;
            images[Textures.orbColourless7] = Resources.cl7;
            images[Textures.orbColourless8] = Resources.cl8;
            images[Textures.orbColourless9] = Resources.cl9;
            images[Textures.orbColourless10] = Resources.cl10;
            images[Textures.orbColourless11] = Resources.cl11;
            images[Textures.orbColourless12] = Resources.cl12;

            images[Textures.artSoothsay] = Resources.artSoothsay;
            images[Textures.artGreatWhiteBuffalo] = Resources.artGreatWhiteBuffalo;
            images[Textures.artAlterFate2] = Resources.artAlterFate2;
            images[Textures.artSinisterPact] = Resources.artSinisterPact;
            images[Textures.artAberrantSacrifice] = Resources.artAberrantSacrifice;
            images[Textures.artAbolish] = Resources.artAbolish;
            images[Textures.artAlterFate] = Resources.artAlterFate;
            images[Textures.artAlterTime] = Resources.artAlterTime;
            images[Textures.artAncientChopter] = Resources.artAncientChopter;
            images[Textures.artAncientDruid] = Resources.artAncientDruid;
            images[Textures.artBearCavalary] = Resources.artBearCavalary;
            images[Textures.artBelwas] = Resources.artBelwas;
            images[Textures.artBenedictor] = Resources.artBenedictor;
            images[Textures.artBloodclaw] = Resources.artBloodclaw;
            images[Textures.artBubastis] = Resources.artBubastis;
            images[Textures.artCallToArms] = Resources.artCallToArms;
            images[Textures.artCallToArmsAlternate] = Resources.artCallToArmsAlternate;
            images[Textures.artCerebus] = Resources.artCerebus;
            images[Textures.artChainsofSin] = Resources.artChainsofSin;
            images[Textures.artChainsofVirtue] = Resources.artChainsofVirtue;
            images[Textures.artChieftainZlootbox] = Resources.artChieftainZlootbox;
            images[Textures.artChimera] = Resources.artChimera;
            images[Textures.artChromaticUnicorn] = Resources.artChromaticUnicorn;
            images[Textures.artCleansingFire] = Resources.artCleansingFire;
            images[Textures.artCounterspell] = Resources.artCounterspell;
            images[Textures.artCoupDeGrace] = Resources.artCoupDeGrace;
            images[Textures.artDamageWard] = Resources.artDamageWard;
            images[Textures.artDaringPoppy] = Resources.artDaringPoppy;
            images[Textures.artDeath] = Resources.artDeath;
            images[Textures.artDecayingZombie] = Resources.artDecayingZombie;
            images[Textures.artDragonHatchling] = Resources.artDragonHatchling;
            images[Textures.artElderTreeant] = Resources.artElderTreeant;
            images[Textures.artElvenCultivator] = Resources.artElvenCultivator;
            images[Textures.artElvenDruid] = Resources.artElvenDruid;
            images[Textures.artEnragedDragon] = Resources.artEnragedDragon;
            images[Textures.artEssenceOfClarity] = Resources.artEssenceOfClarity;
            images[Textures.artEssenceOfDemise] = Resources.artEssenceOfDemise;
            images[Textures.artEssenceOfRage] = Resources.artEssenceOfRage;
            images[Textures.artEvolveFangs] = Resources.artEvolveFangs;
            images[Textures.artExtinguish] = Resources.artExtinguish;
            images[Textures.artFacelessSphinx] = Resources.artFacelessSphinx;
            images[Textures.artFamine] = Resources.artFamine;
            images[Textures.artFeralImp] = Resources.artFeralImp;
            images[Textures.artFigment] = Resources.artFigment;
            images[Textures.artFireheartPheonix] = Resources.artFireheartPheonix;
            images[Textures.artFlamekindler] = Resources.artFlamekindler;
            images[Textures.artFlamemane] = Resources.artFlamemane;
            images[Textures.artForkedLightning] = Resources.artForkedLightning;
            images[Textures.artFrenziedPiranha] = Resources.artFrenziedPiranha;
            images[Textures.artFreshFox] = Resources.artFreshFox;
            images[Textures.artFrothingGoblin] = Resources.artFrothingGoblin;
            images[Textures.artFuryOfTheRighteous] = Resources.artFuryOfTheRighteous;
            images[Textures.artGleefulDuty] = Resources.artGleefulDuty;
            images[Textures.artGoblinGrenade] = Resources.artGoblinGrenade;
            images[Textures.artGottedammerung] = Resources.artGottedammerung;
            images[Textures.artGraverobberSyrdin] = Resources.artGraverobberSyrdin;
            images[Textures.artGrazingBison] = Resources.artGrazingBison;
            images[Textures.artGrizzlyBear] = Resources.artGrizzlyBear;
            images[Textures.artGrizzlyCub] = Resources.artGrizzlyCub;
            images[Textures.artGryphon] = Resources.artGryphon;
            images[Textures.artGryphonRider] = Resources.artGryphonRider;
            images[Textures.artHauntedChapel] = Resources.artHauntedChapel;
            images[Textures.artHeroicMight] = Resources.artHeroicMight;
            images[Textures.artHoundmaster] = Resources.artHoundmaster;
            images[Textures.artHourOfTheWolf] = Resources.artHourOfTheWolf;
            images[Textures.artHuntress] = Resources.artHuntress;
            images[Textures.artHypnotist] = Resources.artHypnotist;
            images[Textures.artIlasGambit] = Resources.artIlasGambit;
            images[Textures.artIlasGravekeeper] = Resources.artIlasGravekeeper;
            images[Textures.artIlatianFlutePlayer] = Resources.artIlatianFlutePlayer;
            images[Textures.artIlatianHaunter] = Resources.artIlatianHaunter;
            images[Textures.artIlatianWineMerchant] = Resources.artIlatianWineMerchant;
            images[Textures.artInfiltrator] = Resources.artInfiltrator;
            images[Textures.artInvigorate] = Resources.artInvigorate;
            images[Textures.artKappa] = Resources.artKappa;
            images[Textures.artKraken] = Resources.artKraken;
            images[Textures.artLoneRanger] = Resources.artLoneRanger;
            images[Textures.artLoneWolf] = Resources.artLoneWolf;
            images[Textures.artLordIla] = Resources.artLordIla;
            images[Textures.artLordPlevin] = Resources.artLordPlevin;
            images[Textures.artMagmaVents] = Resources.artMagmaVents;
            images[Textures.artMaleficentSpirit] = Resources.artMaleficentSpirit;
            images[Textures.artMarilith] = Resources.artMarilith;
            images[Textures.artMattysGambit] = Resources.artMattysGambit;
            images[Textures.artMeteorRain] = Resources.artMeteorRain;
            images[Textures.artMoratianBattleStandard] = Resources.artMoratianBattleStandard;
            images[Textures.artMorenianMedic] = Resources.artMorenianMedic;
            images[Textures.artMysteriousLilac] = Resources.artMysteriousLilac;
            images[Textures.artNothing] = Resources.artNothing;
            images[Textures.artOneWithNature] = Resources.artOneWithNature;
            images[Textures.artOvergrow] = Resources.artOvergrow;
            images[Textures.artPestilence] = Resources.artPestilence;
            images[Textures.artPropheticVision] = Resources.artPropheticVision;
            images[Textures.artPyrostorm] = Resources.artPyrostorm;
            images[Textures.artRaiseDead] = Resources.artRaiseDead;
            images[Textures.artRapture] = Resources.artRapture;
            images[Textures.artRavagingGreed] = Resources.artRavagingGreed;
            images[Textures.artRelentlessConscriptor] = Resources.artRelentlessConscriptor;
            images[Textures.artResoundingBlast] = Resources.artResoundingBlast;
            images[Textures.artRiderOfDeath] = Resources.artRiderOfDeath;
            images[Textures.artRockhandOgre] = Resources.artRockhandOgre;
            images[Textures.artRockToken] = Resources.artRockToken;
            images[Textures.artRottingZombie] = Resources.artRottingZombie;
            images[Textures.artSanguineArtisan] = Resources.artSanguineArtisan;
            images[Textures.artScrollOfEarth] = Resources.artScrollOfEarth;
            images[Textures.artSebasGambit] = Resources.artSebasGambit;
            images[Textures.artSeethingRage] = Resources.artSeethingRage;
            images[Textures.artSeraph] = Resources.artSeraph;
            images[Textures.artSereneDandelion] = Resources.artSereneDandelion;
            images[Textures.artShimmeringKoi] = Resources.artShimmeringKoi;
            images[Textures.artShottyContruct] = Resources.artShottyContruct;
            images[Textures.artSkeltal] = Resources.artSkeltal;
            images[Textures.artSolemnAberration] = Resources.artSolemnAberration;
            images[Textures.artSolemnLotus] = Resources.artSolemnLotus;
            images[Textures.artSpark] = Resources.artSpark;
            images[Textures.artSparryz] = Resources.artSparryz;
            images[Textures.artSpirit] = Resources.artSpirit;
            images[Textures.artSpiritOfSalvation] = Resources.artSpiritOfSalvation;
            images[Textures.artSquire] = Resources.artSquire;
            images[Textures.artStampedingDragon] = Resources.artStampedingDragon;
            images[Textures.artStarkLily] = Resources.artStarkLily;
            images[Textures.artSteamBolt] = Resources.artSteamBolt;
            images[Textures.artSurvivalInstincts] = Resources.artSurvivalInstincts;
            images[Textures.artSuspiciousVortex] = Resources.artSuspiciousVortex;
            images[Textures.artTaouyRuins] = Resources.artTaouyRuins;
            images[Textures.artTarantula] = Resources.artTarantula;
            images[Textures.artTeleport] = Resources.artTeleport;
            images[Textures.artTempleHealer] = Resources.artTempleHealer;
            images[Textures.artTerminate] = Resources.artTerminate;
            images[Textures.artUnmake] = Resources.artUnmake;
            images[Textures.artUnstableMemeExperiment] = Resources.artUnstableMemeExperiment;
            images[Textures.artUnyeildingStalwart] = Resources.artUnyeildingStalwart;
            images[Textures.artVibrantZinnia] = Resources.artVibrantZinnia;
            images[Textures.artWar] = Resources.artWar;
            images[Textures.artWaterBolt] = Resources.artWaterBolt;
            images[Textures.artWilt] = Resources.artWilt;
            images[Textures.artWolf] = Resources.artWolf;
            images[Textures.artWolf1] = Resources.artWolf1;
            images[Textures.artYungLich] = Resources.artYungLich;
            images[Textures.artZap] = Resources.artZap;
            images[Textures.artZombieToken] = Resources.artZombieToken;
            images[Textures.artIradj] = Resources.artIradj;
            images[Textures.artSeblastian] = Resources.artSeblastian;
            images[Textures.artArachosa] = Resources.artArachosa;
            images[Textures.artSpiderling] = Resources.artIradj;
            images[Textures.artCount_sFera_sII] = Resources.artCount_sFera_sII;
            images[Textures.artWarp] = Resources.artWarp;
            images[Textures.artHosro] = Resources.artHosro;
            images[Textures.artParalyzing_sSpider] = Resources.artParalyzing_sSpider;
            images[Textures.artWarp] = Resources.artWarp;
            images[Textures.artJabroni] = Resources.artJabroni;
            images[Textures.artMakaroni] = Resources.artMakaroni;
            images[Textures.artArchfather] = Resources.artArchfather;
            images[Textures.artHungry_sFelhound] = Resources.artHungry_sFelhound;
            images[Textures.artVincennes] = Resources.artVincennes;
            images[Textures.artIlatian_sGhoul] = Resources.artIlatian_sGhoul;                 
            sizes = new Dictionary<Textures, Size>();
            foreach (var i in images)
            {
                sizes[i.Key] = i.Value.Size;
            }
        }

        public static Textures orbTexture(ManaColour mc)
        {
            switch (mc)
            {
                case ManaColour.Chaos: return Textures.orbChaos;
                case ManaColour.Colourless: return Textures.orbColourless;
                case ManaColour.Death: return Textures.orbDeath;
                case ManaColour.Life: return Textures.orbLife;
                case ManaColour.Might: return Textures.orbMight;
                case ManaColour.Nature: return Textures.orbNature;
                case ManaColour.Order: return Textures.orbOrder;
                default:
                    throw new Exception();
            }
        }

        public static Textures colourlessTexture(int c)
        {
            switch (c)
            {
                case 0: return Textures.orbColourless0;
                case 1: return Textures.orbColourless1;
                case 2: return Textures.orbColourless2;
                case 3: return Textures.orbColourless3;
                case 4: return Textures.orbColourless4;
                case 5: return Textures.orbColourless5;
                case 6: return Textures.orbColourless6;
                case 7: return Textures.orbColourless7;
                case 8: return Textures.orbColourless8;
                case 9: return Textures.orbColourless9;
                case 10: return Textures.orbColourless10;
                case 11: return Textures.orbColourless11;
                case 12: return Textures.orbColourless12;
                default: throw new Exception();
            }
        }

        public static Textures packDisplayImage(Packs pack)
        {
            switch (pack)
            {
                case Packs.FirstEdition12Pack: return Textures.packFirstEdition12Pack;
                case Packs.FirstEdition40Pack: return Textures.packFirstEdition40Pack;
            }

            throw new Exception();
        }

        public static Textures setIcon(CardSet set)
        {
            switch (set)
            {
                case CardSet.FirstEdition: return Textures.setFirstedition;
            }

            throw new Exception();
        }

        public static Color rarityColor(Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Common: return Color.White;
                case Rarity.Uncommon: return Color.MediumPurple;
                case Rarity.Rare: return Color.Gold;
                case Rarity.Legendary: return Color.LightCoral;
                case Rarity.None: return Color.ForestGreen;
            }

            throw new Exception();
        }

        public static Textures cardArt(CardTemplate ct)
        {
            switch (ct)
            {
                case CardTemplate.Big_sMonkey: return Textures.artBigMonkey;
                case CardTemplate.Commander_sSparryz: return Textures.artSparryz;
                case CardTemplate.Flamekindler: return Textures.artFlamekindler;
                case CardTemplate.Moratian_sBattle_sStandard: return Textures.artMoratianBattleStandard;
                case CardTemplate.Seraph: return Textures.artSeraph;
                case CardTemplate.Chromatic_sUnicorn: return Textures.artChromaticUnicorn;
                case CardTemplate.Enraged_sDragon: return Textures.artEnragedDragon;
                case CardTemplate.Haunted_sChapel: return Textures.artHauntedChapel;
                case CardTemplate.Unyeilding_sStalwart: return Textures.artUnyeildingStalwart;
                case CardTemplate.Bubastis: return Textures.artBubastis;
                case CardTemplate.Morenian_sMedic: return Textures.artMorenianMedic;
                case CardTemplate.Famished_sTarantula: return Textures.artTarantula;
                case CardTemplate.Vibrant_sZinnia: return Textures.artVibrantZinnia;
                case CardTemplate.Primal_sChopter: return Textures.artAncientChopter;
                case CardTemplate.Stark_sLily: return Textures.artStarkLily;
                case CardTemplate.Serene_sDandelion: return Textures.artSereneDandelion;
                case CardTemplate.Daring_sPoppy: return Textures.artDaringPoppy;
                case CardTemplate.Mysterious_sLilac: return Textures.artMysteriousLilac;
                case CardTemplate.Solemn_sLotus: return Textures.artSolemnLotus;
                case CardTemplate.Resounding_sBlast: return Textures.artResoundingBlast;
                case CardTemplate.Feral_sImp: return Textures.artFeralImp;
                case CardTemplate.Shotty_sContruct: return Textures.artShottyContruct;
                case CardTemplate.Houndmaster: return Textures.artHoundmaster;
                case CardTemplate.Marilith: return Textures.artMarilith;
                case CardTemplate.Seething_sRage: return Textures.artSeethingRage;
                case CardTemplate.Ilas_sBargain: return Textures.artAberrantSacrifice;
                case CardTemplate.Rapture: return Textures.artRapture;
                case CardTemplate.Suspicious_sVortex: return Textures.artSuspiciousVortex;
                case CardTemplate.Ancient_sDruid: return Textures.artAncientDruid;
                case CardTemplate.Raise_sDead: return Textures.artRaiseDead;
                case CardTemplate.Deep_sFry: return Textures.artSpark;
                case CardTemplate.Abolish: return Textures.artAbolish;
                case CardTemplate.Chains_sof_sVirtue: return Textures.artChainsofVirtue;
                case CardTemplate.Chains_sof_sSin: return Textures.artChainsofSin;
                case CardTemplate.Rider_sof_sDeath: return Textures.artDeath;
                case CardTemplate.Rider_sof_sWar: return Textures.artWar;
                case CardTemplate.Rider_sof_sPestilence: return Textures.artPestilence;
                case CardTemplate.Rider_sof_sFamine: return Textures.artFamine;
                case CardTemplate.Magma_sVents: return Textures.artMagmaVents;
                case CardTemplate.Gotterdammerung: return Textures.artGottedammerung;
                case CardTemplate.Overgrow: return Textures.artOvergrow;
                case CardTemplate.Gleeful_sDuty: return Textures.artGleefulDuty;
                case CardTemplate.Counterspell: return Textures.artCounterspell;
                case CardTemplate.Invigorate: return Textures.artInvigorate;
                case CardTemplate.Ilatian_sHaunter: return Textures.artIlatianHaunter;
                case CardTemplate.Frenzied_sPirhana: return Textures.artFrenziedPiranha;
                case CardTemplate.Ilas_sGravekeeper: return Textures.artIlasGravekeeper;
                case CardTemplate.Kraken: return Textures.artKraken;
                case CardTemplate.Prince_sIla: return Textures.artLordIla;
                case CardTemplate.Wilt: return Textures.artWilt;
                case CardTemplate.Huntress_sOf_sNibemem: return Textures.artHuntress;
                case CardTemplate.Baby_sDragon: return Textures.artDragonHatchling;
                case CardTemplate.Fresh_sFox: return Textures.artFreshFox;
                case CardTemplate.Survival_sInstincts: return Textures.artSurvivalInstincts;
                case CardTemplate.Damage_sWard: return Textures.artDamageWard;
                case CardTemplate.One_sWith_sNature: return Textures.artOneWithNature;
                case CardTemplate.Graverobber_sSyrdin: return Textures.artGraverobberSyrdin;
                case CardTemplate.Sinister_sPact: return Textures.artSinisterPact;
                case CardTemplate.Goblin_sGrenade: return Textures.artGoblinGrenade;
                case CardTemplate.Cleansing_sFire: return Textures.artCleansingFire;
                case CardTemplate.Bhewas: return Textures.artBelwas;
                case CardTemplate.Zap: return Textures.artZap;
                case CardTemplate.Kappa: return Textures.artKappa;
                case CardTemplate.Cantrip: return Textures.artAlterFate;
                case CardTemplate.Temple_sHealer: return Textures.artTempleHealer;
                case CardTemplate.Yung_sLich: return Textures.artYungLich;
                case CardTemplate.Chieftain_sZ_aloot_aboks: return Textures.artChieftainZlootbox;
                case CardTemplate.Risen_sAbberation: return Textures.artSolemnAberration;
                case CardTemplate.Shibby_sShtank: return Textures.artEssenceOfClarity;
                case CardTemplate.Unmake: return Textures.artUnmake;
                case CardTemplate.Rockhand_sEchion: return Textures.artRockhandOgre;
                case CardTemplate.Primordial_sChimera: return Textures.artChimera;
                case CardTemplate.Illegal_sGoblin_sLaboratory: return Textures.artUnstableMemeExperiment;
                case CardTemplate.Teleport: return Textures.artTeleport;
                case CardTemplate.Squire: return Textures.artSquire;
                case CardTemplate.Spirit: return Textures.artSpirit;
                case CardTemplate.Gryphon: return Textures.artGryphon;
                case CardTemplate.Wolf: return Textures.artWolf;
                case CardTemplate.Zombie: return Textures.artZombieToken;
                case CardTemplate.Rock: return Textures.artRockToken;
                case CardTemplate.Call_sTo_sArms: return Textures.artCallToArms;
                case CardTemplate.Sanguine_sArtisan: return Textures.artSanguineArtisan;
                case CardTemplate.Shimmering_sKoi: return Textures.artShimmeringKoi;
                case CardTemplate.Decaying_sHorror: return Textures.artDecayingZombie;
                case CardTemplate.Relentless_sConsriptor: return Textures.artRelentlessConscriptor;
                case CardTemplate.Terminate: return Textures.artTerminate;
                case CardTemplate.Spirit_sof_sSalvation: return Textures.artSpiritOfSalvation;
                case CardTemplate.Benedictor: return Textures.artBenedictor;
                case CardTemplate.Pyrostorm: return Textures.artPyrostorm;
                case CardTemplate.Elven_sCultivator: return Textures.artElvenCultivator;
                case CardTemplate.Faceless_sSphinx: return Textures.artFacelessSphinx;
                case CardTemplate.Cerberus: return Textures.artCerebus;
                case CardTemplate.Heroic_sMight: return Textures.artHeroicMight;
                case CardTemplate.Taouy_sRuins: return Textures.artTaouyRuins;
                case CardTemplate.Shibby_as_sSaboteur: return Textures.artInfiltrator;
                case CardTemplate.Brute_sForce: return Textures.artEssenceOfRage;
                case CardTemplate.Scroll_sof_sEarth: return Textures.artScrollOfEarth;
                case CardTemplate.Maleficent_sSpirit: return Textures.artMaleficentSpirit;
                case CardTemplate.Thirstclaw: return Textures.artBloodclaw;
                case CardTemplate.Flameheart_sPhoenix: return Textures.artFireheartPheonix;
                case CardTemplate.Lord_sPlombie: return Textures.artLordPlevin;
                case CardTemplate.Gryphon_sRider: return Textures.artGryphonRider;
                case CardTemplate.Ravaging_sGreed: return Textures.artRavagingGreed;
                case CardTemplate.Water_sBolt: return Textures.artWaterBolt;
                case CardTemplate.Lone_sRanger: return Textures.artLoneRanger;
                case CardTemplate.Alter_sFate: return Textures.artAlterFate2;
                case CardTemplate.Great_sWhite_sBuffalo: return Textures.artGreatWhiteBuffalo;
                case CardTemplate.Count_sFera_sII: return Textures.artCount_sFera_sII;
                case CardTemplate.Arachosa: return Textures.artArachosa;
                case CardTemplate.Spiderling: return Textures.artSpiderling;
                case CardTemplate.Paralyzing_sSpider: return Textures.artParalyzing_sSpider;
                case CardTemplate.Seblastian: return Textures.artSeblastian;
                case CardTemplate.Warp: return Textures.artWarp;
                case CardTemplate.Hosro: return Textures.artHosro;
                case CardTemplate.Iradj: return Textures.artIradj;
                case CardTemplate.Jabroni: return Textures.artJabroni;
                case CardTemplate.Makaroni: return Textures.artMakaroni;
                case CardTemplate.Archfather: return Textures.artArchfather;
                case CardTemplate.Hungry_sFelhound: return Textures.artHungry_sFelhound;
                case CardTemplate.Vincennes: return Textures.artVincennes;
                case CardTemplate.Ilatian_sGhoul: return Textures.artIlatian_sGhoul;
                default: return Textures.iconCross;
            }
        }

        public static Size sizeOf(Textures t)
        {
            return sizes[t];
        }
    }
}
