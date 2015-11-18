using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace TSP
{

    class ProblemAndSolver
    {

        private class TSPSolution
        {
            /// <summary>
            /// we use the representation [cityB,cityA,cityC] 
            /// to mean that cityB is the first city in the solution, cityA is the second, cityC is the third 
            /// and the edge from cityC to cityB is the final edge in the path.  
            /// You are, of course, free to use a different representation if it would be more convenient or efficient 
            /// for your node data structure and sear1h algorithm. 
            /// </summary>
            public ArrayList
                Route;

            public TSPSolution(ArrayList iroute)
            {
                Route = new ArrayList(iroute);
            }


            /// <summary>
            /// Compute the cost of the current route.  
            /// Note: This does not check that the route is complete.
            /// It assumes that the route passes from the last city back to the first city. 
            /// </summary>
            /// <returns></returns>
            public double costOfRoute()
            {
                // go through each edge in the route and add up the cost. 
                int x;
                City here;
                double cost = 0D;

                for (x = 0; x < Route.Count - 1; x++)
                {
                    here = Route[x] as City;
                    cost += here.costToGetTo(Route[x + 1] as City);
                }

                // go from the last city to the first. 
                here = Route[Route.Count - 1] as City;
                cost += here.costToGetTo(Route[0] as City);
                return cost;
            }
        }

        #region Private members 

        /// <summary>
        /// Default number of cities (unused -- to set defaults, change the values in the GUI form)
        /// </summary>
        // (This is no longer used -- to set default values, edit the form directly.  Open Form1.cs,
        // click on the Problem Size text box, go to the Properties window (lower right corner), 
        // and change the "Text" value.)
        private const int DEFAULT_SIZE = 25;

        private const int CITY_ICON_SIZE = 5;

        // For normal and hard modes:
        // hard mode only
        private const double FRACTION_OF_PATHS_TO_REMOVE = 0.20;

        /// <summary>
        /// the cities in the current problem.
        /// </summary>
        private City[] Cities;

        /// <summary>
        /// a route through the current problem, useful as a temporary variable. 
        /// </summary>
        private ArrayList Route;
        /// <summary>
        /// best solution so far. 
        /// </summary>
        private TSPSolution bssf; 

        /// <summary>
        /// how to color various things. 
        /// </summary>
        private Brush cityBrushStartStyle;
        private Brush cityBrushStyle;
        private Pen routePenStyle;


        /// <summary>
        /// keep track of the seed value so that the same sequence of problems can be 
        /// regenerated next time the generator is run. 
        /// </summary>
        private int _seed;
        /// <summary>
        /// number of cities to include in a problem. 
        /// </summary>
        private int _size;

        private double[,] map;
        /// <summary>
        /// Difficulty level
        /// </summary>
        private HardMode.Modes _mode;

        /// <summary>
        /// random number generator. 
        /// </summary>
        private Random rnd;
        #endregion

        #region Public members
        public int Size
        {
            get { return _size; }
        }

        public int Seed
        {
            get { return _seed; }
        }
        #endregion

        #region Constructors
        public ProblemAndSolver()
        {
            this._seed = 1; 
            rnd = new Random(1);
            this._size = DEFAULT_SIZE;

            this.resetData();
        }

        public ProblemAndSolver(int seed)
        {
            this._seed = seed;
            rnd = new Random(seed);
            this._size = DEFAULT_SIZE;

            this.resetData();
        }

        public ProblemAndSolver(int seed, int size)
        {
            this._seed = seed;
            this._size = size;
            rnd = new Random(seed); 
            this.resetData();
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Reset the problem instance.
        /// </summary>
        private void resetData()
        {

            Cities = new City[_size];
            map = new Double[Cities.Length, Cities.Length];
            Route = new ArrayList(_size);
            bssf = null;

            if (_mode == HardMode.Modes.Easy)
            {
                for (int i = 0; i < _size; i++)
                    Cities[i] = new City(rnd.NextDouble(), rnd.NextDouble());
            }
            else // Medium and hard
            {
                for (int i = 0; i < _size; i++)
                    Cities[i] = new City(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble() * City.MAX_ELEVATION);
            }

            HardMode mm = new HardMode(this._mode, this.rnd, Cities);
            if (_mode == HardMode.Modes.Hard)
            {
                int edgesToRemove = (int)(_size * FRACTION_OF_PATHS_TO_REMOVE);
                mm.removePaths(edgesToRemove);
            }
            City.setModeManager(mm);

            cityBrushStyle = new SolidBrush(Color.Black);
            cityBrushStartStyle = new SolidBrush(Color.Red);
            routePenStyle = new Pen(Color.Blue,1);
            routePenStyle.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// make a new problem with the given size.
        /// </summary>
        /// <param name="size">number of cities</param>
        //public void GenerateProblem(int size) // unused
        //{
        //   this.GenerateProblem(size, Modes.Normal);
        //}

        /// <summary>
        /// make a new problem with the given size.
        /// </summary>
        /// <param name="size">number of cities</param>
        public void GenerateProblem(int size, HardMode.Modes mode)
        {
            this._size = size;
            this._mode = mode;
            resetData();
        }

        /// <summary>
        /// return a copy of the cities in this problem. 
        /// </summary>
        /// <returns>array of cities</returns>
        public City[] GetCities()
        {
            City[] retCities = new City[Cities.Length];
            Array.Copy(Cities, retCities, Cities.Length);
            return retCities;
        }

        /// <summary>
        /// draw the cities in the problem.  if the bssf member is defined, then
        /// draw that too. 
        /// </summary>
        /// <param name="g">where to draw the stuff</param>
        public void Draw(Graphics g)
        {
            float width  = g.VisibleClipBounds.Width-45F;
            float height = g.VisibleClipBounds.Height-45F;
            Font labelFont = new Font("Arial", 10);

            // Draw lines
            if (bssf != null)
            {
                // make a list of points. 
                Point[] ps = new Point[bssf.Route.Count];
                int index = 0;
                foreach (City c in bssf.Route)
                {
                    if (index < bssf.Route.Count -1)
                        g.DrawString(" " + index +"("+c.costToGetTo(bssf.Route[index+1]as City)+")", labelFont, cityBrushStartStyle, new PointF((float)c.X * width + 3F, (float)c.Y * height));
                    else 
                        g.DrawString(" " + index +"("+c.costToGetTo(bssf.Route[0]as City)+")", labelFont, cityBrushStartStyle, new PointF((float)c.X * width + 3F, (float)c.Y * height));
                    ps[index++] = new Point((int)(c.X * width) + CITY_ICON_SIZE / 2, (int)(c.Y * height) + CITY_ICON_SIZE / 2);
                }

                if (ps.Length > 0)
                {
                    g.DrawLines(routePenStyle, ps);
                    g.FillEllipse(cityBrushStartStyle, (float)Cities[0].X * width - 1, (float)Cities[0].Y * height - 1, CITY_ICON_SIZE + 2, CITY_ICON_SIZE + 2);
                }

                // draw the last line. 
                g.DrawLine(routePenStyle, ps[0], ps[ps.Length - 1]);
            }

            // Draw city dots
            foreach (City c in Cities)
            {
                g.FillEllipse(cityBrushStyle, (float)c.X * width, (float)c.Y * height, CITY_ICON_SIZE, CITY_ICON_SIZE);
            }

        }

        /// <summary>
        ///  return the cost of the best solution so far. 
        /// </summary>
        /// <returns></returns>
        public double costOfBssf ()
        {
            if (bssf != null)
                return (bssf.costOfRoute());
            else
                return -1D; 
        }

        /// <summary>
        ///  solve the problem.  This is the entry point for the solver when the run button is clicked
        /// right now it just picks a simple solution. 
        /// </summary>
        public void solveProblem()
        {
            Console.WriteLine("cities.length: " + Cities.Length);
            for (int i = 0; i < Cities.Length; i++)
            {
                for(int x = 0; x < Cities.Length; x++)
                {
                    map[i, x] = Cities[i].costToGetTo(Cities[x]);
                    if (map[i, x] == 0)
                        map[i, x] = double.PositiveInfinity;
                }
            }
            reduceMatrix();
            double bssf = double.PositiveInfinity;
            // outputMap();
           int  iterator = 0;
            while (double.IsPositiveInfinity(bssf)) //keep going to make sure I don't end up with infinity
            {
                iterator++;
                for (int i = 0; i < 10; i++)
                {
                    double tempGreedy = greedy();
                    if (tempGreedy < bssf)
                        bssf = tempGreedy;

                }
            }
            Console.WriteLine(iterator*10);
            Console.WriteLine("bssf: " + bssf);
        }   
        public double greedy()
        {
            double cost = 0;
            List<double> path = new List<double>();
            Random rand = new Random();
            HashSet<double> randNums = new HashSet<double>();
            PointF firstPoint = new PointF();

            for(int i = 0; i < Cities.Length; i++)
            {
                double index = 0;
                int size = randNums.Count;
                while(size == randNums.Count)
                {
                    index = rand.Next(0, Cities.Length);
                    randNums.Add(index);
                }
                cost += map[i, Convert.ToInt32(index)];
                if (cost == double.PositiveInfinity)
                    return cost;

               // Console.WriteLine(cost);
                path.Add(index);
                if(i == 0)
                {
                    firstPoint.X = i;
                    firstPoint.Y = Convert.ToInt32(index);
                }
            }
            cost += map[Convert.ToInt32(path[path.Count - 1]), Convert.ToInt32(firstPoint.X)];
            path.Add(path[0]);
            
            return cost;
        }
        public void outputMap()
        {
            string[,] copy = new string[Cities.Length, Cities.Length];
            for(int i = 0; i < Cities.Length; i++)
            {
                for(int j = 0; j < Cities.Length; j++)
                {
                    if (map[i, j] == double.PositiveInfinity)
                        copy[i, j] = "****";
                    else if (map[i, j] == 0)
                        copy[i, j] = "@@@@";
                    else
                        copy[i, j] = Convert.ToString(map[i, j]);
                    
                    if (copy[i, j].Length == 1)
                        copy[i, j] = "000" + copy[i, j];
                    if (copy[i, j].Length == 2)
                        copy[i, j] = "00" + copy[i, j];
                    if (copy[i, j].Length == 3)
                        copy[i, j] = "0" + copy[i, j];
                }
            }
            for(int i = 0; i < Cities.Length; i++)
            {
                for(int j = 0; j < Cities.Length; j++)
                {
                    Console.Write(copy[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
        public void reduceMatrix()
        {
            for (int i = 0; i < Cities.Length; i++) // reduce rows
            {
                double minimum = double.PositiveInfinity;
                for (int j = 0; j < Cities.Length; j++)
                {
                    if (map[i, j] < minimum)
                        minimum = map[i, j];
                }
                if (minimum == 0) //if there is already a 0 in that row, don't waste time looping through it
                    continue;
                for (int j = 0; j < Cities.Length; j++)
                {
                    map[i, j] = map[i, j] - minimum;
                }
            }
            for (int j = 0; j < Cities.Length; j++) //reduce columns
            {
                double minimum = double.PositiveInfinity;
                for (int i = 0; i < Cities.Length; i++)
                {
                    if (map[i, j] < minimum)
                        minimum = map[i, j];
                }
                if (minimum == 0) //if there is already a 0 in that column, don't waste time looping through it
                    continue;
                for (int i = 0; i < Cities.Length; i++)
                    map[i, j] = map[i, j] - minimum;
            }
        }
        #endregion
    }

}
