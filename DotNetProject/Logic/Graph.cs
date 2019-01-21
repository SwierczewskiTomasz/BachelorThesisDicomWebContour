using System;
using DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;

namespace Logic
{
    public class Vertex
    {
        public Point point;
        public List<Edge> Edges;
        //public List<(Vertex, int)> Vertices;
        public Dictionary<Vertex, int> Vertices;
    }

    public class Edge
    {
        public bool Artificial;
        public Vertex vertex1, vertex2;
        public List<Point> points;
        public double weight;
        public int Lenght
        {
            get
            {
                if (Artificial)
                    return (int)(Graph.ManhattanDistance(vertex1.point, vertex2.point) * weight);
                else
                    return points.Count + 1;
            }
        }
    }

    public class Graph
    {
        List<Vertex> Vertices;
        List<Edge> Edges;

        public Dictionary<Vertex, Vertex> AStarAlgotihm(Vertex startVertex, Vertex endVertex, Func<Point, Point, int> heuristicFunction)
        {
            Dictionary<Vertex, int> Distance = new Dictionary<Vertex, int>();
            Dictionary<Vertex, Vertex> Previous = new Dictionary<Vertex, Vertex>();

            HashSet<Vertex> Close = new HashSet<Vertex>();
            Dictionary<Vertex, MySortedListElement> OpenDictionary = new Dictionary<Vertex, MySortedListElement>();
            MySortedList OpenList = new MySortedList();

            Distance.Add(startVertex, 0);
            Previous.Add(startVertex, null);

            MySortedListElement element = OpenList.Add(startVertex, heuristicFunction(startVertex.point, endVertex.point));
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
                            OpenList.Add(currentElement.Key, Distance[w] + heuristicFunction(w.point, endVertex.point));

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

        public List<List<Vertex>> FindUnconnectedGraph(List<Vertex> pointsVertices)
        {
            Queue<Vertex> ListUnreachedVertices = new Queue<Vertex>();
            List<List<Vertex>> ConnectedParts = new List<List<Vertex>>();
            Dictionary<Vertex, bool> VertexWasInQueue = new Dictionary<Vertex, bool>();

            foreach (Vertex v in Vertices)
                VertexWasInQueue.Add(v, false);

            if (Vertices.Count != 0)
            {
                Vertex first = Vertices.First();
                ListUnreachedVertices.Enqueue(first);
                VertexWasInQueue[first] = true;
            }

            List<Vertex> ConnectedPart = new List<Vertex>();

            while (ListUnreachedVertices.Count != 0)
            {
                Vertex v = ListUnreachedVertices.Dequeue();
                ConnectedPart.Add(v);

                foreach (Vertex neighbour in v.Vertices.Keys)
                    if (!VertexWasInQueue[neighbour])
                    {
                        ListUnreachedVertices.Enqueue(neighbour);
                        VertexWasInQueue[neighbour] = true;
                    }

                if (ListUnreachedVertices.Count == 0)
                {
                    if (ConnectedPart.Count > 2)
                        ConnectedParts.Add(ConnectedPart);
                    else
                    {
                        foreach (var ve in pointsVertices)
                        {
                            if (ConnectedPart.Contains(ve))
                                ConnectedParts.Add(ConnectedPart);
                        }
                    }
                    ConnectedPart = new List<Vertex>();
                    Vertex next = null;
                    try
                    {
                        next = VertexWasInQueue.First(k => !k.Value).Key;
                    }
                    catch (Exception e)
                    {
                        e.ToString();
                    }
                    if (next != null)
                    {
                        ListUnreachedVertices.Enqueue(next);
                        VertexWasInQueue[next] = true;
                    }
                }
            }

            return ConnectedParts;
        }

        public void FillUnconnectedGraphs(List<List<Vertex>> ConnectedParts, double weight, double maxDistance)
        {
            for (int i = 0; i < ConnectedParts.Count; i++)
            {
                for (int j = i + 1; j < ConnectedParts.Count; j++)
                {
                    List<Vertex> list1 = ConnectedParts[i];
                    List<Vertex> list2 = ConnectedParts[j];

                    int distance = int.MaxValue;
                    Vertex vertex1 = null;
                    Vertex vertex2 = null;

                    foreach (var v1 in list1)
                    {
                        foreach (var v2 in list2)
                        {
                            int manhattanDistance = ManhattanDistance(v1.point, v2.point);
                            if (manhattanDistance < distance)
                            {
                                distance = manhattanDistance;
                                vertex1 = v1;
                                vertex2 = v2;
                            }
                        }
                    }

                    if (distance < maxDistance)
                    {
                        int weightedDistance = (int)(weight * distance);
                        Edge edge = new Edge();
                        edge.Artificial = true;
                        edge.weight = weight;
                        edge.vertex1 = vertex1;
                        edge.vertex2 = vertex2;

                        vertex1.Edges.Add(edge);
                        vertex2.Edges.Add(edge);

                        vertex1.Vertices.Add(vertex2, weightedDistance);
                        vertex2.Vertices.Add(vertex1, weightedDistance);

                        Edges.Add(edge);
                    }
                }
            }
        }

        public void PrepareGraph(List<Vertex> pointsVertices, double weight, double maxDistance)
        {
            List<List<Vertex>> ConnectedParts = FindUnconnectedGraph(pointsVertices);
            FillUnconnectedGraphs(ConnectedParts, weight, maxDistance);
        }

        public Dictionary<Vertex, Vertex> ModificatedAStarAlgotihm(Vertex startVertex, Vertex endVertex, Func<Point, Point, int> heuristicFunction, double weight)
        {
            //PrepareGraph(startVertex, endVertex, weight);
            Dictionary<Vertex, Vertex> Previous = AStarAlgotihm(startVertex, endVertex, heuristicFunction);
            return Previous;
        }

        public List<Point> FindShortestPath(List<Point> points, double weight)
        {
            List<Point> result = new List<Point>();
            List<Vertex> pointsVertices = new List<Vertex>();

            foreach (var point in points)
            {
                Vertex vertex = new Vertex();
                vertex.point = point;
                vertex.Vertices = new Dictionary<Vertex, int>();
                vertex.Edges = new List<Edge>();
                Vertices.Add(vertex);
                pointsVertices.Add(vertex);
            }

            double maxDistance = 0;
            int ip = 0;
            for (ip = 0; ip < points.Count; ip++)
            {
                Point p1 = points[ip];
                Point p2 = points[(ip + 1) % points.Count];

                double distance = ManhattanDistance(p1, p2);
                if (distance > maxDistance)
                    maxDistance = distance;
            }

            //PrepareGraph(pointsVertices, weight, maxDistance * 2);
            PrepareGraph(pointsVertices, weight, maxDistance * 1.2);

            // foreach (var e in Edges)
            // {
            //     // if (e.Artificial)
            //     //     result.AddRange(BresenhamClass.Bresenham(new List<Point>(), e.vertex1.point.x, e.vertex1.point.y, e.vertex2.point.x, e.vertex2.point.y));
            //     // else
            //     //     result.AddRange(e.points);
            //     if(!e.Artificial)
            //         result.AddRange(e.points);
            //     //result.Add(e.vertex1.point);
            //     //result.Add(e.vertex2.point);
            // }

            for (int i = 0; i < points.Count; i++)
            {
                Vertex startVertex = pointsVertices[i];
                Vertex endVertex = pointsVertices[(i + 1) % points.Count];

                Dictionary<Vertex, Vertex> previous = AStarAlgotihm(startVertex, endVertex, ManhattanDistance);

                Vertex currentVertex = endVertex;

                Vertex exists = previous[currentVertex];
                int count = 0;

                while (currentVertex != startVertex)
                {
                    count++;
                    Vertex previousVertex = previous[currentVertex];
                    Edge e = previousVertex.Edges.First(f => ((f.vertex1 == currentVertex && f.vertex2 == previousVertex) || (f.vertex2 == currentVertex && f.vertex1 == previousVertex)));
                    if (e == null)
                        throw new Exception("Censored - don't searched edge after A* algorithm");

                    if (e.points == null)
                    {
                        result.AddRange(BresenhamClass.Bresenham(new List<Point>(), previousVertex.point.x, previousVertex.point.y, currentVertex.point.x, currentVertex.point.y));
                    }
                    else
                    {
                        result.AddRange(e.points);
                    }

                    currentVertex = previousVertex;
                }
            }

            return result;
        }

        public void AddArtificialEdges(double weight, Vertex startVertex)
        {
            foreach (Vertex v in Vertices)
            {
                if (!v.Vertices.ContainsKey(startVertex))
                {
                    int distance = ManhattanDistance(startVertex.point, v.point);
                    int weightDistance = (int)(weight * distance);
                    v.Vertices.Add(startVertex, weightDistance);
                }
            }
        }

        public static List<Point> FindShortestPath(int[,] matrix, int xmin, int xmax, int ymin, int ymax, double weight, List<Point> points)
        {
            Graph graph = GenerateGraphFromMatrix(matrix, xmin, xmax, ymin, ymax);
            List<Point> result = new List<Point>(graph.FindShortestPath(points, weight));
            return result;
        }

        public static int ManhattanDistance(Point point1, Point point2)
        {
            return Math.Abs(point1.x - point2.x) + Math.Abs(point1.y - point2.y);
        }

        public static Graph GenerateGraphFromMatrix(int[,] matrix, int xmin, int xmax, int ymin, int ymax)
        {
            Graph graph = new Graph();

            graph.Vertices = new List<Vertex>();
            graph.Edges = new List<Edge>();

            int width = xmax - xmin;
            int height = ymax - ymin;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (matrix[x, y] != 0)
                    {
                        int count = CountNeighbours(matrix, width, height, x, y);

                        if (!(count == 1 || count == 3))
                        {
                            Vertex vertex = new Vertex();
                            vertex.point = new Point(x + xmin, y + ymin);
                            vertex.Vertices = new Dictionary<Vertex, int>();
                            vertex.Edges = new List<Edge>();

                            Queue<Vertex> queueVertices = new Queue<Vertex>();
                            queueVertices.Enqueue(vertex);

                            //Cleaning visited points
                            matrix[x, y] = 0;

                            while (queueVertices.Count != 0)
                            {
                                Vertex currentVertex = queueVertices.Dequeue();
                                graph.Vertices.Add(currentVertex);

                                Queue<Edge> queueEdges = new Queue<Edge>();

                                foreach (Point n in Neighbours(width, height, x, y))
                                {
                                    if (matrix[n.x, n.y] != 0)
                                    {
                                        Edge e = new Edge();
                                        e.vertex1 = currentVertex;
                                        e.Artificial = false;
                                        e.points = new List<Point>();
                                        e.points.Add(new Point(n.x + xmin, n.y + ymin));
                                        queueEdges.Enqueue(e);
                                    }
                                }

                                while (queueEdges.Count != 0)
                                {
                                    Edge currentEdge = queueEdges.Dequeue();
                                    //graph.Edges.Add(currentEdge);

                                    Queue<Point> queueOfPotentialPoints = new Queue<Point>();

                                    Point currentPoint = currentEdge.points.First();
                                    currentEdge.points.RemoveAt(0);
                                    queueOfPotentialPoints.Enqueue(currentPoint);

                                    while (queueOfPotentialPoints.Count != 0)
                                    {
                                        Point potentialPoint = queueOfPotentialPoints.Dequeue();

                                        int countPointNeighbours = CountNeighbours(matrix, width, height, potentialPoint.x - xmin, potentialPoint.y - ymin);
                                        if (countPointNeighbours == 0 || countPointNeighbours > 1)
                                        {
                                            //new Vertex!!!
                                            Vertex secondVertex = new Vertex();
                                            secondVertex.point = potentialPoint;
                                            secondVertex.Edges = new List<Edge>();
                                            secondVertex.Edges.Add(currentEdge);
                                            secondVertex.Vertices = new Dictionary<Vertex, int>();

                                            currentEdge.vertex2 = secondVertex;
                                            graph.Edges.Add(currentEdge);

                                            secondVertex.Vertices.Add(currentVertex, currentEdge.Lenght);
                                            currentVertex.Vertices.Add(secondVertex, currentEdge.Lenght);

                                            currentVertex.Edges.Add(currentEdge);
                                            queueVertices.Enqueue(secondVertex);

                                            if (queueOfPotentialPoints.Count != 0)
                                                throw new Exception("Unexpected situation - not all points in list of potential points for edge has been reviewed");
                                            // listOfPotentialPoints.Clear();
                                        }
                                        else if (countPointNeighbours == 1)
                                        {

                                            if (currentEdge.Lenght >= 15)
                                            {
                                                Vertex secondVertex = new Vertex();
                                                secondVertex.point = potentialPoint;
                                                secondVertex.Edges = new List<Edge>();
                                                secondVertex.Edges.Add(currentEdge);
                                                secondVertex.Vertices = new Dictionary<Vertex, int>();

                                                currentEdge.vertex2 = secondVertex;
                                                graph.Edges.Add(currentEdge);

                                                secondVertex.Vertices.Add(currentVertex, currentEdge.Lenght);
                                                currentVertex.Vertices.Add(secondVertex, currentEdge.Lenght);

                                                currentVertex.Edges.Add(currentEdge);
                                                queueVertices.Enqueue(secondVertex);
                                            }
                                            else
                                            {
                                                currentEdge.points.Add(potentialPoint);

                                                foreach (Point p in Neighbours(width, height, potentialPoint.x - xmin, potentialPoint.y - ymin))
                                                {
                                                    if (matrix[p.x, p.y] != 0)
                                                        queueOfPotentialPoints.Enqueue(new Point(p.x + xmin, p.y + ymin));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("This fragment of code shouldn't be reached");
                                        }

                                        //Cleaning visited points
                                        matrix[potentialPoint.x - xmin, potentialPoint.y - ymin] = 0;

                                        if (queueOfPotentialPoints.Count == 2)
                                            queueOfPotentialPoints.Clear();
                                        //throw new Exception("");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return graph;
        }

        public static int CountNeighbours(int[,] matrix, int width, int height, int x, int y)
        {
            int count = 0;

            // Warunek if (i == x || j == y) daje nam łączność 4-krotną a nie 8-krotną

            for (int i = x - 1 < 0 ? 0 : x - 1; i < (x + 2 < width ? x + 2 : width); i++)
                for (int j = y - 1 < 0 ? 0 : y - 1; j < (y + 2 < height ? y + 2 : height); j++)
                    if (i == x || j == y)
                        if (i != x || j != y)
                            if (matrix[i, j] != 0)
                                count++;

            return count;
        }

        public static IEnumerable<Point> Neighbours(int width, int height, int x, int y)
        {
            for (int i = x - 1 < 0 ? 0 : x - 1; i < (x + 2 < width ? x + 2 : width); i++)
                for (int j = y - 1 < 0 ? 0 : y - 1; j < (y + 2 < height ? y + 2 : height); j++)
                    if (i == x || j == y)
                        if (i != x || j != y)
                            yield return new Point(i, j);
        }
    }
}