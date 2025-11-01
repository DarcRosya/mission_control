using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; 

namespace Travelling
{
    public class CityGraph
    {
        public Dictionary<string, List<(string name, int distance)>> adjacencyList;
        private static List<string> _outputLines;
        
        private CityGraph()
        {
            adjacencyList = [];
            _outputLines = [];
        }
        public static CityGraph LoadFromFile(string filePath)
        {
            CityGraph graph = new CityGraph();
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] parts = line.Split(',');
                string[] cities = parts[0].Split('-');
                string cityA = cities[0];
                string cityB = cities[1];
                
                int distance = int.Parse(parts[1]);

                if (!graph.adjacencyList.ContainsKey(cityA))
                {
                    graph.adjacencyList[cityA] = new List<(string, int)>();
                }
                if (!graph.adjacencyList.ContainsKey(cityB))
                {
                    graph.adjacencyList[cityB] = new List<(string, int)>();
                }

                graph.adjacencyList[cityA].Add((cityB, distance));
                graph.adjacencyList[cityB].Add((cityA, distance));

                _outputLines.Add($"{cityA}-{cityB},{distance}");
                _outputLines.Add($"{cityB}-{cityA},{distance}");
            }
            return graph;
        }

        public List<string> FindShortestPath(string from, string to)
        {
            if (from == to)
            {
                return new List<string> { from };
            }

            var distances = new Dictionary<string, int>();
            var previous = new Dictionary<string, string>();
            var priorityQueue = new PriorityQueue<(string city, int priority), int>();

            foreach (var city in adjacencyList.Keys)
            {
                distances[city] = int.MaxValue;
            }

            if (!distances.ContainsKey(from) || !distances.ContainsKey(to))
            {
                return null;
            }

            distances[from] = 0;
            priorityQueue.Enqueue((from, 0), 0);

            while (priorityQueue.Count > 0)
            {
                var (currentCity, currentPriority) = priorityQueue.Dequeue();

                if (currentPriority > distances[currentCity]) continue;

                if (currentCity == to) break;

                if (adjacencyList.TryGetValue(currentCity, out var neighbors))
                {
                    foreach (var (neighborCity, distanceToNeighbor) in neighbors)
                    {
                        int newDistance = distances[currentCity] + distanceToNeighbor;

                        if (newDistance < distances[neighborCity])
                        {
                            distances[neighborCity] = newDistance;
                            previous[neighborCity] = currentCity;
                            priorityQueue.Enqueue((neighborCity, newDistance), newDistance);
                        }
                    }
                }
            }

            if (!previous.ContainsKey(to))
            {
                return null;
            }

            var path = new List<string>();
            string current = to;
            while (previous.ContainsKey(current))
            {
                path.Add(current);
                current = previous[current];
            }
            path.Add(from);
            path.Reverse();
            return path;
        }

        public int GetPathDistance(List<string> path)
        {
            int totalDistance = 0;

            if (path == null || path.Count < 2)
            {
                return 0;
            }

            for (int i = 0; i < path.Count - 1; i++)
            {
                string currentCity = path[i];
                string nextCity = path[i+1];

                if (adjacencyList.TryGetValue(currentCity, out var neighbors))
                {
                    bool segmentFound = false; 
                    foreach (var neighbor in neighbors)
                    {
                        if (neighbor.name == nextCity)
                        {
                            totalDistance += neighbor.distance;
                            segmentFound = true;
                            break;
                        }
                    }
                    if (!segmentFound)
                    {
                        return -1; 
                    }
                }
                else
                {
                    return -1; 
                }
            }
            return totalDistance;
        }

        public override string ToString()
        {
            return string.Join("\n", _outputLines).TrimEnd('\n');
        }

        public List<string> GetCities()
        {
            return adjacencyList.Keys.ToList();
        }
    }
}