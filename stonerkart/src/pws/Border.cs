using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    abstract class Border
    {
        public int thickness { get; set; }

        public Border(int thickness)
        {
            this.thickness = thickness;
        }

        public abstract void draw(DrawerMaym dm, int x, int y, int width, int height);
    }

    class SolidBorder : Border
    {
        public Color borderColor { get; private set; }

        public SolidBorder(int thickness, Color borderColor) : base(thickness)
        {
            this.borderColor = borderColor;
        }

        public override void draw(DrawerMaym dm, int x, int y, int width, int height)
        {
            dm.fillRectange(borderColor, 0, 0, width, 4); //top
            dm.fillRectange(borderColor, 0, 0, 4, height); //left
            dm.fillRectange(borderColor, 0, 0 + height - 4, width, 4); //bottom
            dm.fillRectange(borderColor, 0 + width - 4, 0, 4, height); //right
        }

    }

    class AnimatedBorder : Border
    {
        public Textures texture { get; set; }

        private Box cropbox;


        private double offset;
        public double animationspeed { get; set; }

        private double ypos;

        private double cropsize;
        private double cropskip;
        private double cropthickness;

        private static Random rand = new Random();


        public AnimatedBorder(Textures texture, int thickness, double animationspeed = 0.0005, double cropsize = 0.2) : base(thickness)
        {
            this.texture = texture;
            this.animationspeed = animationspeed;
            offset = rand.NextDouble() * (1 - cropsize);
            ypos = rand.NextDouble()*(1-cropsize*2)+cropsize;

            cropsize = 0.2;
            cropthickness = cropsize/10;
            cropskip = cropsize - cropthickness;
        }

        public override void draw(DrawerMaym dm, int x, int y, int width, int height)
        {
            //if (ypos > 1 - cropsize) ypos = 0;


            if (ypos + cropsize + cropskip >= 1)
            {
                animationspeed = -animationspeed;
            }
            else if (ypos - cropsize - cropskip <= 0)
            {
                animationspeed = -animationspeed;
            }
            ypos += animationspeed;

            dm.drawTexture(texture, 0, 0, width, thickness, 
                new Box(offset, ypos, cropskip, cropthickness)); //top

            dm.drawTexture(texture, 0, 0, thickness, height,
                new Box(offset, ypos, cropthickness, cropskip)); //left

            dm.drawTexture(texture, 0, height - thickness, width, thickness,
                new Box(offset, ypos + cropthickness, cropskip, -cropthickness)); //bottom

            dm.drawTexture(texture, width - thickness, 0, thickness, height,
                new Box(offset + cropskip - cropthickness, ypos, cropthickness, cropskip)); //right
        }

        /*
        private double helpMe(double initialValue)
        {
            var moveDistance = (rand.NextDouble() - 0.5)*animationspeed;
            var result = initialValue + moveDistance;

            if (result < 0 || result > 1 - cropsize)
            {
                return initialValue;
            }
            return result;
        }
        */
    }
}
