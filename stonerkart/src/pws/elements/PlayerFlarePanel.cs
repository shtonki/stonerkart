using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class PlayerFlarePanel : Square, Observer<UserChanged>
    {
        private Square flare; //rhyme = accidental
        private Square name;

        public PlayerFlarePanel(int width, int height) : base(width, height)
        {
        }

        private void updatestuff(User user)
        {
            if (Width < Height) throw new Exception();
            var icon = user.Icon;
            var playername = user.Name;

            //Backcolor = Color.FromArgb(150, 150, 150, 150);

            flare = new Square(height, height);
            addChild(flare);
            flare.Backimege = new Imege(TextureLoader.cardArt(icon));
            flare.Border = new AnimatedBorder(Textures.border0, 4);

            int namepadding = height / 5;
            name = new Square(width - height - namepadding, height);
            addChild(name);
            name.Text = playername;
            name.X = flare.Width + namepadding;
        }

        public void notify(object o, UserChanged t)
        {
            updatestuff((User)o);
        }
    }
}
