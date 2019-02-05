using System;
using DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;

namespace Logic
{
    public static class MakeConnectedGraph
    {
        public static List<List<Vertex>> FindUnconnectedGraph(Graph graph, List<Vertex> pointsVertices)
        {
            Queue<Vertex> ListUnreachedVertices = new Queue<Vertex>();
            List<List<Vertex>> ConnectedParts = new List<List<Vertex>>();
            Dictionary<Vertex, bool> VertexWasInQueue = new Dictionary<Vertex, bool>();

            foreach (Vertex v in graph.Vertices)
                VertexWasInQueue.Add(v, false);

            if (graph.Vertices.Count != 0)
            {
                Vertex first = graph.Vertices.First();
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

        public static void FillUnconnectedGraphs(Graph graph, List<List<Vertex>> ConnectedParts, double weight, double maxDistance)
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
                            int manhattanDistance = Graph.ManhattanDistance(v1.Point, v2.Point);
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
                        ArtificialEdge edge = graph.AddArtificialEdge(vertex1, vertex2, weight, weightedDistance);
                    }
                }
            }
        }

        public static void PrepareGraph(Graph graph, List<Vertex> pointsVertices, double weight, double maxDistance)
        {
            List<List<Vertex>> ConnectedParts = FindUnconnectedGraph(graph, pointsVertices);
            FillUnconnectedGraphs(graph, ConnectedParts, weight, maxDistance);
        }
    }
}