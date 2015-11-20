using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    public class State
    {
        double[,] map;
        double LB;
        int index;
        public State() { }
        public State(double[,] map)
        {
            this.map = map;
        }
        public State(double[,] map, double LB)
        {
            this.map = map;
            this.LB = LB;
        }
        public int getIndex()
        {
            return index;
        }
        public void setIndex(int temp)
        {
            index = temp;
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
