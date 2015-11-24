using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using C5;
using System.Diagnostics;
using System.Data;
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

        double BSSF; // Best solution so far
        int bssfUpdates = 0; //keeps track of how many times the BSSF has been updated
        int totalNumberStates = 1; //keeps track of how many states have been made, initialize to 1 bc of initial state
        int totalPrunes = 0; //keeps track of total prunes
        IntervalHeap<State> queue; //global queue
        State bestState; //global bestState
        Stopwatch timer; //global timer
        public void solveProblem()
        {
            timer = new Stopwatch();
            timer.Start();
            
            for (int i = 0; i < Cities.Length; i++) //O(n^2), initialize initialStates map
            {
                for(int x = 0; x < Cities.Length; x++)
                {
                    initialState.setPoint(i, x, Cities[i].costToGetTo(Cities[x]));
                    if (initialState.getPoint(i, x) == 0)
                        initialState.setPoint(i, x, double.PositiveInfinity);
                }
            }

            initialState.initializeEdges(); //initializeEdges initializes the state's dictionary
                                            //with key 0 to n(# of cities), value -> -1
            reduceMatrix(initialState); //reduce the matrix to find lower bound

            queue = new IntervalHeap<State>(); //this is a global queue
            BSSF = greedy(); //find initial best solution so far

            findGreatestDiff(initialState); //exlude minus include, O(n^4)
            TimeSpan ts = timer.Elapsed;
            bool terminatedEarly = false; //boolean to keep track if we stopped after 30 seconds
            int maxStates = 0; //keeps track of the maximum amount of states in the queue at one point
            int totalSeconds = 0; 
            while (queue.Count != 0) //some complexity
            {
                if (queue.Count > maxStates) //if maxStates needs to be updated, update it
                    maxStates = queue.Count; 
                ts = timer.Elapsed;
                if (ts.Seconds == 30)
                {
                    terminatedEarly = true;
                    break;
                }
                State min = queue.DeleteMin();
                if (totalSeconds < ts.Seconds)
                {
                    totalSeconds = ts.Seconds;
                    Console.WriteLine("seconds: " + totalSeconds);
                }

                if (min.getLB() < BSSF)   //if the min popped off the queue has a lowerbound less than
                    findGreatestDiff(min);//the BSSF, expand it, otherwise, prune it.
                else
                    break;
            }
            totalPrunes++;
            if (!terminatedEarly)//if it solved the problem in less than 30 seconds
            { 
                int city = 0;
                for (int i = 0; i < bestState.getEdges().Count; i++) 
                {
                    if (i == 0) //outputting a map into the Route list
                    {
                        Route.Add(Cities[i]);
                        city = bestState.getEdge(i);
                    }
                    else
                    {
                        Route.Add(Cities[city]);
                        city = bestState.getEdge(city);
                    }
                }
                bssf = new TSPSolution(Route);
                // update the cost of the tour. 
                Program.MainForm.tbCostOfTour.Text = " " + bssf.costOfRoute();
                Program.MainForm.tbElapsedTime.Text = ts.TotalSeconds.ToString();
                // do a refresh. 
                Program.MainForm.Invalidate();
            }
            Console.WriteLine("**************************************************");
            Console.WriteLine("**************************************************");
            Console.WriteLine("**************************************************");
            Console.WriteLine("# Cities: " + Cities.Length);
            Console.WriteLine("Seed: " + Seed);
            Console.WriteLine("Time elapsed: " + ts.Seconds + "." + ts.Milliseconds);
            Console.WriteLine("Best tour found: " + BSSF);
            Console.WriteLine("Max states: " + maxStates);
            Console.WriteLine("BSSF Updates: " + bssfUpdates);
            Console.WriteLine("# of states created: " + totalNumberStates);
            Console.WriteLine("# of prunes: " + totalPrunes);
          //  System.Windows.Forms.Application.Exit();
        }
        public void findGreatestDiff(State state)//O(n^4)
        {
            int chosenX = 0;
            int chosenY = 0;
            double greatestDiff = double.NegativeInfinity;
            for (int i = 0; i < state.getMap().GetLength(0); i++) //rows, O(n^4)
            {
                for (int j = 0; j < state.getMap().GetLength(1); j++) //columns
                {
                    if (state.getPoint(i, j) == 0) //if point is 0
                    {
                        int possibleMax = findExcludeMinusInclude(i, j, state); //O(n^2)
                        if (possibleMax >= greatestDiff) //set the point to point values, if it is 0 is covered by =
                        {
                            chosenX = i;
                            chosenY = j;
                            greatestDiff = possibleMax;
                        }
                    }
                }
            }
            State include = makeInclude(chosenX, chosenY, state); //O(n^2)
            if (BSSF > include.getLB())
            {
                include.setEdge(chosenX, chosenY);
                checkCycles(include, chosenX, chosenY); //O(n)
                queue.Add(include);
            }
            else
                totalPrunes++;

            if (isDictionaryFilled(include))//O(n)
            {
                BSSF = include.getLB();
                bssfUpdates++;
                bestState = include;
            }

            State exclude = makeExclude(chosenX, chosenY, state);//O(n^2)
            if (BSSF > exclude.getLB())
                queue.Add(exclude);
            else
                totalPrunes++;
            if (isDictionaryFilled(exclude))//O(n)
            {
                BSSF = exclude.getLB();
                bestState = exclude;
                bssfUpdates++;
            }

            //Console.WriteLine(exclude.getLB() + " " + include.getLB());
        }
        public void checkCycles(State state, int i, int j) //O(n)
        {
            Dictionary<int, int> edges = new Dictionary<int, int>(state.getEdges());
            if (getDictionarySize(edges) == edges.Count)
                return;
            int[] entered = new int[edges.Count];
            int[] exited = new int[edges.Count];
            for (int x = 0; x < entered.Length; x++) //O(n)
                entered[x] = -1;
            for (int x = 0; x < edges.Count; x++) //O(n)
            {
                exited[x] = edges[x];
                if (exited[x] != -1)
                    entered[exited[x]] = x;
            }
            
            entered[j] = i;
            exited[i] = j;

            int start = i;
            int end = j;
            while(exited[end] != -1) //O(n)
                end = exited[end];
            while (entered[start] != -1)//O(n)
                start = entered[start];
            if (getDictionarySize(edges) != edges.Count - 1)
            {
                while (start != j) //O(n)
                {
                    state.setPoint(end, start, double.PositiveInfinity);
                    state.setPoint(j, start, double.PositiveInfinity);
                    start = exited[start];
                }
            }
        }
        public int getDictionarySize(Dictionary<int, int> dictionary) //O(n)
        {
            int size = 0;
            for(int i = 0; i < dictionary.Count; i++)
            {
                if (dictionary[i] != -1)
                    size++;
            }
            return size;
        }
        public bool isDictionaryFilled(State state)//O(n)
        {
            for(int i = 0; i < state.getEdges().Count; i++)
            {
                if (state.getEdge(i) == -1)
                    return false;
            }
            return true;
        }
        public int findExcludeMinusInclude(int x, int y, State state)//O(n^2)
        {
            State excludeState = makeExclude(x, y, state); //O(n^2)
            reduceMatrix(excludeState);//O(n^2)

            State includeState = makeInclude(x, y, state); //O(n^2)
            reduceMatrix(includeState); //O(n^2)
            
            int cost = Convert.ToInt32(excludeState.getLB() - includeState.getLB());
            return cost;
        }
        public State makeInclude(int x, int y, State state) //O(n^2)
        {
            State includeState = new State(copyMatrix(state.getMap()), state.getLB(), state.getEdges()); //O(n^2)
            totalNumberStates++;
            includeState.setPoint(y, x, double.PositiveInfinity);
            for (int j = 0; j < includeState.getMap().GetLength(1); j++) //set the row to infinity, O(n)
            {
                includeState.setPoint(x, j, double.PositiveInfinity);
            }
            for (int i = 0; i < includeState.getMap().GetLength(0); i++) //set the column to infinity, O(n)
            {
                includeState.setPoint(i, y, double.PositiveInfinity);
            }
            reduceMatrix(includeState); //O(n^2)
            includeState.setEdges(state.getEdges());
            return includeState;
        }
        public State makeExclude(int x, int y, State state) //O(n^2)
        {
            State excludeState = new State(copyMatrix(state.getMap()), state.getLB(), state.getEdges()); //O(n^2)
            totalNumberStates++;
            excludeState.setPoint(x, y, double.PositiveInfinity);
            reduceMatrix(excludeState); //O(n^2)
            excludeState.setEdges(state.getEdges());
            return excludeState;
        }
        public double[,] copyMatrix(double[,] initialMatrix) //O(n^2)
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
        public double greedy() //O(n^2)
        {
            double BSSF = 0;
            ArrayList correctRoute = new ArrayList();
            List<int> visitedCities = new List<int>();
            int firstCity = -1;
            int iterations = 0;
            for (int city = 0; city < Cities.Length; city++) //O(n^2)
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
                        if (i == city - 1)
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
            //Program.MainForm.Invalidate();
            Route.Clear();
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
        public void reduceMatrix(State currentState) //O(n^2)
        {
            double lowerBound = currentState.getLB() ;
            for (int i = 0; i < currentState.getMap().GetLength(0); i++) // reduce rows
            {//O(n^2)
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
            {//O(n^2)
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