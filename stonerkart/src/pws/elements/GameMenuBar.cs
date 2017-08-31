using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class GameMenuBar : Square
    {
        public Button showFriendsButton { get; }
        private PlayerFlarePanel playerFlarePanel;

        #region layout

        private const int playerFlareWidth = 200;
        private const int playerFlareX = 20;

        private const int friendsButtonX = 250;

        #endregion

        public GameMenuBar() : base(Frame.BACKSCREENWIDTH, Frame.MENUHEIGHT)
        {
            Backimege = new MemeImege(Textures.buttonbg2, 1);
            Y = Frame.BACKSCREENHEIGHT - Frame.MENUHEIGHT;

            

            int friendsButtonHeight = Height;
            showFriendsButton = new Button(friendsButtonHeight, friendsButtonHeight);
            showFriendsButton.Backimege = new Imege(Textures.buttonFriends);
            showFriendsButton.X = friendsButtonX;
            addChild(showFriendsButton);
        }

        public void setFlare(User user)
        {
            if (playerFlarePanel != null) throw new Exception();
            playerFlarePanel = new PlayerFlarePanel(user.name, user.icon, playerFlareWidth, Height);
            addChild(playerFlarePanel);
            playerFlarePanel.X = playerFlareX;
            playerFlarePanel.Backcolor = Color.FromArgb(150, 150, 150, 150);
        }
    }
}
