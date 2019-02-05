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
        public Point Point { get; }
        public List<Edge> Edges { get; }
        public Dictionary<Vertex, int> Vertices { get; }

        public Vertex(Point point)
        {
            Point = point;
            Edges = new List<Edge>();
            Vertices = new Dictionary<Vertex, int>();
        }
    }

    public abstract class Edge
    {
        public Vertex Vertex1 { get; }
        public Vertex Vertex2 { get; }

        public Edge(Vertex vertex1, Vertex vertex2)
        {
            Vertex1 = vertex1;
            Vertex2 = vertex2;
        }

        public abstract int GetLenght();
        public abstract List<Point> GetPoints();
    }

    public class ArtificialEdge : Edge
    {
        public double Weight { get; }

        public ArtificialEdge(Vertex vertex1, Vertex vertex2, double weight) : base(vertex1, vertex2)
        {
            Weight = weight;
        }

        public override int GetLenght()
        {
            return (int)(Graph.ManhattanDistance(Vertex1.Point, Vertex2.Point) * Weight);
        }

        public override List<Point> GetPoints()
        {
            return BresenhamClass.Bresenham(new List<Point>(), Vertex1.Point.x, Vertex1.Point.y, Vertex2.Point.x, Vertex2.Point.y);
        }
    }

    public class RealEdge : Edge
    {
        public List<Point> Points { get; }

        public RealEdge(Vertex vertex1, Vertex vertex2) : base(vertex1, vertex2)
        {
            Points = new List<Point>();
        }
        public RealEdge(Vertex vertex1, Vertex vertex2, List<Point> points) : base(vertex1, vertex2)
        {
            Points = new List<Point>(points);
        }

        public override int GetLenght()
        {
            return Points.Count + 1;
        }

        public override List<Point> GetPoints() => Points;
    }

    public class Graph
    {
        public List<Vertex> Vertices { get; }
        public List<Edge> Edges { get; }

        public Graph()
        {
            Vertices = new List<Vertex>();
            Edges = new List<Edge>();
        }

        public Vertex AddVertex(Point point)
        {
            Vertex vertex = new Vertex(point);
            Vertices.Add(vertex);
            return vertex;
        }

        public RealEdge AddRealEdge(Vertex vertex1, Vertex vertex2, List<Point> listOfPointsForEdge)
        {
            RealEdge edge = new RealEdge(vertex1, vertex2, listOfPointsForEdge);

            vertex1.Edges.Add(edge);
            vertex2.Edges.Add(edge);
            Edges.Add(edge);

            vertex1.Vertices.Add(vertex2, edge.GetLenght());
            vertex2.Vertices.Add(vertex1, edge.GetLenght());

            return edge;
        }

        public ArtificialEdge AddArtificialEdge(Vertex vertex1, Vertex vertex2, double weight, int weightedDistance)
        {
            ArtificialEdge edge = new ArtificialEdge(vertex1, vertex2, weight);

            vertex1.Edges.Add(edge);
            vertex2.Edges.Add(edge);

            vertex1.Vertices.Add(vertex2, weightedDistance);
            vertex2.Vertices.Add(vertex1, weightedDistance);

            Edges.Add(edge);

            return edge;
        }

        public static int ManhattanDistance(Point point1, Point point2)
        {
            return Math.Abs(point1.x - point2.x) + Math.Abs(point1.y - point2.y);
        }
    }
}