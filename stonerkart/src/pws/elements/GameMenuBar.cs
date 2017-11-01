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
        public Button addFriendsButton { get; }
        public PlayerFlarePanel playerFlarePanel { get; }
        public PlayerShekelPanel shekelsCount { get; }
        public Button menuButton { get; }
        public Button backButton { get; }

        #region layout

        private const int playerFlareWidth = 200;
        private const int playerFlareX = 20;

        private const int friendsButtonX = 250;
        private const int addFriendsButtonX = 300;

        private const int shekelSquareWidth = 200;
        private const int shekelSquareX = 420;

        private const int menuButtonX = Frame.BACKSCREENWIDTH - 100;
        private const int backButtonX = menuButtonX - buttonWidth - 10;
        private const int buttonWidth = 90;

        #endregion

        public GameMenuBar() : base(Frame.BACKSCREENWIDTH, Frame.MENUHEIGHT)
        {
            Backimege = new MemeImege(Textures.buttonbg2, 1);
            Y = Frame.BACKSCREENHEIGHT - Frame.MENUHEIGHT;

            playerFlarePanel = new PlayerFlarePanel(playerFlareWidth, Height);
            playerFlarePanel.X = playerFlareX;
            addChild(playerFlarePanel);

            menuButton = new Button(buttonWidth, Height);
            addChild(menuButton);
            menuButton.X = menuButtonX;
            menuButton.Backcolor = Color.PapayaWhip;
            menuButton.Border = new SolidBorder(2, Color.Black);
            menuButton.Text = "Menu";

            backButton = new Button(buttonWidth, Height);
            addChild(backButton);
            backButton.X = backButtonX;
            backButton.Backcolor = Color.PapayaWhip;
            backButton.Border = new SolidBorder(2, Color.Black);
            backButton.Text = "Back";

            shekelsCount = new PlayerShekelPanel(shekelSquareWidth, Height - 10);
            addChild(shekelsCount);
            shekelsCount.Y = 5;
            shekelsCount.X = shekelSquareX;

            int friendsButtonHeight = Height;
            showFriendsButton = new Button(friendsButtonHeight, friendsButtonHeight);
            showFriendsButton.Backimege = new Imege(Textures.iconFriends);
            showFriendsButton.X = friendsButtonX;
            addChild(showFriendsButton);

            addFriendsButton = new Button(friendsButtonHeight, friendsButtonHeight);
            addFriendsButton.Backimege = new Imege(Textures.iconAddFriends);
            addFriendsButton.X = addFriendsButtonX;
            addChild(addFriendsButton);
        }
    }

    class PlayerShekelPanel : ShekelPanel, Observer<UserChanged>
    {
        public PlayerShekelPanel(int width, int height) : base(width, height)
        {
        }

        public void notify(object o, UserChanged t)
        {
            int shekels = ((User)o).Shekels;
            setPrice(shekels);
        }
    }
}
