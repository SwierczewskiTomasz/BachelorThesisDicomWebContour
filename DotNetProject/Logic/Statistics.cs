using System;
using DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;

namespace Logic
{
    public class StatisticsResult
    {
        Point CenterOfMass;
        int[] Histogram;
        int HistogramMin;
        int HistogramMax;
        double HistogramMean;
        double Area;
        double Permieter;
        int NumberOfPixelsInsideContour;
        int NumberOfPixelsOfContour;

        public static StatisticsResult GenerateStatistics(List<Point> pixels, int[,] matrixWithContour, 
        int[,] image, int xmin, int xmax, int ymin, int ymax, Point startPoint, double pixelAreaInMms,
        double pixelLenghtInMms)
        {
            StatisticsResult statisticsResult = new StatisticsResult();
            statisticsResult.CenterOfMass = Statistics.Barycentrum(pixels);
            statisticsResult.Histogram = Statistics.HistogramOfContour(matrixWithContour, image, xmin, xmax, ymin, ymax, startPoint);
            statisticsResult.HistogramMin = Statistics.MinInHistogram(statisticsResult.Histogram);
            statisticsResult.HistogramMax = Statistics.MaxInHistogram(statisticsResult.Histogram);
            statisticsResult.HistogramMean = Statistics.MeanInHistogram(statisticsResult.Histogram);
            statisticsResult.Area = Statistics.AreaInMms(statisticsResult.Histogram, pixelAreaInMms);
            statisticsResult.Permieter = Statistics.PerimeterInMmsSecondMethod(pixels, pixelLenghtInMms);   
            statisticsResult.NumberOfPixelsInsideContour = Statistics.AreaInPixels(statisticsResult.Histogram);
            statisticsResult.NumberOfPixelsOfContour = Statistics.PerimeterInPixels(pixels);
            return statisticsResult;
        }

    }

    public static class Statistics
    {
        //Center of mass ;)
        public static Point Barycentrum(List<Point> pixels)
        {
            Point barycentrum;
            int x = 0;
            int y = 0;
            int count = 0;

            foreach (var p in pixels)
            {
                x += p.x;
                y += p.y;
                count++;
            }

            x /= count;
            y /= count;

            barycentrum = new Point(x, y);
            return barycentrum;
        }

        public static int[] HistogramOfContour(int[,] matrixWithContour, int[,] image, int xmin, int xmax, int ymin, int ymax, Point startPoint)
        {
            int[] histogram = new int[CannyAlgorithm.numberOfColors];
            bool inside;
            bool edge;

            for (int x = xmin; x < xmax; x++)
            {
                inside = false;
                edge = false;
                for (int y = ymin; y < ymax; y++)
                {
                    if (matrixWithContour[x, y] != 0)
                    {
                        if (!edge)
                        {
                            inside = !inside;
                            edge = true;
                        }
                    }
                    else if (edge)
                        edge = false;
                    if (inside)
                        histogram[image[x, y]]++;
                }
                if (inside)
                    throw new Exception("An odd number of Edges");
            }

            return histogram;
        }

        public static int AreaInPixels(int[] histogram)
        {
            int count = 0;
            for (int i = 0; i < histogram.Length; i++)
                count += histogram[i];
            return count;
        }

        public static double AreaInMms(int[] histogram, double pixelAreaInMms)
        {
            return pixelAreaInMms * AreaInPixels(histogram);
        }

        public static int PerimeterInPixels(List<Point> pixels)
        {
            return pixels.Count;
        }

        public static double PerimeterInMms(List<Point> pixels, double pixelLenghtInMms)
        {
            return pixels.Count * pixelLenghtInMms;
        }

        public static double PermieterInPixelsSecondMethod(List<Point> pixels)
        {
            double result = 0.0;
            for (int i = 0; i < pixels.Count; i++)
            {
                Point point1 = pixels[i];
                Point point2 = pixels[(i + 5) % pixels.Count];
                double dx = Math.Abs(point1.x - point2.x);
                double dy = Math.Abs(point1.y - point2.y);
                result += Math.Sqrt(dx * dx + dy * dy) / 5.0;
            }

            return result;
        }

        public static double PerimeterInMmsSecondMethod(List<Point> pixels, double pixelLenghtInMms)
        {
            return PermieterInPixelsSecondMethod(pixels) * pixelLenghtInMms;
        }

        public static int MinInHistogram(int[] histogram)
        {
            int i = 0;

            while (histogram[i] == 0)
            {
                if (i == histogram.Length)
                    break;
                i++;
            }

            return i;
        }

        public static int MaxInHistogram(int[] histogram)
        {
            int i = histogram.Length - 1;

            while (histogram[i] == 0)
            {
                i--;
                if (i == 0)
                    break;
            }

            return i;
        }

        public static double MeanInHistogram(int[] histogram)
        {
            double Sum = 0;
            int count = 0;

            for (int i = 0; i < histogram.Length; i++)
            {
                Sum += histogram[i] * i;
                count += histogram[i];
            }

            double Mean = Sum / count;
            return Mean;
        }
    }
}