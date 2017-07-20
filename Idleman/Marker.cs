using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace Idleman
{

    [Flags]
    enum Monster
    {
        BobTheNecromancer,
        ZombieHorde,
        RedKnight
    }

    class Position
    {
        Type Type { get; set; }

        public Position(Point point)
        {
            
        }

        public Position(Anchor anchor)
        {
            
        }
    }

    public class Marker
    {
        // TODO: Change type to more appropriate for hash data set.
        string Id { get; set; }
        string Name { get; set;}
        Shape Shape { get; set; }
        Position Position { get; set; }

        Vector Offset { get; set; }

        Monster Monster { get; set; }

        public Marker()
        {
            Monster = Monster.BobTheNecromancer | Monster.RedKnight;
        }
    }
}
