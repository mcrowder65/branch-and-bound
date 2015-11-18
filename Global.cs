using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    public class Global
    {
        public static List<int> bestTourSoFar;
        public static Random random = new Random();
        public static int initialTourCost;
        public static Point[] rawData;
        public static int[,] cost;
        public static double[,] heuristic; // Reciprocal of cost
        public static double[,] pheromone;
        public static void Initialize(int n, Graphics g, int[,] c)
        {
            if (c == null)
            {
                cost = new int[n + 1, n + 1]; // natural indexing
                for (int row = 1; row <= n; row++)
                {
                    for (int col = row; col <= n; col++)
                    {
                        if (row == col)
                        {
                            cost[row, col] = 0;
                        }
                        else
                        {
                            double xDist =
                            rawData[row].X - rawData[col].X;
                            double yDist =
                            rawData[row].Y - rawData[col].Y;
                            cost[row, col] =
                            (int)(Math.Sqrt(xDist * xDist +
                            yDist * yDist) + 0.5);
                            cost[col, row] = cost[row, col];
                        }
                    }
                }
            }
            else
            {
                cost = c;
                for (int index = 1; index <= n; index++)
                {
                    cost[index, index] = 0;
                }
            }

        // Obtain shortest city tour using greedy starting at 1
        initialTourCost = 0;
            int numbersCitiesVisited = 1;
            int city = 1;
            List<int> visited = new List<int>();
            visited.Add(0);
            visited.Add(1);
            do
            {
                // Find shortest distance from city
                double[] distances = new double[n + 1];
                for (int index = 1; index <= n; index++)
                {
                    if (index == city || visited.Contains(index))
                    {
                        distances[index] = Int32.MaxValue;
                    }
                    else
                    {
                        distances[index] = Global.cost[city, index];
                    }
                }
                // Find shortest among distances
                int shortestIndex = 0;
                double shortest = Double.MaxValue;
                for (int index = 1; index <= n; index++)
                {
                    if (distances[index] < shortest)
                    {
                        shortestIndex = index;
                        shortest = distances[index];
                    }
                }
                int previousCity = city;
                city = shortestIndex;
                visited.Add(city);
                numbersCitiesVisited++;
                initialTourCost += cost[previousCity, city];
            } while (visited.Count <= n);
            initialTourCost += cost[city, 1];
            visited.Add(1);

            pheromone = new double[n + 1, n + 1]; // natural indexing
            for (int row = 1; row <= n; row++)
            {
                for (int col = 1; col <= n; col++)
                {
                    pheromone[row, col] = 1.0 / (n * initialTourCost);
                }
            }
            heuristic = new double[n + 1, n + 1];
            for (int row = 1; row <= n; row++)
            {
                for (int col = row; col <= n; col++)
                {
                    if (row == col)
                    {
                        heuristic[row, col] = Double.MaxValue;
                    }
                    else
                    {
                        heuristic[row, col] = 1.0 / cost[row, col];
                        heuristic[col, row] = heuristic[row, col];
                    }
                    
                }
            }
        }
    }
} 

