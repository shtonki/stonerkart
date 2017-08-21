using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace stonerkart
{
    class PileView : Square, Observer<PileChangedMessage>
    {
        private List<CardView> cardViews = new List<CardView>();

        public PileView()
        {
        }

        public PileView(CardList cl)
        {
            cl.addObserver(this);
            layoutCards();
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

        public override void setSize(int width, int height, TextLayout layout = null)
        {
            base.setSize(width, height, layout);
            layoutCards();
        }

        public void notify(object o, PileChangedMessage t)
        {
            populate((CardList)o);
            layoutCards();
        }

        private void populate(CardList list)
        {
            children.Clear();
            cardViews.Clear();

            foreach (var c in list)
            {
                CardView cv = new CardView(c);
                cardViews.Add(cv);
                addChild(cv);
                cv.hoverable = false;
            }
        }

        private void layoutCards()
        {
            if (cardViews.Count == 0) return;

            var cvWidth = Width/cardViews.Count;

            for (int i = 0; i < cardViews.Count; i++)
            {
                CardView cv = cardViews[i];
                cv.X = i*cvWidth;
                cv.Height = Height;
            }
        }

        private CardView lastBroughtToFront;

        public override void onMouseMove(MouseMoveEventArgs args)
        {
            base.onMouseMove(args);
            var cv = viewAt(args.X, args.Y);
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

        private CardView viewAt(int clickx, int clicky)
        {
            if (cardViews.Count == 0) return null;

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
