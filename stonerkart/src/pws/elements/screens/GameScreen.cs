using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class GameScreen : Screen
    {
        public PromptPanel promptPanel { get; }

        public PileView handView { get; }
        public PileView stackView { get; }

        public PlayerPanel heroPanel { get; }
        public PlayerPanel villainPanel { get; }

        public HexPanel hexPanel { get; }

        public TurnIndicator turnIndicator { get; }

        private const int panelMargin = 30;
        private const int panelWidthPreMargin = 500;


        private const int leftPanelWidth = panelWidthPreMargin - panelMargin;
        private const int leftPanelHeight = Frame.AVAILABLEHEIGHT - panelMargin*2;
        private const int leftPanelX = panelMargin;
        private const int leftPanelY = panelMargin;

        private const int rightPanelWidth = panelWidthPreMargin - panelMargin - 100;
        private const int rightPanelHeight = Frame.AVAILABLEHEIGHT - panelMargin * 2;
        private const int rightPanelX = Frame.BACKSCREENWIDTH - rightPanelWidth - panelMargin;
        private const int rightPanelY = panelMargin;

        private const int handAndHexPaddingY = 25;
        private const int handXPadding = 20;
        private const int handViewHeight = 300;
        private const int handViewWidth = rightPanelX - (leftPanelX + leftPanelWidth) - handXPadding*2;
        private const int handViewY = Frame.AVAILABLEHEIGHT - panelMargin - handViewHeight - handAndHexPaddingY;
        private const int handViewX = leftPanelWidth + leftPanelX + handXPadding;

        private const int hexPanelHeight = leftPanelHeight - handViewHeight - handAndHexPaddingY*2;
        private const int hexPanelXPaddingToRightPanel = 100;
        private const int hexPanelY = 25;
        private const int hexRows = 7;
        private const int hexColumns = 11;
        private int hexSize = (int)Math.Round((hexPanelHeight)/(hexRows + 0.5));

        private const int stackViewWidth = 300;
        private const int stackViewHeight = 700;
        private const int stackViewY = 30;

        private const int turnIndicatorWidth = 175;
        private const int turnIndicatorHeight = 300;
        private const int turnIndicatorX = leftPanelX + leftPanelWidth + 30;
        private const int turnIndicatorY = 200;

        private const int playerPanelPadding = 25;
        private const int playerPanelHeight = (rightPanelHeight - playerPanelPadding*3)/2;
        private const int playerPanelWidth = rightPanelWidth - playerPanelPadding*2;
        private const int heroPanelX = playerPanelPadding;
        private const int heroPanelY = rightPanelHeight - playerPanelHeight - playerPanelPadding;
        private const int villainPanelX = playerPanelPadding;
        private const int villainPanelY = playerPanelPadding;


        public GameScreen() : base(new Imege(Textures.table0))
        {
            Square leftPanel = new Square(leftPanelWidth, leftPanelHeight);
            leftPanel.X = 25;
            addElement(leftPanel);
            leftPanel.Backimege = new MemeImege(Textures.buttonbg0);
            leftPanel.Border = new SolidBorder(2, Color.Black);
            leftPanel.setLocation(leftPanelX, leftPanelY);

            promptPanel = new PromptPanel(leftPanelWidth, 500);
            leftPanel.addChild(promptPanel);


            handView = new PileView();
            addElement(handView);
            handView.Backimege = new MemeImege(Textures.buttonbg2);
            handView.setSize(handViewWidth, handViewHeight);
            handView.X = handViewX;
            handView.Y = handViewY;


            Square rightPanel = new Square(rightPanelWidth, rightPanelHeight);
            addElement(rightPanel);
            rightPanel.Backimege = new MemeImege(Textures.buttonbg0);
            rightPanel.setLocation(rightPanelX, rightPanelY);
            rightPanel.Border = new SolidBorder(2, Color.Black);

            heroPanel = new PlayerPanel(playerPanelWidth, playerPanelHeight);
            rightPanel.addChild(heroPanel);
            heroPanel.setLocation(heroPanelX, heroPanelY);

            villainPanel = new PlayerPanel(playerPanelWidth, playerPanelHeight);
            rightPanel.addChild(villainPanel);
            villainPanel.setLocation(villainPanelX, villainPanelY);

            stackView = new PileView();
            //rightPanel.addChild(stackView);
            stackView.setSize(stackViewWidth, stackViewHeight);
            stackView.Columns = 1;
            stackView.Backimege = new MemeImege(Textures.buttonbg2);
            stackView.moveTo(MoveTo.Center, stackViewY);

            hexPanel = new HexPanel(hexColumns, hexRows, hexSize - 2);
            addElement(hexPanel);
            hexPanel.X = rightPanel.X - hexPanel.Width - hexPanelXPaddingToRightPanel;
            hexPanel.Y = hexPanelY;

            turnIndicator = new TurnIndicator(turnIndicatorWidth, turnIndicatorHeight);
            addElement(turnIndicator);
            turnIndicator.setLocation(turnIndicatorX, turnIndicatorY);
        }
    }
}
