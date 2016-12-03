using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    interface GameAction
    {
        byte[] toBytes();
        GameAction fromBytes(byte[] bytes);
    }

    class ManaOrbSelection : GameAction
    {
        public ManaColour orb;

        public ManaOrbSelection(ManaColour orb)
        {
            this.orb = orb;
        }

        public byte[] toBytes()
        {
            throw new NotImplementedException();
        }

        public GameAction fromBytes(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }

    class CastSelection : GameAction
    {
        public StackWrapper? wrapper;

        public CastSelection(StackWrapper? wrapper)
        {
            this.wrapper = wrapper;
        }

        public GameAction fromBytes(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public byte[] toBytes()
        {
            throw new NotImplementedException();
        }
    }

    class MoveSelection : GameAction
    {
        public List<Tuple<Card, Path>> moves;

        public MoveSelection(List<Tuple<Card, Path>> moves)
        {
            this.moves = moves;
        }

        public GameAction fromBytes(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public byte[] toBytes()
        {
            throw new NotImplementedException();
        }
    }
}
