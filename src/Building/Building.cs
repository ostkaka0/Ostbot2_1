using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OstBot_2_1.Building
{
    abstract class Building
    {
        int x;
        int y;
        int width;
        int height;
        string owner;
        float health;

        Block[][,] oldBlocks = new Block[2][,];

        public abstract KeyValuePair<int, int> Cost();

        //public virtual Structure getStructure();

    }
}
