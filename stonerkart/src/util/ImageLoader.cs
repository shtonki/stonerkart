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

                case CardTemplate.Huntress_sOf_sNibememe:
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

                case CardTemplate.Belwas:
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
