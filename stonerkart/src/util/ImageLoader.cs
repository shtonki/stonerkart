using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using stonerkart.Properties;

namespace stonerkart
{
    static class ImageLoader
    {
        private static Dictionary<object, Image> cache = new Dictionary<object, Image>();

        public static Image artImage(CardTemplate ct)
        {
            return xd<CardTemplate>(ct, artImageEx);
        }

        public static Image frameImage(ManaColour colour)
        {
            return xd<ManaColour>(colour, frameImageEx);
        }

        public static Image orbImage(ManaColour c)
        {
            //wrapped in tuple to diferentiate between frameImage caching
            return xd<Tuple<ManaColour>>(new Tuple<ManaColour>(c), l => manaImageEx(l.Item1));
        }

        public static Image cardSetImage(CardSet cardSet, Rarity r)
        {
            return xd<Tuple<CardSet, Rarity>>(new Tuple<CardSet, Rarity>(cardSet, r), cardSetImageEx);
        }

        private static Image xd<T>(T o, Func<T, Image> f)
        {
            if (cache.ContainsKey(o))
            {
                return cache[o];
            }
            Image rt = f(o);
            cache[o] = rt;
            return rt;
        }

        private static Image cardSetImageEx(Tuple<CardSet, Rarity> tpl)
        {
            CardSet cs = tpl.Item1;
            Rarity r = tpl.Item2;

            Bitmap baseImage;
            Color swapColour;

            switch (cs)
            {
                case CardSet.FirstEdition:
                {
                    baseImage = Resources.setFirstedition;
                } break;

                default: throw new NotImplementedException();
            }

            switch (r)
            {
                case Rarity.Common:
                {
                    swapColour = Color.Black; 
                } break;

                case Rarity.Uncommon:
                {
                    swapColour = Color.DodgerBlue;
                } break;

                case Rarity.Rare:
                {
                    swapColour = Color.MediumVioletRed;
                } break;

                case Rarity.Legendary:
                {
                    swapColour = Color.DarkGoldenrod;
                } break;

                case Rarity.None:
                {
                    swapColour = Color.LimeGreen;
                } break;

                default: throw new NotImplementedException();
            }

            for (int x = 0; x < baseImage.Width; x++)
            {
                for (int y = 0; y < baseImage.Height; y++)
                {
                    Color gotColor = baseImage.GetPixel(x, y);
                    gotColor = Color.FromArgb(gotColor.A, swapColour.R, swapColour.G, swapColour.B);
                    baseImage.SetPixel(x, y, gotColor);
                }
            }

            return baseImage;
        }

