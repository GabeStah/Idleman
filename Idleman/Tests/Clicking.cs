using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idleman.Tests
{
    class Clicking
    {
        public static void ClickZombidleWindowTest(int iterations = 20, int delay = 250, int xCoord = 250, int yCoord = 500)
        {
            var window = new Screen("ApolloRuntimeContentWindow", "Zombidle");
            for (int i = 1; i <= iterations; i++)
            {
                System.Threading.Thread.Sleep(delay);
                window.PostMouseClick(xCoord, yCoord);
            }
        }
    }
}
