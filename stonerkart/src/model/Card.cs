using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Card : Observable<CardChangedMessage>
    {
        public string name { get; }
        public Image image { get; }
        public Pile pile { get; private set; }

        public Card()
        {
            name = "Kappa Pride";
            image = Properties.Resources.jordanno;
        }

        public void moveTo(Pile p)
        {
            pile?.remove(this);
            pile = p;
            p.add(this);
        }
    }
}
