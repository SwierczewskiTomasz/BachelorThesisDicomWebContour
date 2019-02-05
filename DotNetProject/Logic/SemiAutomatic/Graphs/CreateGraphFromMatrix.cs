using System;
using DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;

namespace Logic
{
    public static class GenerateGraphFromMatrix
    {
        public static Graph Generate(int[,] matrix, int xmin, int xmax, int ymin, int ymax)
        {
            Graph graph = new Graph();

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
                            Vertex vertex = new Vertex(new Point(x + xmin, y + ymin));

                            Queue<Vertex> queueVertices = new Queue<Vertex>();
                            queueVertices.Enqueue(vertex);

                            //Cleaning visited points
                            matrix[x, y] = 0;

                            while (queueVertices.Count != 0)
                            {
                                Vertex currentVertex = queueVertices.Dequeue();
                                graph.Vertices.Add(currentVertex);

                                Queue<Point> queueFirstPointsOfEdges = new Queue<Point>();

                                foreach (Point n in Neighbours(width, height, x, y))
                                {
                                    if (matrix[n.x, n.y] != 0)
                                    {
                                        queueFirstPointsOfEdges.Enqueue(new Point(n.x + xmin, n.y + ymin));
                                    }
                                }

                                while (queueFirstPointsOfEdges.Any())
                                {
                                    Point currentPoint = queueFirstPointsOfEdges.Dequeue();
                                    List<Point> listOfPointsForEdge = new List<Point>();

                                    Queue<Point> queueOfPotentialPoints = new Queue<Point>();

                                    queueOfPotentialPoints.Enqueue(currentPoint);

                                    while (queueOfPotentialPoints.Count != 0)
                                    {
                                        Point potentialPoint = queueOfPotentialPoints.Dequeue();

                                        int countPointNeighbours = CountNeighbours(matrix, width, height, potentialPoint.x - xmin, potentialPoint.y - ymin);
                                        if (countPointNeighbours == 0 || countPointNeighbours > 1)
                                        {
                                            //new Vertex!!!
                                            Vertex secondVertex = new Vertex(potentialPoint);
                                            Edge currentEdge = graph.AddRealEdge(currentVertex, secondVertex, listOfPointsForEdge);
                                            queueVertices.Enqueue(secondVertex);

                                            if (queueOfPotentialPoints.Count != 0)
                                                throw new Exception("Unexpected situation - not all points in list of potential points for edge has been reviewed");
                                        }
                                        else if (countPointNeighbours == 1)
                                        {
                                            if (listOfPointsForEdge.Count >= 15)
                                            {
                                                Vertex secondVertex = new Vertex(potentialPoint);
                                                Edge currentEdge = graph.AddRealEdge(currentVertex, secondVertex, listOfPointsForEdge);
                                                queueVertices.Enqueue(secondVertex);
                                            }
                                            else
                                            {
                                                listOfPointsForEdge.Add(potentialPoint);

                                                foreach (Point p in Neighbours(width, height, potentialPoint.x - xmin, potentialPoint.y - ymin))
                                                {
                                                    if (matrix[p.x, p.y] != 0)
                                                        queueOfPotentialPoints.Enqueue(new Point(p.x + xmin, p.y + ymin));
                                                }
                                            }
                                        }

                                        //Cleaning visited points
                                        matrix[potentialPoint.x - xmin, potentialPoint.y - ymin] = 0;

                                        // if (queueOfPotentialPoints.Count == 2)
                                        //     queueOfPotentialPoints.Clear();
                                        // //throw new Exception("");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return graph;
        }

        private static int CountNeighbours(int[,] matrix, int width, int height, int x, int y)
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

        private static IEnumerable<Point> Neighbours(int width, int height, int x, int y)
        {
            for (int i = x - 1 < 0 ? 0 : x - 1; i < (x + 2 < width ? x + 2 : width); i++)
                for (int j = y - 1 < 0 ? 0 : y - 1; j < (y + 2 < height ? y + 2 : height); j++)
                    if (i == x || j == y)
                        if (i != x || j != y)
                            yield return new Point(i, j);
        }
    }
}