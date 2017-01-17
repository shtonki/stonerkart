using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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
                case CardTemplate.Alter_Fate:
                {
                    return Resources.artAlterTime;
                }

                case CardTemplate.Goblin_Grenade:
                {
                    return Resources.artGoblinGrenade;
                }

                case CardTemplate.Illegal_Goblin_Laboratory:
                {
                    return Resources.artUnstableMemeExperiment;
                }

                case CardTemplate.Bear_Cavalary:
                {
                    return Resources.artBearCavalary;
                }

                case CardTemplate.Cleansing_Fire:
                {
                    return Resources.artCleansingFire;
                }

                case CardTemplate.Rockhand_Ogre:
                {
                    return Resources.artRockhandOgre;
                }

                case CardTemplate.Unmake:
                {
                    return Resources.artUnmake;
                }

                case CardTemplate.Shibby_Shtank:
                {
                    return Resources.artEssenceOfClarity;
                }

                case CardTemplate.Risen_Abberation:
                {
                    return Resources.artSolemnAberration;
                }

                case CardTemplate.Nature_Heroman:
                {
                    return Resources.artLoneRanger;
                }

                case CardTemplate.Yung_Lich:
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

                case CardTemplate.Temple_Healer:
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
