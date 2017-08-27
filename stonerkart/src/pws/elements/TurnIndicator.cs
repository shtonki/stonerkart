using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class TurnIndicator : Square
    {
        private Square[] squares;
        private Square yourTurn;
        private Square opponentsTurn;


        public TurnIndicator(int width, int height) : base(width, height)
        {
            var steps = Enum.GetValues(typeof (Steps)).Cast<Steps>().ToArray();
            squares = new Square[steps.Length];
            int squareHeight = height/(squares.Length+2);

            for (int i = 0; i < squares.Length; i++)
            {
                var s = squares[i] = new Square(width, squareHeight);
                s.Text = ((Steps)i).ToString();
                addChild(s);
                s.Y = squareHeight*(1+i);
                s.Border = new SolidBorder(4, Color.White);
                s.Backcolor = inactive;
            }

            opponentsTurn = new Square(width, squareHeight);
            opponentsTurn.Text = "Opponents Turn";
            addChild(opponentsTurn);
            opponentsTurn.Border = new SolidBorder(4, Color.White);
            opponentsTurn.Backcolor = Color.Black;

            yourTurn = new Square(width, squareHeight);
            yourTurn.Text = "Your Turn";
            yourTurn.Y = squares.Length*squareHeight + squareHeight;
            addChild(yourTurn);
            yourTurn.Border = new SolidBorder(4, Color.White);
            yourTurn.Backcolor = Color.Black;
        }

        private Square activeSquare;
        private Color inactive = Color.Gray;
        private Color active = Color.DarkGreen;
        private Color heroturn = Color.DodgerBlue;
        private Color oppturn = Color.Maroon;
        private Color notturn = Color.Black;

        public void setActive(Steps step, bool herosTurn)
        {
            int ix = (int)step;
            if (activeSquare != null) activeSquare.Backcolor = inactive;
            activeSquare = squares[ix];
            activeSquare.Backcolor = active;

            yourTurn.Backcolor = herosTurn ? heroturn : notturn;
            opponentsTurn.Backcolor = !herosTurn ? oppturn : notturn;
        }
    }
}
