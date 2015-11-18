using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    public class Ant
    {
        // Constants
        private const int ALPHA = 1;
        private const int BETA = 5;
        private const double LOCAL_PHEROMONE_UPDATE = 0.1;
        // Fields
        private List<int> citiesVisited;
        private int startCity;
        private int numberCities;
        private int tourCost;
        private double probabilityThreshold;
        // Constructor
        public Ant(int startingCity, int numberCities, double
        probabilityThreshold)
        {
            startCity = startingCity;
            this.numberCities = numberCities;
            this.probabilityThreshold = probabilityThreshold;
            citiesVisited = new List<int>();
            tourCost = 0;
        }
        // Commands
        public void ConstructTour()
        {
            citiesVisited.Add(0); // for natural indexing
            citiesVisited.Add(startCity);
            int previousCity = startCity;
            do
            {
                int nextCity = AddEdgeFrom(previousCity);
                if (!citiesVisited.Contains(nextCity))
                {
                    citiesVisited.Add(nextCity);
                }
                tourCost += Global.cost[previousCity, nextCity];
            // Ant colony system local trail update
            /*(1.0 - LOCAL_PHEROMONE_UPDATE) *
            Global.pheromone[previousCity, nextCity] +
            LOCAL_PHEROMONE_UPDATE *
            (1.0 / (numberCities * Global.initialTourCost));*/
                previousCity = nextCity;

            } while (citiesVisited.Count <= numberCities);
            tourCost += Global.cost[previousCity, startCity];
            citiesVisited.Add(startCity);
            if (citiesVisited.Count != numberCities + 2)
            {
                Console.WriteLine("ERROR IN CONSTRUCTING TOUR");
            }
        }
        // Queries
        public int AddEdgeFrom(int city)
        {
            // Based on modified Ant Colony System heuristic
            double r = Global.random.NextDouble();
            if (r <= probabilityThreshold)
            {
                double[] arcWeights = new double[numberCities + 1];
                for (int index = 1; index <= numberCities; index++)
                {
                    if (index == city ||
                    citiesVisited.Contains(index))
                    {
                        arcWeights[index] = 0.0;
                    }
                    else
                    {
                        arcWeights[index] = Global.pheromone[city,
                        index] *
                        Math.Pow(Global.heuristic[city, index], BETA);
                    }
                }
                // Get the largest in arcWeights
                double largest = -1.0;
                int largestIndex = 0;
                for (int index = 1; index <= numberCities; index++)
                {
                    if (arcWeights[index] > largest)
                    {
                        largest = arcWeights[index];
                        largestIndex = index;
                    }
                }
                if (arcWeights[largestIndex] == 0.0)
                {
                    // Return the first city not yet visited
                    for (int index = 1; index <= numberCities; index++)
                    {
                        if (!citiesVisited.Contains(index))
                        {
                            return index;
                        }
                    }
                }
                else
                {
                    return largestIndex;
              }
            }
            else
            { // Same as Ant System heuristic
                double denominator = 0.0;
                for (int index = 1; index <= numberCities; index++)
                {
                    if (index != city &&
                    !citiesVisited.Contains(index))
                    {
                        denominator += Global.pheromone[city, index] *
                        Math.Pow(Global.heuristic[city, index],
                        BETA);
                    }
                }
                if (denominator == 0.0)
                {
                    // Return the first city not yet visited
                    for (int index = 1;
                    index <= numberCities; index++)
                    {
                        if (!citiesVisited.Contains(index))
                        {
                            return index;
                        }
                    }
                }
                // prob of going from city to index
                double[] prob = new double[numberCities + 1];
                for (int index = 1; index <= numberCities; index++)
                {
                    if (index == city ||
                    citiesVisited.Contains(index))
                    {
                        prob[index] = 0.0;
                    }
                    else
                    {
                        prob[index] = Global.pheromone[city, index] *
                        Math.Pow(Global.heuristic[city, index],
                        BETA) / denominator;
                    }
                }
                double rnd = Global.random.NextDouble();
                double sum = 0.0;
                for (int index = 1; index <= numberCities; index++)
                {
                    sum += prob[index];
                    if (rnd <= sum && index != city &&
                    !citiesVisited.Contains(index))
                    {
                        return index;
                    }
                }
            }
            // Unreachable
            return 0;
        }
        // Properties
        public int TourCost
        {
            get
            { // Read-only
                return tourCost;
            }
        }
 public List<int> CitiesVisited
        {
            get
            {
                return citiesVisited;
            }
        }
    }
} 

