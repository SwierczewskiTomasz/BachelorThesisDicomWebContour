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
        public List<(Vertex, int)> Vertexes;
    }

    public class Edge
    {
        public Vertex vertex1, vertex2;
        public List<Point> points;
        public int lenght => points.Count;
    }

    public class Graph
    {
        List<Vertex> Vertices;
        List<Edge> Edges;

        public static Graph GenerateGraphFromMatrix(int[,] matrix, int width, int height)
        {
            Graph graph = new Graph();

            graph.Vertices = new List<Vertex>();
            graph.Edges = new List<Edge>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (matrix[x, y] != 0)
                    {
                        int count = CountNeighbours(matrix, width, height, x, y);

                        if (!(count == 0 || count == 3))
                        {
                            Vertex vertex = new Vertex();
                            vertex.point = new Point(x, y);
                            vertex.Vertexes = new List<(Vertex, int)>();

                            List<Vertex> listVertices = new List<Vertex>();
                            listVertices.Add(vertex);

                            while (listVertices.Count != 0)
                            {
                                Vertex currentVertex = listVertices.First();
                                listVertices.RemoveAt(0);

                                List<Edge> listEdges = new List<Edge>();

                                foreach (var n in Neighbours(width, height, x, y))
                                    if (matrix[n.Item1, n.Item2] != 0)
                                    {
                                        Edge e = new Edge();
                                        e.vertex1 = currentVertex;
                                        e.points = new List<Point>();
                                        e.points.Add(new Point(n.Item1, n.Item2));
                                        listEdges.Add(e);
                                    }

                                while (listEdges.Count != 0)
                                {
                                    Edge currentEdge = listEdges.First();
                                    listEdges.RemoveAt(0);

                                    List<Point> listPoints = new List<Point>();

                                    Point currentPoint = currentEdge.points.First();
                                    currentEdge.points.RemoveAt(0);
                                    listPoints.Add(currentPoint);
                                    
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

            for (int i = x - 1 < 0 ? 0 : x - 1; i < (x + 1 < width ? x + 1 : width); i++)
                for (int j = y - 1 < 0 ? 0 : y - 1; j < (y + 1 < height ? y + 1 : height); j++)
                    if (i == x || j == y)
                        //if (i != x || j != y)
                        if (matrix[i, j] != 0)
                            count++;

            return count;
        }

        public static IEnumerable<(int, int)> Neighbours(int width, int height, int x, int y)
        {
            for (int i = x - 1 < 0 ? 0 : x - 1; i < (x + 1 < width ? x + 1 : width); i++)
                for (int j = y - 1 < 0 ? 0 : y - 1; j < (y + 1 < height ? y + 1 : height); j++)
                    if (i == x || j == y)
                        yield return (i, j);
        }
    }
}