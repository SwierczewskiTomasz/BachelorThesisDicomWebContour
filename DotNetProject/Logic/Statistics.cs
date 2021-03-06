using System;
using DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;

namespace Logic
{
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

        public static int[] HistogramOfContour(int[,] matrixWithContour, int[,] image, int xmin, int xmax, int ymin, int ymax)
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

        public static int[] HistogramOfContourFloodFill(int[,] matrixWithContour, int[,] image, int xmin, int xmax, int ymin, int ymax, Point StartPoint)
        {
            int[] histogram = new int[CannyAlgorithm.numberOfColors];
            Queue<Point> points = new Queue<Point>();
            int[,] checkedMatrix = new int[xmax - xmin, ymax - ymin];

            points.Enqueue(StartPoint);
            checkedMatrix[StartPoint.x, StartPoint.y] = 1;

            while (points.Count > 0)
            {
                Point currentPoint = points.Dequeue();
                if (matrixWithContour[currentPoint.x, currentPoint.y] == 0)
                {
                    histogram[image[currentPoint.x, currentPoint.y]]++;

                    if (currentPoint.x + 1 < xmax)
                    {
                        if (checkedMatrix[currentPoint.x + 1, currentPoint.y] == 0)
                        {
                            if (matrixWithContour[currentPoint.x + 1, currentPoint.y] == 0)
                            {
                                points.Enqueue(new Point(currentPoint.x + 1, currentPoint.y));
                                checkedMatrix[currentPoint.x + 1, currentPoint.y] = 1;
                            }
                            else
                            {
                                currentPoint.ToString();
                            }
                        }
                    }

                    if (currentPoint.x - 1 >= xmin)
                    {
                        if (checkedMatrix[currentPoint.x - 1, currentPoint.y] == 0)
                        {
                            if (matrixWithContour[currentPoint.x - 1, currentPoint.y] == 0)
                            {
                                points.Enqueue(new Point(currentPoint.x - 1, currentPoint.y));
                                checkedMatrix[currentPoint.x - 1, currentPoint.y] = 1;
                            }
                        }
                    }

                    if (currentPoint.y + 1 < ymax)
                    {
                        if (checkedMatrix[currentPoint.x, currentPoint.y + 1] == 0)
                        {
                            if (matrixWithContour[currentPoint.x, currentPoint.y + 1] == 0)
                            {
                                points.Enqueue(new Point(currentPoint.x, currentPoint.y + 1));
                                checkedMatrix[currentPoint.x, currentPoint.y + 1] = 1;
                            }
                        }
                    }

                    if (currentPoint.y - 1 >= ymin)
                    {
                        if (checkedMatrix[currentPoint.x, currentPoint.y - 1] == 0)
                        {
                            if (matrixWithContour[currentPoint.x, currentPoint.y - 1] == 0)
                            {
                                points.Enqueue(new Point(currentPoint.x, currentPoint.y - 1));
                                checkedMatrix[currentPoint.x, currentPoint.y - 1] = 1;
                            }
                        }
                    }
                }
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


        public static StatisticsResult GenerateStatistics(List<Point> pixels, int[,] matrixWithContour,
        int[,] image, int xmin, int xmax, int ymin, int ymax, double pixelAreaInMms,
        double pixelLenghtInMms, Point startPoint)
        {
            StatisticsResult statisticsResult = new StatisticsResult();
            statisticsResult.CenterOfMass = Statistics.Barycentrum(pixels);
            try
            {
                statisticsResult.Histogram = Statistics.HistogramOfContourFloodFill(matrixWithContour, image, xmin, xmax, ymin, ymax, startPoint);
                statisticsResult.HistogramMin = Statistics.MinInHistogram(statisticsResult.Histogram);
                statisticsResult.HistogramMax = Statistics.MaxInHistogram(statisticsResult.Histogram);
                statisticsResult.HistogramMean = Statistics.MeanInHistogram(statisticsResult.Histogram);
                statisticsResult.Area = Statistics.AreaInMms(statisticsResult.Histogram, pixelAreaInMms);
                statisticsResult.NumberOfPixelsInsideContour = Statistics.AreaInPixels(statisticsResult.Histogram);
            }
            catch (Exception e)
            {
                e.ToString();
            }
            // statisticsResult.Permieter = Statistics.PerimeterInMmsSecondMethod(pixels, pixelLenghtInMms);
            statisticsResult.Permieter = Statistics.PerimeterInMms(pixels, pixelLenghtInMms);
            statisticsResult.NumberOfPixelsOfContour = Statistics.PerimeterInPixels(pixels);
            return statisticsResult;
        }

        public static StatisticsResult GenerateStatistics(ManualContourDTO contour)
        {
            System.Drawing.Bitmap bitmap = OrthancConnection.GetBitmapByInstanceId(contour.dicomid);
            int width = bitmap.Width;
            int height = bitmap.Height;
            int[,] matrixWithContour = CannyAlgorithm.MakeMatrixFromPoints(width, height, contour.lines.First().points);

            int count = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (matrixWithContour[x, y] == 0)
                    {
                        count++;
                    }
                }
            }

            int[,] image = CannyAlgorithm.ReadMatrixFromBitmap(bitmap);


            double pixelSizeX = 0;
            double pixelSizeY = 0;

            List<string> splitString = contour.pixelSpacing.Split('\\').ToList();
            List<double> split = new List<double>();
            foreach (var s in splitString)
            {
                double d = 0;
                if (double.TryParse(s, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out d))
                {
                    split.Add(d);
                }
            }
            if (split.Count >= 2)
            {
                pixelSizeX = split[0];
                pixelSizeY = split[1];
            }

            double pixelAreaInMms = pixelSizeX * pixelSizeY;
            double pixelLenghtInMms = pixelSizeX;

            return GenerateStatistics(contour.lines.First().points, matrixWithContour, image, 0, width, 0, height, pixelAreaInMms, pixelLenghtInMms, contour.centralPoints.First());
        }
    }
}