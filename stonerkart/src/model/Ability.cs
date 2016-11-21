using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    abstract class Ability
    {

    }

    class ActivatedAbility : Ability
    {
        public Cost costs;
        public PileLocation castableFrom;
        public List<TargetRule> targets;

        public ActivatedAbility(Cost costs, PileLocation castableFrom, List<TargetRule> targets)
        {
            this.costs = costs;
            this.castableFrom = castableFrom;
            this.targets = targets;
        }
    }
}
