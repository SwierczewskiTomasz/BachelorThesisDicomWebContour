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

        public static SemiAutomaticPreviewDTO Default(SemiAutomaticPreviewDTO points)
        => CannyWithoutStatistics(points);

        public static SemiAutomaticPreviewDTO CannyWithoutStatistics(SemiAutomaticPreviewDTO points)
        {
            List<Point> pixels;

            pixels = CannyAlgorithm.CannyWithoutStatistics(points.dicomid, points.lines.First().points, points.width, points.height);

            List<LinePointsAndPixels> lines = new List<LinePointsAndPixels>();
            LinePointsAndPixels line = new LinePointsAndPixels();
            line.points = new List<Point>(points.lines.First().points);
            line.pixels = new List<Point>(pixels);
            line.brushColor = points.lines.First().brushColor;

            lines.Add(line);

            SemiAutomaticPreviewDTO contour = new SemiAutomaticPreviewDTO(points.guid,
                points.dicomid, points.tag, lines, points.width, points.height, points.pixelSpacing, 1);
            return contour;
        }


        public static SemiAutomaticContourDTO Canny(SemiAutomaticPointsDTO points)
        {
            List<Point> pixels;
            StatisticsResult statisticsResult;

            (pixels, statisticsResult) = CannyAlgorithm.Canny(points.dicomid, points.lines.First().points, points.width, points.height, 
                points.centralPoints, points.pixelSpacing);

            List<LinePointsAndPixels> lines = new List<LinePointsAndPixels>();
            LinePointsAndPixels line = new LinePointsAndPixels();
            line.points = new List<Point>(points.lines.First().points);
            line.pixels = new List<Point>(pixels);
            line.brushColor = points.lines.First().brushColor;

            lines.Add(line);

            SemiAutomaticContourDTO contour = new SemiAutomaticContourDTO(points.guid,
                points.dicomid, points.tag, lines, points.width, points.height, statisticsResult, points.centralPoints, points.pixelSpacing);
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
                int x2 = points.lines.First().points[(i + 1) % count].x;
                int y2 = points.lines.First().points[(i + 1) % count].y;
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

            System.Drawing.Bitmap bitmap = OrthancConnection.GetBitmapByInstanceId(points.dicomid);
            int[,] matrixWithContour = CannyAlgorithm.MakeMatrixFromPoints(bitmap.Width, bitmap.Height, pixels);
            int[,] image = CannyAlgorithm.ReadMatrixFromBitmap(bitmap);

            StatisticsResult statisticsResult = Statistics.GenerateStatistics(pixels, matrixWithContour, image, 0, bitmap.Width, 0, bitmap.Height,
                0, 0, points.centralPoints.First());

            SemiAutomaticContourDTO contour = new SemiAutomaticContourDTO(points.guid,
                points.dicomid, points.tag, lines, points.width, points.height, statisticsResult, points.centralPoints, points.pixelSpacing);
            return contour;
        }
    }
}