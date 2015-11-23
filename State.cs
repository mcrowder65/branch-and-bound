using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    public class State:IComparable<State>
    {
        double[,] map;
        double LB;
        int index;
        Dictionary<int, int> edges;
        public State() { }
        public State(double[,] map)
        {
            this.map = map;
            initializeEdges();
        }
        public State(double[,] map, double LB, Dictionary<int,int> edges)
        {
            this.map = map;
            this.LB = LB;
            this.edges = edges;
        }
        public void initializeEdges()
        {
            edges = new Dictionary<int, int>();
            for(int i = 0; i < map.GetLength(0); i++)
            {
                edges[i] = -1;
            }
        }
        public void setEdge(int key, int value)
        {
            edges[key] = value;
        }
        public void setEdges(Dictionary<int, int> temp)
        {
            edges = temp;
        }
        public int getEdge(int temp)
        {
            return edges[temp];
        }
        public Dictionary<int, int> getEdges()
        {
            return edges;
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

        int IComparable<State>.CompareTo(State other)
        {
            if (other == null)
                return 1;
            if (LB == other.getLB())
                return 0;
            else if (LB < other.getLB())
                return -1;
            else
                return 1;
        }
        /*public static bool operator <(State s1, State s2)
        {
            return s1.CompareTo(s2) < 0;
        }*/
    }
}
