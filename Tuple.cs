using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    class Tuple
    {
        int x;
        int y;
        int greatestDiff;
        public Tuple(int x, int y, int greatestDiff)
        {
            this.x = x;
            this.y = y;
            this.greatestDiff = greatestDiff;
        }
        public Tuple()
        { }

        public int getX()
        {
            return x;
        }
        public void setX(int temp)
        {
            x = temp;
        }
        public int getY()
        {
            return y;
        }
        public void setY(int temp)
        {
            y = temp;
        }
        public int getGreatestDiff()
        {
            return greatestDiff;
        }
        public void setGreatestDiff(int temp)
        {
            greatestDiff = temp;
        }

    }
}