        private static Image artImageEx(CardTemplate ct)
        {
            switch (ct)
            {
                case CardTemplate.Lone_sRanger:
                {
                    return Resources.artLoneRanger;
                }

                case CardTemplate.Water_sBolt:
                {
                    return Resources.artWaterBolt;
                }

                case CardTemplate.Ravaging_sGreed:
                {
                    return Resources.artRavagingGreed;
                }

                case CardTemplate.Gryphon_sRider:
                {
                    return Resources.artGryphonRider;
                }

                case CardTemplate.Lord_sPlevin:
                {
                    return Resources.artLordPlevin;
                }

                case CardTemplate.Flameheart_sPhoenix:
                {
                    return Resources.artFireheartPheonix;
                }

                case CardTemplate.Thirstclaw:
                {
                    return Resources.artBloodclaw;
                }

                case CardTemplate.Maleficent_sSpirit:
                {
                    return Resources.artMaleficentSpirit;
                }

                case CardTemplate.Scroll_sof_sEarth:
                {
                    return Resources.artScrollOfEarth;
                }

                case CardTemplate.Rock:
                {
                    return Resources.artRockToken;
                }

                case CardTemplate.Brute_sForce:
                {
                    return Resources.artEssenceOfRage;
                }

                case CardTemplate.Shibby_as_sSaboteur:
                {
                    return Resources.artInfiltrator;
                }

                case CardTemplate.Taouy_sRuins:
                {
                    return Resources.artTaouyRuins;
                }

                case CardTemplate.Heroic_sMight:
                {
                    return Resources.artHeroicMight;
                }

                case CardTemplate.Cerberus:
                {
                    return Resources.artCerebus;
                }

                case CardTemplate.Faceless_sSphinx:
                {
                    return Resources.artFacelessSphinx;
                }

                case CardTemplate.Elven_sCultivator:
                {
                    return Resources.artElvenCultivator;
                }

                case CardTemplate.Pyrostorm:
                {
                    return Resources.artPyrostorm;
                }

                case CardTemplate.Benedictor:
                {
                    return Resources.artBenedictor;
                }

                case CardTemplate.Spirit_sof_sSalvation:
                {
                    return Resources.artSpiritOfSalvation;
                }

                case CardTemplate.Terminate:
                {
                    return Resources.artTerminate;
                }

                case CardTemplate.Relentless_sConsriptor:
                {
                    return Resources.artRelentlessConscriptor;
                }

                case CardTemplate.Decaying_sHorror:
                {
                    return Resources.artRottingZombie;
                }

                case CardTemplate.Zombie:
                {
                    return Resources.artDecayingZombie;
                }

                case CardTemplate.Shimmering_sKoi:
                {
                    return Resources.artShimmeringKoi;
                }

                case CardTemplate.Commander_sSparryz:
                {
                    return Resources.artSparryz;
                }

                case CardTemplate.Flamekindler:
                {
                    return Resources.artFlamekindler;
                }

                case CardTemplate.Moratian_sBattle_sStandard:
                {
                    return Resources.artMoratianBattleStandard;
                }

                case CardTemplate.Seraph:
                {
                    return Resources.artSeraph;
                }

                case CardTemplate.Chromatic_sUnicorn:
                {
                    return Resources.artChromaticUnicorn;
                }

                case CardTemplate.Enraged_sDragon:
                {
                    return Resources.artStampedingDragon;
                }

                case CardTemplate.Haunted_sChapel:
                {
                    return Resources.artHauntedChapel;
                }

                case CardTemplate.Unyeilding_sStalwart:
                {
                    return Resources.artUnyeildingStalwart;
                }

                case CardTemplate.Spirit:
                {
                    return Resources.artSpirit;
                }

                case CardTemplate.Bubastis:
                {
                    return Resources.artBubastis;
                }

                case CardTemplate.Morenian_sMedic:
                {
                    return Resources.artMorenianMedic;
                }

                case CardTemplate.Famished_sTarantula:
                {
                    return Resources.artTarantula;
                }

                case CardTemplate.Vibrant_sZinnia:
                {
                    return Resources.artVibrantZinnia;
                }

                case CardTemplate.Primal_sChopter:
                {
                    return Resources.artAncientChopter;
                }

                case CardTemplate.Stark_sLily:
                {
                    return Resources.artStarkLily;
                }
                
                case CardTemplate.Gryphon:
                {
                    return Resources.artGryphon;
                }

                case CardTemplate.Serene_sDandelion:
                {
                    return Resources.artSereneDandelion;
                }

                case CardTemplate.Mysterious_sLilac:
                {
                    return Resources.artMysteriousLilac;
                }

                case CardTemplate.Daring_sPoppy:
                {
                    return Resources.artDaringPoppy;
                }

                case CardTemplate.Solemn_sLotus:
                {
                    return Resources.artSolemnLotus;
                }

                case CardTemplate.Resounding_sBlast:
                {
                    return Resources.artResoundingBlast;
                }

                case CardTemplate.Feral_sImp:
                {
                    return Resources.artFeralImp;
                }

                case CardTemplate.Shotty_sContruct:
                {
                    return Resources.artShottyContruct;
                }

                case CardTemplate.Sanguine_sArtisan:
                {
                    return Resources.artSanguineArtisan;
                }

                case CardTemplate.Houndmaster:
                {
                    return Resources.artHoundmaster;
                }

                case CardTemplate.Wolf:
                {
                    return Resources.artWolf1;
                }

                case CardTemplate.Marilith:
                {
                    return Resources.artMarilith;
                }

                case CardTemplate.Seething_sRage:
                {
                    return Resources.artEnragedDragon;
                }

                case CardTemplate.Rapture:
                {
                    return Resources.artRapture;
                }

                case CardTemplate.Ilas_sBargain:
                {
                    return Resources.artAberrantSacrifice;
                }

                case CardTemplate.Suspicious_sVortex:
                {
                    return Resources.artSuspiciousVortex;
                }

                case CardTemplate.Ancient_sDruid:
                {
                    return Resources.artAncientDruid;
                }

                case CardTemplate.Raise_sDead:
                {
                    return Resources.artRaiseDead;
                }

                case CardTemplate.Deep_sFry:
                {
                    return Resources.artSpark;
                }

                case CardTemplate.Abolish:
                {
                    return Resources.artAbolish;
                }

                case CardTemplate.Rider_sof_sWar:
                {
                        return Resources.artWar;
                }

                case CardTemplate.Rider_sof_sPestilence:
                {
                    return Resources.artPestilence;
                }

                case CardTemplate.Rider_sof_sFamine:
                {
                    return Resources.artFamine;
                }

                case CardTemplate.Rider_sof_sDeath:
                {
                    return Resources.artDeath;
                }

                case CardTemplate.Magma_sVents:
                {
                    return Resources.artMagmaVents;
                }

                case CardTemplate.Chains_sof_sVirtue:
                {
                    return Resources.artChainsofVirtue;
                }

                case CardTemplate.Chains_sof_sSin:
                {
                    return Resources.artChainsofSin;
                }

                case CardTemplate.Gotterdammerung:
                {
                    return Resources.artGottedammerung;
                }

                case CardTemplate.Overgrow:
                {
                    return Resources.artOvergrow;
                }

                case CardTemplate.Gleeful_sDuty:
                {
                    return Resources.artGleefulDuty;
                }

                case CardTemplate.Counterspell:
                {
                    return Resources.artCounterspell;
                }

                case CardTemplate.Invigorate:
                {
                    return Resources.artInvigorate;
                }

                case CardTemplate.Ilatian_sHaunter:
                {
                    return Resources.artIlatianHaunter;
                }

                case CardTemplate.Frenzied_sPirhana:
                {
                    return Resources.artFrenziedPiranha;
                }

                case CardTemplate.Ilas_sGravekeeper:
                {
                    return Resources.artIlasGravekeeper;
                }

                case CardTemplate.Kraken:
                {
                    return Resources.artKraken;
                }

                case CardTemplate.Prince_sIla:
                {
                    return Resources.artLordIla;
                }

                case CardTemplate.Wilt:
                {
                    return Resources.artWilt;
                }

                case CardTemplate.Call_sTo_sArms:
                {
                    return Resources.artCallToArmsAlternate;
                }

                case CardTemplate.Huntress_sOf_sNibemem:
                {
                    return Resources.artHuntress;
                }

                case CardTemplate.Squire:
                {
                    return Resources.artSquire;
                }

                case CardTemplate.Baby_sDragon:
                {
                    return Resources.artDragonHatchling;
                }

                case CardTemplate.Fresh_sFox:
                {
                    return Resources.artFreshFox;
                }

                case CardTemplate.Damage_sWard:
                {
                    return Resources.artDamageWard;
                }

                case CardTemplate.Teleport:
                {
                    return Resources.artTeleport;
                }

                case CardTemplate.One_sWith_sNature:
                {
                    return Resources.artOneWithNature;
                }

                case CardTemplate.Survival_sInstincts:
                {
                    return Resources.artSurvivalInstincts;
                }

                case CardTemplate.Graverobber_sSyrdin:
                {
                    return Resources.artGraverobberSyrdin;
                }

                case CardTemplate.Alter_sFate:
                {
                    return Resources.artAlterTime;
                }

                case CardTemplate.Goblin_sGrenade:
                {
                    return Resources.artGoblinGrenade;
                }

                case CardTemplate.Illegal_sGoblin_sLaboratory:
                {
                    return Resources.artUnstableMemeExperiment;
                }

                case CardTemplate.Primordial_sChimera:
                {
                    return Resources.artChimera;
                }

                case CardTemplate.Cleansing_sFire:
                {
                    return Resources.artCleansingFire;
                }

                case CardTemplate.Rockhand_sEchion:
                {
                    return Resources.artRockhandOgre;
                }

                case CardTemplate.Unmake:
                {
                    return Resources.artUnmake;
                }

                case CardTemplate.Shibby_sShtank:
                {
                    return Resources.artEssenceOfClarity;
                }

                case CardTemplate.Risen_sAbberation:
                {
                    return Resources.artSolemnAberration;
                }

                case CardTemplate.Chieftain_sZ_aloot_aboks:
                {
                    return Resources.artChieftainZlootbox;
                }

                case CardTemplate.Yung_sLich:
                {
                    return Resources.artYungLich;
                }

                case CardTemplate.Bhewas:
                {
                    return Resources.artBelwas;
                }

                case CardTemplate.Zap:
                {
                    return Resources.artZap;
                }

                case CardTemplate.Kappa:
                {
                    return Resources.artKappa;
                }

                case CardTemplate.Cantrip:
                {
                    return Resources.artAlterFate;
                }

                case CardTemplate.Temple_sHealer:
                {
                    return Resources.artTempleHealer;
                }

                default:
                {
                    return Resources.artNothing;
                }
            }
        }

