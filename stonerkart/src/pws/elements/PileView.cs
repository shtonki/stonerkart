using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace stonerkart
{
    class PileView : Square, Observer<PileChangedMessage>
    {
        private List<CardView> cardViews;
        private int? rows = 1;
        private int? columns = null;

        public int maxPadding { get; set; } = 0;

        public PileView()
        {
            Backcolor = Color.Crimson;
        }


        public override int Height
        {
            get { return base.Height; }
            set
            {
                setSize(Width, value);
            }
        }

        public override int Width
        {
            get { return base.Width; }
            set
            {
                setSize(value, Height);
            }
        }

        public int? Rows
        {
            get { return rows; }
            set
            {
                rows = value;
                Columns = null;
            }
        }

        public int? Columns
        {
            get { return columns; }
            set
            {
                columns = value;
                rows = null;
            }
        }

        public override void setSize(int width, int height, TextLayout layout = null)
        {
            base.setSize(width, height, layout);
            layoutCards();
        }

        public void sub(PublicSaxophone sax)
        {
            sax.sub(this, (a, p) => viewAt(a.X - AbsoluteX, a.Y - AbsoluteY)?.card);
        }

        public void notify(object o, PileChangedMessage t)
        {
            if (cardViews == null)
            {
                populate((CardList)o);
            }
            else
            {
                update(t);
            }
            layoutCards();
        }

        private void update(PileChangedMessage m)
        {
            foreach (var c in m.cards)
            {
                if (m.arg == PileChangedArg.Add)
                {
                    addCardView(c);
                }
                else if (m.arg == PileChangedArg.Remove)
                {
                    removeCardView(c);
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        private void addCardView(Card c)
        {
            lock (this)
            {
                if (cardViews.Any(crd => crd.card == c)) throw new Exception();
                CardView cv = new CardView(c);
                cardViews.Add(cv);
                addChild(cv);
                cv.Hoverable = false;
                cv.Visible = false;
            }
        }

        private void removeCardView(Card c)
        {
            lock (this)
            {
                CardView cv = cardViews.First(cw => cw.card == c);
                cardViews.Remove(cv);
                removeChild(cv);
            }
        }

        private void populate(CardList list)
        {
            cardViews = new List<CardView>();

            foreach (var c in list)
            {
                addCardView(c);
            }
        }

        private void layoutCards()
        {
            lastBroughtToFront = null;
            if (cardViews == null || cardViews.Count == 0) return;

            lock (this)
            {
                int cvWidth;
                int cvHeight;
                int xInc;
                int yInc;
                double xPad;
                double yPad;
                int count;


                if (Rows.HasValue)
                {
                    cvHeight = cardViews[0].Height = Height/Rows.Value;
                    cvWidth = cardViews[0].Width;

                    xPad = cardViews.Count == 1 ? 0 : ((double)(Width - cvWidth)) / (cardViews.Count - 1);
                    yPad = 0;
                    xInc = 0;
                    yInc = cvHeight;
                    if (xPad > cvWidth) xPad = cvWidth;
                    count = (1 + cardViews.Count)/Rows.Value;
                }
                else if (Columns.HasValue)
                {
                    cvWidth = cardViews[0].Width = Width/Columns.Value;
                    cvHeight = cardViews[0].Height;
                    yPad = cardViews.Count == 1 ? 0 : ((double)(Height - cvHeight))/(cardViews.Count - 1);
                    xPad = 0;
                    xInc = cvWidth;
                    yInc = 0;
                    if (yPad > cvHeight) yPad = cvHeight;
                    count = (1 + cardViews.Count)/Columns.Value;
                }
                else throw new Exception();

                if (maxPadding > 0)
                {
                    xPad = xPad > maxPadding ? maxPadding : xPad;
                    yPad = yPad > maxPadding ? maxPadding : yPad;
                }

                int xorg = 0;
                int yorg = 0;
                int i = 0;
                while (i < cardViews.Count)
                {
                    for (int j = 0; j < count; j++)
                    {
                        if (i >= cardViews.Count) break;
                        CardView cv = cardViews[i];
                        cv.X = (int)Math.Round(j*xPad + xorg);
                        cv.Y = (int)Math.Round(j*yPad + yorg);
                        cv.Height = cvHeight;
                        cv.Visible = true;
                        i++;
                    }
                    xorg += xInc;
                    yorg += yInc;
                }
            }
        }

        private CardView lastBroughtToFront;

        public override void onMouseMove(MouseMoveEventArgs args)
        {
            base.onMouseMove(args);
            var cv = viewAt(args.X - X, args.Y - Y);
            if (cv != lastBroughtToFront && lastBroughtToFront != null)
            {
                int ix = cardViews.IndexOf(lastBroughtToFront);
                lastBroughtToFront.DrawOrder = ix;
            }
            if (cv != null) cv.DrawOrder = -1;
            lastBroughtToFront = cv;
        }

        public override void onMouseExit(MouseMoveEventArgs args)
        {
            base.onMouseExit(args);

            if (lastBroughtToFront != null)
            {
                int ix = cardViews.IndexOf(lastBroughtToFront);
                lastBroughtToFront.DrawOrder = ix;
            }
        }

        public override void onClick(MouseButtonEventArgs args)
        {
            base.onClick(args);
        }

        private CardView viewAt(int clickx, int clicky)
        {
            if (cardViews == null || cardViews.Count == 0) return null;

            int clickxadj = clickx + cardViews[0].Width/2;
            int clickyadj = clicky + cardViews[0].Height/2;

            CardView best = null;
            int d = Int32.MaxValue;

            foreach (var cv in cardViews)
            {
                if (clickx > cv.X && clickx < cv.X + cv.Width &&
                    clicky > cv.Y && clicky < cv.Y + cv.Height)
                {
                    int dx = cv.X - clickxadj;
                    int dy = cv.Y - clickyadj;
                    int cd = (int)Math.Sqrt(dx*dx + dy*dy);
                    if (cd < d)
                    {
                        best = cv;
                        d = cd;
                    }
                }
            }

            return best;
        }
    }
}
