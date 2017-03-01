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

        private static TargetMatrix[] xd(String s, Game g)
        {
            List<TargetMatrix> matricies = new List<TargetMatrix>();
            string[] matrixStrings = s.Split('.');
            foreach (string matrixString in matrixStrings)
            {
                List<TargetColumn> columns = new List<TargetColumn>();
                string[] colStrings = matrixString.Split(':');
                foreach (string colString in colStrings)
                {
                    List<Targetable> targets = new List<Targetable>();
                    string[] targetStrings = colString.Split(',');
                    foreach (string targetString in targetStrings)
                    {
                        if (targetString.Length == 0) continue;
                        char targetType = targetString[0];
                        int targetOrd = Int32.Parse(targetString.Substring(1));
                        Targetable target;
                        switch (targetType)
                        {
                            case 'c':
                            {
                                target = g.cardFromOrd(targetOrd);
                            } break;

                            case 't':
                            {
                                target = g.tileFromOrd(targetOrd);
                            } break;

                            case 'p':
                            {
                                target = g.playerFromOrd(targetOrd);
                            } break;

                            case 'm':
                            {
                                target = new ManaOrb((ManaColour)targetOrd);
                            } break;

                            default: throw new Exception();
                        }
                        targets.Add(target);
                    }
                    columns.Add(new TargetColumn(targets.ToArray()));
                }
                matricies.Add(new TargetMatrix(columns.ToArray()));
            }
            return matricies.ToArray();
        }

        public CastSelection(Game g, string s)
        {
            if (s.Length == 0)
            {
                wrapper = null;
            }
            else
            {
                string[] ss = s.Split(';');

                int cardOrd = Int32.Parse(ss[0]);
                Card card = g.cardFromOrd(cardOrd);

                int abilityOrd = Int32.Parse(ss[1]);
                Ability ability = card.abilityFromOrd(abilityOrd);


                TargetMatrix[] targetMatricies = xd(ss[2], g);

                TargetMatrix[] costs = xd(ss[3], g);

                wrapper = new StackWrapper(card, ability, targetMatricies, costs);
            }
        }

        private static void xd(StringBuilder sb, TargetMatrix[] ms, Game g)
        {
            foreach (TargetMatrix tm in ms)
            {
                foreach (TargetColumn col in tm.columns)
                {
                    foreach (Targetable v in col.targets)
                    {
                        if (v is Card)
                        {
                            sb.Append('c');
                            sb.Append(g.ord((Card)v));
                        }
                        else if (v is Tile)
                        {
                            sb.Append('t');
                            sb.Append(g.ord((Tile)v));
                        }
                        else if (v is Player)
                        {
                            sb.Append('p');
                            sb.Append(g.ord((Player)v));
                        }
                        else if (v is ManaOrb)
                        {
                            sb.Append('m');
                            sb.Append((int)((ManaOrb)v).colour);
                        }
                        else
                        {
                            throw new Exception();
                        }
                        sb.Append(',');
                    }
                    if (sb[sb.Length - 1] == ',') sb.Length--;
                    sb.Append(':');
                }
                if (sb[sb.Length - 1] == ':') sb.Length--;
                sb.Append('.');
            }
            if (sb[sb.Length - 1] == '.') sb.Length--;
            sb.Append(";");
        }

        public string toString(Game g)
        {
            if (!wrapper.HasValue)
            {
                return "";
            }
            else
            {
                StringBuilder sb = new StringBuilder();

                StackWrapper wp = wrapper.Value;

                int cardOrd = g.ord(wp.card);
                sb.Append(cardOrd);
                sb.Append(';');

                int abilityOrd = wp.card.abilityOrd(wp.ability);
                sb.Append(abilityOrd);
                sb.Append(';');

                xd(sb, wp.matricies, g);

                xd(sb, wp.costMatricies, g);

                return sb.ToString();
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
            moves = new List<Tuple<Card, Path>>();
            if (s.Length == 0) return;
            string[] ss = s.Split(';');

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
            if (moves.Count == 0) return "";
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

    class ChoiceSelection : GameAction
    {
        public int[] choices;

        public ChoiceSelection(IEnumerable<int> choices)
        {
            this.choices = choices.ToArray();
        }

        public ChoiceSelection(params int[] choices)
        {
            this.choices = choices;
        }

        public ChoiceSelection(Game g, string s)
        {
            string[] ss = s.Split(',');

            choices = ss.Select(Int32.Parse).ToArray();
        }

        public string toString(Game g)
        {
            if (!choices.Any()) return "";
            StringBuilder b = new StringBuilder();

            foreach (int choice in choices)
            {
                b.Append(choice);
                b.Append(',');
            }

            b.Length--;

            return b.ToString();
        }
    }
}
