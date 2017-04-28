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
        public StackWrapper wrapper;

        public CastSelection(StackWrapper wrapper)
        {
            this.wrapper = wrapper;
        }

        private static TargetMatrix[] matriciesFromString(String s, Game g)
        {
            int i = 0;

            List<TargetMatrix> matricies = new List<TargetMatrix>();
            List<TargetColumn> columns = null;
            List<Targetable> targets = null;

            while (i < s.Length)
            {
                Func<char> nextChar = () => s[i++];
                char c = nextChar();
                switch (c)
                {
                    case '{':
                    {
                        if (columns != null) throw new Exception();
                        columns = new List<TargetColumn>();
                    }
                        break;

                    case '}':
                    {
                        if (columns == null) throw new Exception();
                        matricies.Add(new TargetMatrix(columns));
                        columns = null;
                    }
                        break;

                    case '[':
                    {
                        if (targets != null) throw new Exception();
                        targets = new List<Targetable>();
                    }
                        break;

                    case ']':
                    {
                        if (targets == null) throw new Exception();
                        columns.Add(new TargetColumn(targets));
                        targets = null;
                    }
                        break;


                default:
                    {
                        //let's abuse switches
                        StringBuilder sb = new StringBuilder();
                        while (true)
                        {
                            char x = nextChar();
                            if (x == 'x') break;
                            sb.Append(x);
                        }
                        int targetOrd = Int32.Parse(sb.ToString());
                        Targetable target = null;
                        switch (c)
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
                        }

                        if (target == null) throw new Exception();

                        targets.Add(target);
                    } break;
                }
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

                TargetMatrix[] targetMatricies = matriciesFromString(ss[2], g);

                TargetMatrix[] costs = matriciesFromString(ss[3], g);

                wrapper = new StackWrapper(card, ability, targetMatricies, costs);
            }
        }

        private static void matriciesToString(StringBuilder sb, TargetMatrix[] ms, Game g)
        {
            foreach (TargetMatrix tm in ms)
            {
                sb.Append('{');
                foreach (TargetColumn col in tm.columns)
                {
                    sb.Append('[');
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
                        sb.Append('x');
                    }
                    sb.Append(']');
                }
                sb.Append('}');
            }
        }

        public string toString(Game g)
        {
            if (wrapper == null)
            {
                return "";
            }
            else
            {
                StringBuilder sb = new StringBuilder();

                int cardOrd = g.ord(wrapper.castingCard);
                sb.Append(cardOrd);
                sb.Append(';');

                int abilityOrd = wrapper.castingCard.abilityOrd(wrapper.ability);
                sb.Append(abilityOrd);
                sb.Append(';');

                matriciesToString(sb, wrapper.targetMatrices, g);
                sb.Append(';');

                matriciesToString(sb, wrapper.costMatricies, g);

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
                int length = Int32.Parse(foo[1]);

                Card card = g.cardFromOrd(cardOrd);

                var ts = foo.Reverse().Take(foo.Count() - 2).Reverse().Select(w => g.tileFromOrd(Int32.Parse(w)));

                Path path = new Path(ts.ToList(), length);

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
                Path p = v.Item2;
                var ts = p.tyles;
                sb.Append(g.ord(c));
                sb.Append(',');
                sb.Append(p.length);
                foreach (var t in ts)
                {
                    sb.Append(',');
                    sb.Append(g.ord(t));
                }
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
            if (s.Length == 0)
            {
                choices = new int[0];
            }
            else
            {
                string[] ss = s.Split(',');
                choices = ss.Select(Int32.Parse).ToArray();
            }
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

    class TriggeredAbilitiesGluer : GameAction
    {
        public ChoiceSelection choices;
        public CastSelection[] castSelections;

        public TriggeredAbilitiesGluer()
        {
            choices = new ChoiceSelection();
            castSelections = new CastSelection[0];
        }

        public TriggeredAbilitiesGluer(ChoiceSelection choices, CastSelection[] castSelections)
        {
            this.choices = choices;
            this.castSelections = castSelections;
        }

        public TriggeredAbilitiesGluer(Game g, string s)
        {
            var ss = s.Split('+');

            choices = new ChoiceSelection(g, ss[0]);
            castSelections = ss.Skip(1).Select(str => new CastSelection(g, str)).ToArray();

            if (choices.choices.Length != castSelections.Length) throw new Exception();
        }

        public string toString(Game g)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(choices.toString(g));
            sb.Append('+');

            foreach (var c in castSelections)
            {
                sb.Append(c.toString(g));
                sb.Append('+');
            }
            sb.Length--;
            return sb.ToString();
        }
    }
}
