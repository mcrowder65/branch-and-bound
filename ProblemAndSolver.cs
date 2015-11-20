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

        public State initialState;
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
            initialState = new State(new Double[Cities.Length, Cities.Length]);
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
        double BSSF;
        Queue queue;
        /// <summary>
        ///  solve the problem.  This is the entry point for the solver when the run button is clicked
        /// right now it just picks a simple solution. 
        /// </summary>
       
        public void solveProblem()
        {

            /*   for (int i = 0; i < Cities.Length; i++)
               {
                   for(int x = 0; x < Cities.Length; x++)
                   {
                       initialState.setPoint(i, x, Cities[i].costToGetTo(Cities[x]));
                       if (initialState.getPoint(i, x) == 0)
                           initialState.setPoint(i, x, double.PositiveInfinity);
                   }
               }*/
            initialState.setMap(new double[4, 4]);
            //row 1
            initialState.setPoint(0, 0, double.PositiveInfinity);
            initialState.setPoint(0, 1, 7);
            initialState.setPoint(0, 2, 3);
            initialState.setPoint(0, 3, 12);
            //row 2
            initialState.setPoint(1, 0, 3);
            initialState.setPoint(1, 1, double.PositiveInfinity);
            initialState.setPoint(1, 2, 6);
            initialState.setPoint(1, 3, 14);
            //row 3
            initialState.setPoint(2, 0, 5);
            initialState.setPoint(2, 1, 8);
            initialState.setPoint(2, 2, double.PositiveInfinity);
            initialState.setPoint(2, 3, 6);
            //row 4
            initialState.setPoint(3, 0, 9);
            initialState.setPoint(3, 1, 3);
            initialState.setPoint(3, 2, 5);
            initialState.setPoint(3, 3, double.PositiveInfinity);



            reduceMatrix(initialState);

            BSSF = greedy(); //this is a global double
            queue = new Queue(); //this is a global queue of states
            findGreatestDiff(initialState);



            int iterator = 0;
            while(queue.size() != 0)
            {
                iterator++;
                State min = queue.deleteMin();
                if (min.getLB() > BSSF && queue.size() == 1)
                    break;
                else if (min.getLB() > BSSF)
                    continue;
                findGreatestDiff(min);
            }
            int hello = 0;
        }
        public void findGreatestDiff(State state)
        {
            int chosenX = 0;
            int chosenY = 0;
            int greatestDiff = 0;
            for (int i = 0; i < state.getMap().GetLength(0); i++)
            {
                for (int j = 0; j < state.getMap().GetLength(1); j++)
                {
                    if (state.getPoint(i, j) == 0)
                    {
                        int possibleMax = findExcludeMinusInclude(i, j, state);
                        if (possibleMax > greatestDiff)
                        {
                            chosenX = i;
                            chosenY = j;
                            greatestDiff = possibleMax;
                        }
                    }
                }
            }
            State include = makeInclude(chosenX, chosenY, state);
            if (BSSF > include.getLB())
                queue.insert(include);

            if (isAllInfinity(include))
                BSSF = include.getLB();

            State exclude = makeExclude(chosenX, chosenY, state);
            if (BSSF > exclude.getLB())
                queue.insert(exclude);

            if (isAllInfinity(exclude))
                BSSF = exclude.getLB();
        }
        public bool isAllInfinity(State state)
        {
            for(int i = 0; i < state.getMap().GetLength(0); i++)
            {
                for(int j = 0; j < state.getMap().GetLength(1); j++)
                {
                    if (state.getPoint(i, j) != double.PositiveInfinity)
                        return false;
                }
            }
            return true;
        }
        public int findExcludeMinusInclude(int x, int y, State state)
        {

            double[,] excludeMatrix = new double[4, 4];

            State excludeState = makeExclude(x, y, state);
            reduceMatrix(excludeState);

            State includeState = makeInclude(x, y, state);
            reduceMatrix(includeState);
            
            int cost = Convert.ToInt32(excludeState.getLB() - includeState.getLB());
            return cost;
        }
        public State makeInclude(int x, int y, State state)
        {
            State includeState = new State(copyMatrix(state.getMap()), state.getLB());
            includeState.setPoint(y, x, double.PositiveInfinity);
            for (int j = 0; j < includeState.getMap().GetLength(1); j++) //set the column to infinity
            {
                for (int i = 0; i < includeState.getMap().GetLength(0); i++)
                {
                    includeState.setPoint(x, j, double.PositiveInfinity);
                }
            }
            for (int i = 0; i < includeState.getMap().GetLength(0); i++) //set the row to infinity
            {
                for (int j = 0; j < includeState.getMap().GetLength(1); j++)
                {
                    includeState.setPoint(i, y, double.PositiveInfinity);
                }
            }
            reduceMatrix(includeState);
            return includeState;
        }
        public State makeExclude(int x, int y, State state)
        {
            State excludeState = new State(copyMatrix(state.getMap()), state.getLB());
            excludeState.setPoint(x, y, double.PositiveInfinity);
            reduceMatrix(excludeState);
            return excludeState;
        }
        public double[,] copyMatrix(double[,] initialMatrix)
        {
            double[,] copy = new double[initialMatrix.GetLength(0), initialMatrix.GetLength(1)];
            for(int i = 0; i < initialMatrix.GetLength(0); i++)
            {
                for(int j = 0; j < initialMatrix.GetLength(1); j++)
                {
                    copy[i, j] = initialMatrix[i, j];
                }
            }
            return copy;
        }
        public double greedy()
        {
            double BSSF = 0;
            ArrayList correctRoute = new ArrayList();
            List<int> visitedCities = new List<int>();
            int firstCity = -1;
            int iterations = 0;
            for (int city = 0; city < Cities.Length; city++)
            {
                iterations = 0;
                bool reloop = false;
                BSSF = 0;
                visitedCities.Clear();
                Route.Clear();
                Route.Add(Cities[city]);
                visitedCities.Add(city);
                for (int i = city; i < Cities.Length; i++)
                {
                    Console.Write(i + " ");
                    double minimum = double.PositiveInfinity;
                    int chosenCity = -1;
                    for (int j = 0; j < Cities.Length; j++)
                    {
                        double potentialMinimum = Cities[i].costToGetTo(Cities[j]);
                        if (potentialMinimum < minimum && !visitedCities.Contains(j) && i != j)
                        {
                            minimum = potentialMinimum;
                            chosenCity = j;
                        }
                    }
                    if (chosenCity == -1)
                    {
                        break;
                    }
                    else
                    {
                        BSSF += minimum;
                        visitedCities.Add(chosenCity);
                        Route.Add(Cities[chosenCity]);
                    }
                    if (i == Cities.Length - 1 && iterations != Cities.Length - 1)
                    {
                        reloop = true;
                        i = -1;
                    }
                    if (reloop)
                    {
                        if (i == city-1)
                            break;
                    }
                    iterations++;
                }
                if (BSSF < double.PositiveInfinity)
                {
                    firstCity = city;
                    correctRoute = new ArrayList(Route);
                    break;
                }
                
            }
            int lastCity = visitedCities[visitedCities.Count - 1];
            BSSF += Cities[lastCity].costToGetTo(Cities[firstCity]);
            
            bssf = new TSPSolution(correctRoute);
            // update the cost of the tour. 
            Program.MainForm.tbCostOfTour.Text = " " + bssf.costOfRoute();
            // do a refresh. 
            Program.MainForm.Invalidate();
            return BSSF;
        }
        public double random(State currentState)
        {
            Random rand = new Random();
            double randNum = rand.Next(0, Cities.Length);
            Route = new ArrayList(Cities.Length);
            double BSSF = 0;
            List<double> randNums = new List<double>();
            randNums.Add(randNum);
            Route.Add(Cities[Convert.ToInt32(randNum)]);
            for(int i = 0; i < Cities.Length-1; i++)
            {
                double startNode = randNums[randNums.Count - 1];
                int size = randNums.Count;
                while (size == randNums.Count)
                {
                    randNum = rand.Next(0, Cities.Length);
                    if (randNums.Contains(randNum))
                        randNum = rand.Next(0, Cities.Length);
                    else
                    {
                        randNums.Add(randNum);
                        Route.Add(Cities[Convert.ToInt32(randNum)]);
                    }
                }
                BSSF += currentState.getPoint(Convert.ToInt32(startNode), Convert.ToInt32(randNum));
                
                if (BSSF == double.PositiveInfinity)
                    return BSSF;
            }
            Route.Add(Cities[Convert.ToInt32(randNums[0])]);
            BSSF += currentState.getPoint(Convert.ToInt32(randNums[randNums.Count - 1]), Convert.ToInt32(randNums[0]));


            bssf = new TSPSolution(Route);
            // update the cost of the tour. 
            Program.MainForm.tbCostOfTour.Text = " " + bssf.costOfRoute();
            // do a refresh. 
            Program.MainForm.Invalidate();
            return BSSF;

        }
        public void outputMap(State currentState)
        {
            string[,] copy = new string[currentState.getMap().GetLength(0), currentState.getMap().GetLength(1)];
            for(int i = 0; i < currentState.getMap().GetLength(0); i++)
            {
                for(int j = 0; j < currentState.getMap().GetLength(1); j++)
                {
                    if (currentState.getPoint(i, j) == double.PositiveInfinity)
                        copy[i, j] = "-";
                    else if (currentState.getPoint(i, j) == 0)
                        copy[i, j] = "0";
                    else
                        copy[i, j] = Convert.ToString(currentState.getPoint(i, j));
                }
            }
            for(int i = 0; i < currentState.getMap().GetLength(0); i++)
            {
                for(int j = 0; j < currentState.getMap().GetLength(1); j++)
                {
                    Console.Write(copy[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
        public void reduceMatrix(State currentState)
        {
            outputMap(currentState);
            double lowerBound = currentState.getLB() ;
            for (int i = 0; i < currentState.getMap().GetLength(0); i++) // reduce rows
            {
                double minimum = double.PositiveInfinity;
                for (int j = 0; j < currentState.getMap().GetLength(1); j++)
                {
                    if (currentState.getPoint(i, j) < minimum)
                        minimum = currentState.getPoint(i, j);
                }
                if (minimum == 0) //if there is already a 0 in that row, don't waste time looping through it
                    continue;
                if (minimum != double.PositiveInfinity)
                {
                    for (int j = 0; j < currentState.getMap().GetLength(1); j++)
                    {
                        double reducedPoint = currentState.getPoint(i, j) - minimum;
                        currentState.setPoint(i, j, reducedPoint);
                    }
                    lowerBound += minimum;
                }
            }
            for (int j = 0; j < currentState.getMap().GetLength(1); j++) //reduce columns
            {
                double minimum = double.PositiveInfinity;
                for (int i = 0; i < currentState.getMap().GetLength(0); i++)
                {
                    if (currentState.getPoint(i, j) < minimum)
                        minimum = currentState.getPoint(i, j);
                }
                if (minimum == 0) //if there is already a 0 in that column, don't waste time looping through it
                    continue;
                if (minimum != double.PositiveInfinity)
                {
                    for (int i = 0; i < currentState.getMap().GetLength(0); i++)
                    {
                        double lowerPoint = currentState.getPoint(i, j) - minimum;
                        currentState.setPoint(i, j, lowerPoint);
                    }
                    lowerBound += minimum;
                }
            }
            currentState.setLB(lowerBound);
        }
        #endregion
    }

}
/*Route = new ArrayList(Cities.Length);
HashSet<int> unvisitedIndexes = new HashSet<int>(); // using a city's index in Cities, we can interate through indexes that have yet to be added
            for (int index = 0; index<Cities.Length; index++)
            {
                unvisitedIndexes.Add(index);
            }

            print("\n\nTESTING\n");

City city;
            for (int i = 0; i<Cities.Length; i++) // keep trying start nodes until a solution is found
            {
                if (Route.Count == Cities.Length)
                {
                    break; // DONE!
                }
                else
                {
                    Route.Clear();
                    for (int index = 0; index<Cities.Length; index++)
                    {
                        unvisitedIndexes.Add(index);
                    }
                    city = Cities[i];
                }

                for (int n = 0; n<Cities.Length; n++) // add nodes n times
                {

                    double shortestDistance = Double.PositiveInfinity;
int closestIndex = -1;
                    foreach (int check in unvisitedIndexes) //find the closest city to add to route
                    {
                        double distance = city.costToGetTo(Cities[check]);
                        if (distance<shortestDistance)
                        {
                            shortestDistance = distance;
                            closestIndex = check;
                        }
                    }

                    if (closestIndex != -1)
                    {
                        city = Cities[closestIndex];
                        Route.Add(city);
                        unvisitedIndexes.Remove(closestIndex);
                    }
                    else
                    {
                        break; // try again
                    }
                }                
            }

            // call this the best solution so far.  bssf is the route that will be drawn by the Draw method. 
            bssf = new TSPSolution(Route);
// update the cost of the tour. 
Program.MainForm.tbCostOfTour.Text = " " + bssf.costOfRoute();
            // do a refresh. 
            Program.MainForm.Invalidate();*/



/*Console.WriteLine("**************************************************");
        Console.WriteLine("INITIAL STATE");
        outputMap(state);
        Console.WriteLine("**************************************************");
        Console.WriteLine("EXCLUDE STATE");
        outputMap(excludeState);
        Console.WriteLine("**************************************************");
        Console.WriteLine("INCLUDE STATE");
        outputMap(includeState);*/
