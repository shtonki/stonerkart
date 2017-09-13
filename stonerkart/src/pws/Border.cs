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
            dm.fillRectange(borderColor, 0, 0, width, thickness); //top
            dm.fillRectange(borderColor, 0, 0, thickness, height); //left
            dm.fillRectange(borderColor, 0, 0 + height - thickness, width, thickness); //bottom
            dm.fillRectange(borderColor, 0 + width - thickness, 0, thickness, height); //right
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

            Imege top =    new Imege(texture, new Box(offset, ypos, cropskip, cropthickness));
            Imege left =   new Imege(texture, new Box(offset, ypos, cropthickness, -cropskip));
            Imege bottom = new Imege(texture, new Box(offset, ypos + cropthickness, cropskip, -cropthickness));
            Imege right =  new Imege(texture, new Box(offset + cropskip - cropthickness, ypos, cropthickness, cropskip));

            dm.drawImege(top, 0, 0, width, thickness); 
            dm.drawImege(left, 0, 0, thickness, height); 
            dm.drawImege(bottom, 0, height - thickness, width, thickness); 
            dm.drawImege(right, width - thickness, 0, thickness, height); 
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
