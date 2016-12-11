using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    interface GameAction
    {
        string toString(Game g);
    }

    class ManaOrbSelection : GameAction
    {
        public ManaColour orb;

        public ManaOrbSelection(ManaColour orb)
        {
            this.orb = orb;
        }

        public ManaOrbSelection(Game g, string data)
        {
            int i = Int32.Parse(data);
            orb = (ManaColour)i;
        }

        public string toString(Game g)
        {
            return ((int)orb).ToString();
        }
    }

    class CastSelection : GameAction
    {
        public StackWrapper? wrapper;

        public CastSelection(StackWrapper? wrapper)
        {
            this.wrapper = wrapper;
        }

        public CastSelection(Game g, string s)
        {
            if (s.Length == 0)
            {
                wrapper = null;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public string toString(Game g)
        {
            if (!wrapper.HasValue)
            {
                return "";
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    class MoveSelection : GameAction
    {
        public List<Tuple<Card, Path>> moves;

        public MoveSelection(List<Tuple<Card, Path>> moves)
        {
            this.moves = moves;
        }

        public MoveSelection(Game g, string s)
        {
            string[] ss = s.Split(';');
            moves = new List<Tuple<Card, Path>>();

            foreach (string str in ss)
            {
                string[] foo = str.Split(',');

                int cardOrd = Int32.Parse(foo[0]);
                int tileOrd = Int32.Parse(foo[1]);

                Card card = g.cardFromOrd(cardOrd);
                Tile tile = g.tileFromOrd(tileOrd);

                Path path = g.pathTo(card, tile);

                moves.Add(new Tuple<Card, Path>(card, path));
            }
        }

        public string toString(Game g)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var v in moves)
            {
                Card c = v.Item1;
                Tile t = v.Item2.to;
                sb.Append(g.ord(c));
                sb.Append(',');
                sb.Append(g.ord(t));
                sb.Append(';');
            }
            sb.Length--;
            return sb.ToString();
        }
    }
}
