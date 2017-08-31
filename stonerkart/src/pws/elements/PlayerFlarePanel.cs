using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class PlayerFlarePanel : Square
    {
        private Square flare; //rhyme = accidental
        private Square name;


        public PlayerFlarePanel(string playername, CardTemplate icon, int width, int height) : base(width, height)
        {
            if (width < height) throw new Exception();

            flare = new Square(height, height);
            addChild(flare);
            flare.Backimege = new Imege(TextureLoader.cardArt(icon));
            flare.Border = new AnimatedBorder(Textures.border0, 4);

            int namepadding = height/5;
            name = new Square(width - height - namepadding, height);
            addChild(name);
            name.Text = playername;
            name.X = flare.Width + namepadding;
        }
    }
}
