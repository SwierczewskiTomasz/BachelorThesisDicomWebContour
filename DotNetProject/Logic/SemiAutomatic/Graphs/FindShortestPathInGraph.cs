using System;
using DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;

namespace Logic
{
    public static class FindShortestPathInGraph
    {
        public static Dictionary<Vertex, Vertex> AStarAlgorithm(Vertex startVertex, Vertex endVertex, Func<Point, Point, int> heuristicFunction)
        {
            Dictionary<Vertex, int> Distance = new Dictionary<Vertex, int>();
            Dictionary<Vertex, Vertex> Previous = new Dictionary<Vertex, Vertex>();

            HashSet<Vertex> Close = new HashSet<Vertex>();
            Dictionary<Vertex, MySortedListElement> OpenDictionary = new Dictionary<Vertex, MySortedListElement>();
            MySortedList OpenList = new MySortedList();

            Distance.Add(startVertex, 0);
            Previous.Add(startVertex, null);

            MySortedListElement element = OpenList.Add(startVertex, heuristicFunction(startVertex.Point, endVertex.Point));
            OpenDictionary.Add(startVertex, element);

            while (OpenDictionary.Count != 0)
            {
                Vertex u;
                int uValue;
                (u, uValue) = OpenList.First();
                OpenDictionary.Remove(u);
                Close.Add(u);

                if (u == endVertex)
                    break;

                foreach (Vertex w in u.Vertices.Keys)
                {
                    int weight = u.Vertices[w];
                    if (!Close.Contains(w))
                    {
                        if (!OpenDictionary.ContainsKey(w))
                        {
                            MySortedListElement currentElement = OpenList.Add(w, weight + Distance[u]);
                            OpenDictionary.Add(w, currentElement);
                            //Distance.Add(w, weight + Distance[u]);
                            Distance.Add(w, int.MaxValue);
                        }
                        if (Distance[w] > Distance[u] + weight)
                        {
                            Distance[w] = Distance[u] + weight;

                            MySortedListElement currentElement = OpenDictionary[w];
                            OpenList.Remove(currentElement);
                            OpenList.Add(currentElement.Key, Distance[w] + heuristicFunction(w.Point, endVertex.Point));

                            if (Previous.ContainsKey(w))
                                Previous[w] = u;
                            else
                                Previous.Add(w, u);
                        }
                    }
                }
            }

            return Previous;
        }

        public static List<Point> FindShortestPath(Graph graph, List<Point> points, double weight)
        {
            List<Point> result = new List<Point>();
            List<Vertex> pointsVertices = AddStartPointsToGraph(graph, points);
            
            double maxDistance = FindMaxDistanceBetweenPoints(points);
            MakeConnectedGraph.PrepareGraph(graph, pointsVertices, weight, maxDistance * 1.2);

            for (int i = 0; i < points.Count; i++)
            {
                Vertex startVertex = pointsVertices[i];
                Vertex endVertex = pointsVertices[(i + 1) % points.Count];

                Dictionary<Vertex, Vertex> previous = AStarAlgorithm(startVertex, endVertex, Graph.ManhattanDistance);

                Vertex currentVertex = endVertex;
                result.Add(currentVertex.Point);

                Vertex exists = previous[currentVertex];
                int count = 0;

                while (currentVertex != startVertex)
                {
                    count++;
                    Vertex previousVertex = previous[currentVertex];
                    Edge e = previousVertex.Edges.First(f => ((f.Vertex1 == currentVertex && f.Vertex2 == previousVertex) || (f.Vertex2 == currentVertex && f.Vertex1 == previousVertex)));
                    if (e == null)
                        throw new Exception("Censored - don't searched edge after A* algorithm");

                    result.AddRange(e.GetPoints());
                    result.Add(currentVertex.Point);
                    currentVertex = previousVertex;
                }
            }

            return result;
        }

        public static List<Vertex> AddStartPointsToGraph(Graph graph, List<Point> points)
        {
            List<Vertex> pointsVertices = new List<Vertex>();

            foreach (var point in points)
            {
                Vertex vertex = graph.AddVertex(point);
                pointsVertices.Add(vertex);
            }

            return pointsVertices;
        }

        public static double FindMaxDistanceBetweenPoints(List<Point> points)
        {
            double maxDistance = 0;
            int i = 0;
            for (i = 0; i < points.Count; i++)
            {
                Point p1 = points[i];
                Point p2 = points[(i + 1) % points.Count];

                double distance = Graph.ManhattanDistance(p1, p2);
                if (distance > maxDistance)
                    maxDistance = distance;
            }

            return maxDistance;
        }

        public static List<Point> FindShortestPath(int[,] matrix, int xmin, int xmax, int ymin, int ymax, double weight, List<Point> points)
        {
            Graph graph = GenerateGraphFromMatrix.Generate(matrix, xmin, xmax, ymin, ymax);
            List<Point> result = new List<Point>(FindShortestPath(graph, points, weight));
            return result;
        }
    }
}
