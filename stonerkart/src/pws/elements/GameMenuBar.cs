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
        private PricePanel shekelsCount { get; }

        #region layout

        private const int playerFlareWidth = 200;
        private const int playerFlareX = 20;

        private const int friendsButtonX = 250;

        private const int shekelSquareWidth = 200;
        private const int shekelSquareX = Frame.BACKSCREENWIDTH - shekelSquareWidth - 50;

        #endregion

        public GameMenuBar() : base(Frame.BACKSCREENWIDTH, Frame.MENUHEIGHT)
        {
            Backimege = new MemeImege(Textures.buttonbg2, 1);
            Y = Frame.BACKSCREENHEIGHT - Frame.MENUHEIGHT;

            shekelsCount = new PricePanel(shekelSquareWidth, Height - 10);
            addChild(shekelsCount);
            shekelsCount.Y = 5;
            shekelsCount.X = shekelSquareX;

            int friendsButtonHeight = Height;
            showFriendsButton = new Button(friendsButtonHeight, friendsButtonHeight);
            showFriendsButton.Backimege = new Imege(Textures.iconFriends);
            showFriendsButton.X = friendsButtonX;
            addChild(showFriendsButton);
        }

        public void setFlare(User user)
        {
            if (playerFlarePanel != null) throw new Exception();
            playerFlarePanel = new PlayerFlarePanel(user.Name, user.Icon, playerFlareWidth, Height);
            playerFlarePanel.Backcolor = Color.Silver;
            addChild(playerFlarePanel);
            playerFlarePanel.X = playerFlareX;
        }

        public void setShekelCount(int shekels)
        {
            shekelsCount.setPrice(shekels);
        }
    }
}
