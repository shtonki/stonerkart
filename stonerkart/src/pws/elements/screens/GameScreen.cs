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
        private int gameID { get; }

        public GamePromptPanel gamePromptPanel { get; }

        public PileView handView { get; }
        public PileView stackView { get; }

        public PlayerPanel heroPanel { get; }
        public PlayerPanel villainPanel { get; }

        public PileView heroGraveyard;
        public Winduh heroGraveyardWinduh;

        public PileView villainGraveyard;
        public Winduh villainGraveyardWinduh;


        public HexPanel hexPanel { get; }

        public AutoHidePileWinduh stackWinduh { get; }

        public TurnIndicator turnIndicator { get; }

        public ToggleButton passUntilEOT { get; }
        public ToggleButton passUntilEOTOnEmptyStack { get; }

        private const int panelMargin = 30;
        private const int panelWidthPreMargin = 500;

        private const int hexPanelDiameter = 7;

        private const int leftPanelWidth = panelWidthPreMargin - panelMargin;
        private const int leftPanelHeight = Frame.AVAILABLEHEIGHT - panelMargin * 2;
        private const int leftPanelX = panelMargin;
        private const int leftPanelY = panelMargin;

        private const int rightPanelWidth = panelWidthPreMargin - panelMargin - 100;
        private const int rightPanelHeight = Frame.AVAILABLEHEIGHT - panelMargin * 2;
        private const int rightPanelX = Frame.BACKSCREENWIDTH - rightPanelWidth - panelMargin;
        private const int rightPanelY = panelMargin;

        private const int handAndHexPaddingY = 25;
        private const int handXPadding = 20;
        private const int handViewHeight = 300;
        private const int handViewWidth = rightPanelX - (leftPanelX + leftPanelWidth) - handXPadding * 2;
        private const int handViewY = Frame.AVAILABLEHEIGHT - panelMargin - handViewHeight - handAndHexPaddingY;
        private const int handViewX = leftPanelWidth + leftPanelX + handXPadding;

        private const int hexPanelHeight = leftPanelHeight - handViewHeight - handAndHexPaddingY * 2;
        private const int hexPanelWidth = 700;
        private const int hexPanelXPaddingToRightPanel = 100;
        private const int hexPanelY = 25;

        private const int stackViewWidth = 200;
        private const int stackViewHeight = 400;

        private const int passUntilEOTY = 380;
        private const int passUntilHeight = 100;

        private const int turnIndicatorWidth = 175;
        private const int turnIndicatorHeight = 300;
        private const int turnIndicatorX = leftPanelX + leftPanelWidth + 30;
        private const int turnIndicatorY = 100;

        private const int playerPanelPadding = 25;
        private const int playerPanelHeight = (rightPanelHeight - playerPanelPadding * 3) / 2;
        private const int playerPanelWidth = rightPanelWidth - playerPanelPadding * 2;
        private const int heroPanelX = playerPanelPadding;
        private const int heroPanelY = rightPanelHeight - playerPanelHeight - playerPanelPadding;
        private const int villainPanelX = playerPanelPadding;
        private const int villainPanelY = playerPanelPadding;

        private const int toggledviewWidth = 200;
        private const int toggledHeight = 600;

        public GameScreen(GameSetupInfo gsi) : base(new Imege(Textures.table0))
        {
            gameID = gsi.gameid;

            var map = Map.MapFromConfiguration(gsi.GameRules.MapConfiguration);
            int hexColumns = map.width;
            int hexRows = map.height;
            int hexSize = (int)Math.Round((hexPanelHeight) / (hexRows + 0.5));

            Square leftPanel = new Square(leftPanelWidth, leftPanelHeight);
            leftPanel.X = 25;
            addElement(leftPanel);
            leftPanel.Backimege = new MemeImege(Textures.buttonbg0);
            leftPanel.Border = new SolidBorder(2, Color.Black);
            leftPanel.setLocation(leftPanelX, leftPanelY);

            gamePromptPanel = new GamePromptPanel(leftPanelWidth, 500);
            leftPanel.addChild(gamePromptPanel);


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

            hexPanel = new HexPanel(hexPanelWidth, hexPanelHeight);
            addElement(hexPanel);
            hexPanel.X = rightPanel.X - hexPanel.Width - hexPanelXPaddingToRightPanel;
            hexPanel.Y = hexPanelY;

            turnIndicator = new TurnIndicator(turnIndicatorWidth, turnIndicatorHeight);
            addElement(turnIndicator);
            turnIndicator.setLocation(turnIndicatorX, turnIndicatorY);

            makePanel(out heroGraveyard, out heroGraveyardWinduh);
            heroPanel.graveyardButton.clicked += a => heroGraveyardWinduh.Visible = !heroGraveyardWinduh.Visible;

            makePanel(out villainGraveyard, out villainGraveyardWinduh);
            villainPanel.graveyardButton.clicked += a => villainGraveyardWinduh.Visible = !villainGraveyardWinduh.Visible;


            stackView = new PileView();
            stackView.setSize(stackViewWidth, stackViewHeight);
            stackView.Columns = 1;
            stackView.Backimege = new MemeImege(Textures.buttonbg2);
            stackView.maxPadding = 40;

            stackWinduh = new AutoHidePileWinduh(stackView, "The Stack");
            addWinduh(stackWinduh);
            stackWinduh.Visible = false;

            passUntilEOT = new ToggleButton(turnIndicatorWidth, passUntilHeight);
            passUntilEOT.X = turnIndicatorX;
            passUntilEOT.Y = passUntilEOTY;
            addElement(passUntilEOT);
            passUntilEOT.TextLayout = new MultiLineFitLayout();
            passUntilEOT.Text = "Auto Pass Until End Of Turn";
            passUntilEOT.Backcolor = Color.FloralWhite;
            passUntilEOT.Border = new SolidBorder(2, Color.Black);
            passUntilEOT.Toggled = false;

            passUntilEOTOnEmptyStack = new ToggleButton(turnIndicatorWidth, passUntilHeight);
            passUntilEOTOnEmptyStack.X = turnIndicatorX;
            passUntilEOTOnEmptyStack.Y = passUntilEOTY + passUntilHeight;
            addElement(passUntilEOTOnEmptyStack);
            passUntilEOTOnEmptyStack.TextLayout = new MultiLineFitLayout();
            passUntilEOTOnEmptyStack.Text = "Auto Pass Empty Stack Until End Of Turn";
            passUntilEOTOnEmptyStack.Backcolor = Color.FloralWhite;
            passUntilEOTOnEmptyStack.Border = new SolidBorder(2, Color.Black);
            passUntilEOTOnEmptyStack.Toggled = false;
        }

        private void makePanel(out PileView pileview, out Winduh winduh)
        {
            pileview = new PileView();
            pileview.setSize(toggledviewWidth, toggledHeight);
            pileview.Columns = 1;
            pileview.Backimege = new MemeImege(Textures.buttonbg2);
            pileview.maxPadding = 30;
            winduh = new Winduh(pileview, true, true);
            addWinduh(winduh);
            winduh.Visible = false;
        }


        private List<Line> targetLines = new List<Line>();
        public void clearArrows()
        {
            foreach (var line in targetLines) removeElement(line);
            targetLines.Clear();
        }

        public void addArrow(CardView from, CardView to)
        {
            Line l = new Line(
                from.AbsoluteX + from.Width / 2,
                from.AbsoluteY + from.Height / 2,
                to.AbsoluteX + to.Width / 2,
                to.AbsoluteY + to.Height / 20,
                Color.Firebrick,
                6);
            targetLines.Add(l);
            addElement(l);
        }

        public void addArrow(CardView from, Tile to)
        {
            var p = hexPanel.hexCoords(to.x, to.y);
            Line l = new Line(
                from.AbsoluteX + from.Width/2, 
                from.AbsoluteY + from.Height/2, 
                p.X + hexPanel.AbsoluteX + hexPanel.hexsize/2, 
                p.Y + hexPanel.AbsoluteY + hexPanel.hexsize / 2,
                Color.Firebrick, 
                6);
            targetLines.Add(l);
            addElement(l);
        }

        protected override IEnumerable<MenuEntry> generateMenuEntries()
        {
            Console.WriteLine(gameID);
            return new MenuEntry[] { new ConcedeEntry(gameID), };
        }
    }

    class AutoHidePileWinduh : Winduh, Observer<PileChangedMessage>
    {
        private PileView pileView;

        public AutoHidePileWinduh(PileView content, string title = "") : base(content, title, false, true)
        {
            pileView = content;
        }

        public void notify(object o, PileChangedMessage t)
        {
            Pile p = (Pile)o;
            Visible = p.Count > 0;
            pileView.notify(o, t);
        }
    }
}
