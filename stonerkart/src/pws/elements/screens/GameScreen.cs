using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class GameScreen : Screen
    {
        public PromptPanel promptPanel { get; }

        public PileView handView { get; }

        public PlayerPanel heroPanel { get; }

        public HexPanel hexPanel { get; }

        private const int leftPanelWidth = 500;
        private const int leftPanelHeight = Frame.AVAILABLEHEIGHT;

        private const int handViewHeight = 300;
        private const int handViewWidth = 800;

        private const int hexRows = 7;
        private const int hexColumns = 11;
        private int hexSize = (int)Math.Round((leftPanelHeight - handViewHeight)/(hexRows + 0.5));


        public GameScreen() : base(new Imege(Textures.table0))
        {
            Square leftPanel = new Square(leftPanelWidth, leftPanelHeight);
            addElement(leftPanel);
            leftPanel.Backimege = new MemeImege(Textures.buttonbg0);

            promptPanel = new PromptPanel(leftPanelWidth, 500);
            leftPanel.addChild(promptPanel);

            heroPanel = new PlayerPanel(leftPanelWidth, 500);
            leftPanel.addChild(heroPanel);
            heroPanel.moveTo(MoveTo.Nowhere, MoveTo.Bottom);

            handView = new PileView();
            addElement(handView);
            handView.setSize(handViewWidth, handViewHeight);
            handView.X = leftPanelWidth;
            handView.Y = Frame.AVAILABLEHEIGHT - handViewHeight;

            hexPanel = new HexPanel(11, 7, hexSize);
            addElement(hexPanel);
            hexPanel.X = leftPanelWidth;
        }
    }
}
