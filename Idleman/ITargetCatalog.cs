using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Idleman
{
    interface ITargetCatalog
    {

    }

    [Flags]
    enum Monster
    {
        BobTheNecromancer,
        ZombieHorde,
        RedKnight
    }

    class TargetCatalog : ITargetCatalog
    {
        Monster Monster { get; set; }

        public TargetCatalog()
        {
            Monster = Monster.BobTheNecromancer | Monster.RedKnight;
        }
    }

}
