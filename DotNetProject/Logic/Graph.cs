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
                        int count = CountNeighbors(matrix, width, height, x, y);
                        if (count > 0)
                        {
                            List<Point> Vertices = new List<Point>();
                            Vertices.Add(new Point(x, y));
                            while (Vertices.Count != 0)
                            {
                                Vertex vertex1 = new Vertex();
                                vertex1.point = new Point(x, y);
                                while(count > 0)
                                {
                                    
                                }
                            }
                        }
                        else
                        {

                        }
                    }
                }
            }

            return graph;
        }

        public static int CountNeighbors(int[,] matrix, int width, int height, int x, int y)
        {
            int count = 0;

            for (int i = x - 1 < 0 ? 0 : x - 1; i < (x + 1 < width ? x + 1 : width); i++)
                for (int j = y - 1 < 0 ? 0 : y - 1; j < (y + 1 < height ? y + 1 : height); j++)
                    if (i != x || j != y)
                        if (matrix[i, j] != 0)
                            count++;

            return count;
        }
    }
}