        private static Image frameImageEx(ManaColour colour)
        {
            switch (colour)
            {
                case ManaColour.Multi:
                {
                    return Resources.frameMulti;
                }

                case ManaColour.Death:
                {
                    return Resources.frameDeath;
                } 

                case ManaColour.Life:
                {
                    return Resources.lifeFrame;
                } 

                case ManaColour.Chaos:
                {
                    return Resources.frameChaos;
                } 

                case ManaColour.Nature:
                {
                    return Resources.frameNature;
                } 

                case ManaColour.Might:
                {
                    return Resources.frameMight;
                } 
                        
                case ManaColour.Order:
                {
                    return Resources.frameOrder;
                } 

                case ManaColour.Colourless:
                {
                    return Resources.frameGrey;
                } 
            }
            throw new Exception();
        }

        private static Image manaImageEx(ManaColour c)
        {
            switch (c)
            {
                case ManaColour.Chaos:
                    return Properties.Resources.orbChaos;

                case ManaColour.Death:
                    return Properties.Resources.orbDeath;

                case ManaColour.Life:
                    return Properties.Resources.orbLife;

                case ManaColour.Might:
                    return Properties.Resources.orbMight;

                case ManaColour.Nature:
                    return Properties.Resources.orbNature;

                case ManaColour.Order:
                    return Properties.Resources.orbOrder;

                case ManaColour.Colourless:
                    return Properties.Resources.orbColourless;

                case ManaColour.Multi:
                    return Resources.orbMulti;

                default:
                    throw new Exception();
            }
        }
    }
}
