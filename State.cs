using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    class State
    {
        double[,] map;
        double LB;
        public State() { }
        public State(double[,] map)
        {
            this.map = map;
        }
        public void setMap(double[,] temp)
        {
            map = temp;
        }
        public void setPoint(int x, int y, double value)
        {
            map[x, y] = value;
        }
        public double getPoint(int x, int y)
        {
            return map[x, y];
        }
        public double[,] getMap()
        {
            return map;
        }
        public void setLB(double temp)
        {
            LB = temp;
        }
        public double getLB()
        {
            return LB;
        }
    }
}
