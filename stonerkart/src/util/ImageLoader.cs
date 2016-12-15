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
            if (cache.ContainsKey(ct))
            {
                return cache[ct];
            }
            Image rt = artImageEx(ct);
            cache[ct] = rt;
            return rt;
        }

        public static Image artImageEx(CardTemplate ct)
        {
            switch (ct)
            {
                case CardTemplate.Belwas:
                {
                    return Resources.artBelwas;
                }

                case CardTemplate.Zap:
                {
                    return Resources.artZap;
                }

                default:
                {
                    return Resources.orbLife;
                }
            }
        }


        public static Image frameImage(ManaColour colour)
        {
            if (cache.ContainsKey(colour))
            {
                return cache[colour];
            }
            Image rt = frameImageEx(colour);
            cache[colour] = rt;
            return rt;
        }

        private static Image frameImageEx(ManaColour colour)
        {
            Image i;
            switch (colour)
            {
                case ManaColour.Death:
                {
                    i = Resources.frameDeath;
                } break;

                case ManaColour.Life:
                {
                    i = Resources.lifeFrame;
                } break;

                case ManaColour.Chaos:
                {
                    i = Resources.frameChaos;
                } break;

                case ManaColour.Nature:
                {
                    i = Resources.frameNature;
                } break;

                default:
                {
                    i = Resources.frameGrey;
                } break;
            }
            return i;
        }

        public static Image orbImage(ManaColour c)
        {
            Tuple<ManaColour> t = new Tuple<ManaColour>(c);
            if (cache.ContainsKey(t))
            {
                return cache[t];
            }
            Image rt = manaImageEx(c);
            cache[t] = rt;
            return rt;
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
                    return Properties.Resources.orbMulti;

                case ManaColour.Nature:
                    return Properties.Resources.orbNature;

                case ManaColour.Order:
                    return Properties.Resources.orbOrder;

                case ManaColour.Colourless:
                    return Properties.Resources.orbColourless;

                default:
                    throw new Exception();
            }
        }
    }
}
