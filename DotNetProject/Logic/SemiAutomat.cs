using System;
using DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;

namespace Logic
{
    public static class SemiAutomatic
    {
        public static SemiAutomaticContourDTO Default(SemiAutomaticPointsDTO points)
        => Canny(points);
        
        public static SemiAutomaticContourDTO Canny(SemiAutomaticPointsDTO points)
        {
            List<Point> pixels = CannyAlgorithm.Canny(points.dicomid, points.lines.First().points);

            List<LinePointsAndPixels> lines = new List<LinePointsAndPixels>();
            LinePointsAndPixels line = new LinePointsAndPixels();
            line.points = new List<Point>(points.lines.First().points);
            line.pixels = new List<Point>(pixels);
            line.brushColor = points.lines.First().brushColor;

            lines.Add(line);

            SemiAutomaticContourDTO contour = new SemiAutomaticContourDTO(points.guid, 
            points.dicomid, points.tag, lines, points.width, points.height);
            return contour;
        }

        public static SemiAutomaticContourDTO TrivialContour(SemiAutomaticPointsDTO points)
        {
            List<Point> pixels = new List<Point>();
            int count = points.lines.First().points.Count;
            for (int i = 0; i < points.lines.First().points.Count; i++)
            {
                int x1 = points.lines.First().points[i].x;
                int y1 = points.lines.First().points[i].y;
                int x2 = points.lines.First().points[(i + 1)%count].x;
                int y2 = points.lines.First().points[(i + 1)%count].y;
                List<Point> pixelsBresenham = new List<Point>(); 
                BresenhamClass.Bresenham(pixelsBresenham, x1, y1, x2, y2);
                pixels = pixels.Concat(pixelsBresenham).ToList();
            }

            List<LinePointsAndPixels> lines = new List<LinePointsAndPixels>();
            LinePointsAndPixels line = new LinePointsAndPixels();
            line.points = new List<Point>(points.lines.First().points);
            line.pixels = new List<Point>(pixels);
            line.brushColor = points.lines.First().brushColor;

            lines.Add(line);

            SemiAutomaticContourDTO contour = new SemiAutomaticContourDTO(points.guid, 
            points.dicomid, points.tag, lines, points.width, points.height);
            return contour;
        }
    }
}