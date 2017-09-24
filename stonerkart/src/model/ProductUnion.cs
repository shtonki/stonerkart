using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{

    public interface Product
    {
        int Price { get; }
    }

    [Serializable]
    public class Pack : Product
    {
        public Packs Template { get; }

        public Pack(Packs template)
        {
            this.Template = template;
        }

        public int Price
        {
            get
            {
                switch (Template)
                {
                    case Packs.FirstEdition12Pack:
                        return 420;

                    case Packs.FirstEdition40Pack:
                        return 1420;

                    default:
                        throw new Exception();
                }
            }
        }
    }

    public enum Packs
    {
        FirstEdition12Pack,
        FirstEdition40Pack,
    }
}
