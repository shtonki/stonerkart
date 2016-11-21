using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Cost
    {
        private List<SubCost> costs;

        public Cost(params SubCost[] costs)
        {
            this.costs = new List<SubCost>(costs);
        }
    }

    interface SubCost
    {
        
    }

    class ManaCost : SubCost
    {
        public int c;

        public ManaCost(int c)
        {
            this.c = c;
        }
    }
}
