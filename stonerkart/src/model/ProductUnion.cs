using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{

    public class ProductUnion
    {
        private Packs? pack { get; }
        public bool isPack => pack.HasValue;
        public Packs Pack => pack.Value;
        

        public int price => priceEx();

        public ProductUnion(Packs pack)
        {
            this.pack = pack;
        }

        protected int priceEx()
        {
            return 420;
        }

        public override string ToString()
        {
            return pack.ToString();
        }

        public static ProductUnion FromString(string s)
        {
            Packs p;
            if (!Packs.TryParse(s, out p)) throw new Exception();
            return new ProductUnion(p);
        }
    }

    public enum Packs
    {
        FirstEdition12Pack
    }
}
