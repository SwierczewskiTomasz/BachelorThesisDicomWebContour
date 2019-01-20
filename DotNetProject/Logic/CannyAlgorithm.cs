using System;
using DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;

namespace Logic
{
    public static class CannyAlgorithm
    {
        public static int numberOfColors = 256;

        public static List<Point> CannyWithoutStatistics(string dicomId, List<Point> points, int canvasWidth, int canvasHeight)
        {
            System.Drawing.Bitmap bitmap = OrthancConnection.GetBitmapByInstanceId(dicomId);
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
            System.Drawing.Imaging.BitmapData bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            double[,][] sobel;
            int width, height;
            width = bitmap.Width;
            height = bitmap.Height;

            int xmin, xmax, ymin, ymax;
            (xmin, xmax, ymin, ymax) = FindPointsMinMaxPositions(points, width, height);

            sobel = SobelOperator(rgbValues, xmin, xmax, ymin, ymax, bmpData.Stride);
            bitmap.UnlockBits(bmpData);

            double[,][] gradient = FindIntensityGradient(sobel, xmin, xmax, ymin, ymax);
            double[,] edges = NonMaximumSuppression(gradient, xmin, xmax, ymin, ymax);
            int[] distribution = DistributionFunction(edges, xmin, xmax, ymin, ymax);
            distribution = CumulativeDistributionFunction(distribution);

            int min, max;
            double lowerThreshold = 0.70, higherThreshold = 0.95;

            (min, max) = ChooseThreshold(distribution, lowerThreshold, higherThreshold);
            int[,] foundedEdges = HysteresisThreshold(edges, xmin, xmax, ymin, ymax, min, max);

            int[,] foundedEdges2 = Make4ConnectedMatrix(foundedEdges, xmin, xmax, ymin, ymax);

            double weight = 2.5;
            List<Point> pixels = new List<Point>(Graph.FindShortestPath(foundedEdges2, xmin, xmax, ymin, ymax, weight, points));

            return pixels;
        }

        public static (List<Point>, StatisticsResult) Canny(string dicomId, List<Point> points, int canvasWidth, int canvasHeight,
            List<Point> centralPoints, string pixelSpacing)
        {
            System.Drawing.Bitmap bitmap = OrthancConnection.GetBitmapByInstanceId(dicomId);
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
            System.Drawing.Imaging.BitmapData bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            double[,][] sobel;
            int width, height;
            width = bitmap.Width;
            height = bitmap.Height;

            int xmin, xmax, ymin, ymax;
            (xmin, xmax, ymin, ymax) = FindPointsMinMaxPositions(points, width, height);

            sobel = SobelOperator(rgbValues, xmin, xmax, ymin, ymax, bmpData.Stride);
            bitmap.UnlockBits(bmpData);

            double[,][] gradient = FindIntensityGradient(sobel, xmin, xmax, ymin, ymax);
            double[,] edges = NonMaximumSuppression(gradient, xmin, xmax, ymin, ymax);
            int[] distribution = DistributionFunction(edges, xmin, xmax, ymin, ymax);
            distribution = CumulativeDistributionFunction(distribution);

            int min, max;
            double lowerThreshold = 0.70, higherThreshold = 0.95;

            (min, max) = ChooseThreshold(distribution, lowerThreshold, higherThreshold);
            int[,] foundedEdges = HysteresisThreshold(edges, xmin, xmax, ymin, ymax, min, max);

            int[,] foundedEdges2 = Make4ConnectedMatrix(foundedEdges, xmin, xmax, ymin, ymax);

            double weight = 2.5;
            List<Point> pixels = new List<Point>(Graph.FindShortestPath(foundedEdges2, xmin, xmax, ymin, ymax, weight, points));

            int[,] matrixWithContour = MakeMatrixFromPoints(width, height, pixels);
            int[,] image = ReadMatrixFromBitmap(bitmap);

            StatisticsResult statisticsResult = null;

            double pixelSizeX = 0;
            double pixelSizeY = 0;

            List<string> splitString = pixelSpacing.Split('\\').ToList();
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

            statisticsResult = Statistics.GenerateStatistics(pixels, matrixWithContour, image, 0, width, 0, height, pixelAreaInMms, pixelLenghtInMms, centralPoints.First());

            return (pixels, statisticsResult);
        }

        public static (int, int, int, int) FindPointsMinMaxPositions(List<Point> points, int bitmapWidth, int bitmapHeight)
        {
            int xmin = int.MaxValue;
            int xmax = int.MinValue;
            int ymin = int.MaxValue;
            int ymax = int.MinValue;

            foreach (var p in points)
            {
                if (p.x < xmin)
                {
                    xmin = p.x;
                }
                if (p.x > xmax)
                {
                    xmax = p.x;
                }
                if (p.y < ymin)
                {
                    ymin = p.y;
                }
                if (p.y > ymax)
                {
                    ymax = p.y;
                }
            }

            int width = xmax - xmin;
            int height = ymax - ymin;

            xmin -= width / 2;
            xmax += width / 2;
            ymin -= height / 2;
            ymax += height / 2;

            if (xmin < 1)
            {
                xmin = 1;
            }
            if (xmax > bitmapWidth - 1)
            {
                xmax = bitmapWidth - 1;
            }
            if (ymin < 1)
            {
                ymin = 1;
            }
            if (ymax > bitmapHeight - 1)
            {
                ymax = bitmapHeight - 1;
            }


            return (xmin, xmax, ymin, ymax);
        }

        public static double[,][] SobelOperator(System.Drawing.Bitmap bitmap, int xmin, int xmax, int ymin, int ymax)
        {
            double[,][] result = new double[xmax - xmin, ymax - ymin][];

            for (int x = xmin; x < xmax; x++)
            {
                for (int y = ymin; y < ymax; y++)
                {
                    double s1, s2;

                    s1 = bitmap.GetPixel(x - 1, y + 1).R + 2 * bitmap.GetPixel(x, y + 1).R + bitmap.GetPixel(x + 1, y + 1).R;
                    s1 -= bitmap.GetPixel(x - 1, y - 1).R + 2 * bitmap.GetPixel(x, y - 1).R + bitmap.GetPixel(x + 1, y - 1).R;

                    s2 = bitmap.GetPixel(x + 1, y - 1).R + 2 * bitmap.GetPixel(x + 1, y).R + bitmap.GetPixel(x + 1, y + 1).R;
                    s2 -= bitmap.GetPixel(x - 1, y - 1).R + 2 * bitmap.GetPixel(x - 1, y).R + bitmap.GetPixel(x - 1, y + 1).R;

                    result[x - xmin, y - ymin] = new double[2] { s1, s2 };
                }
            }

            return result;
        }

        public static double[,][] SobelOperator(byte[] rgbValues, int xmin, int xmax, int ymin, int ymax, int stride)
        {
            double[,][] result = new double[xmax - xmin, ymax - ymin][];

            for (int x = xmin; x < xmax; x++)
            {
                for (int y = ymin; y < ymax; y++)
                {
                    double s1, s2;

                    s1 = GetPixelFromArray(rgbValues, x - 1, y + 1, stride) + 2 * GetPixelFromArray(rgbValues, x, y + 1, stride)
                        + GetPixelFromArray(rgbValues, x + 1, y + 1, stride);
                    s1 -= GetPixelFromArray(rgbValues, x - 1, y - 1, stride) + 2 * GetPixelFromArray(rgbValues, x, y - 1, stride)
                        + GetPixelFromArray(rgbValues, x + 1, y - 1, stride);

                    s2 = GetPixelFromArray(rgbValues, x + 1, y - 1, stride) + 2 * GetPixelFromArray(rgbValues, x + 1, y, stride)
                        + GetPixelFromArray(rgbValues, x + 1, y + 1, stride);
                    s2 -= GetPixelFromArray(rgbValues, x - 1, y - 1, stride) + 2 * GetPixelFromArray(rgbValues, x - 1, y, stride)
                        + GetPixelFromArray(rgbValues, x - 1, y + 1, stride);

                    result[x - xmin, y - ymin] = new double[2] { s1, s2 };
                }
            }

            return result;
        }

        public static byte GetPixelFromArray(byte[] rgbValues, int x, int y, int stride)
        {
            return rgbValues[y * stride + 4 * x + 1];
        }

        public static double[,][] FindIntensityGradient(double[,][] sobel, int xmin, int xmax, int ymin, int ymax)
        {
            double[,][] result = (double[,][])sobel.Clone();

            for (int x = 0; x < xmax - xmin; x++)
            {
                for (int y = 0; y < ymax - ymin; y++)
                {
                    double s1 = sobel[x, y][0];
                    double s2 = sobel[x, y][1];
                    double a = Math.Sqrt(s1 * s1 + s2 * s2);
                    double d = Math.PI / 2;
                    if (s2 != 0)
                        d = Math.Atan(s1 / s2);
                    result[x, y] = new double[] { a, d };
                }
            }

            return result;
        }

        public static double[,] NonMaximumSuppression(double[,][] gradient, int xmin, int xmax, int ymin, int ymax)
        {
            double[,] result = new double[xmax - xmin, ymax - ymin];

            for (int x = 1; x < xmax - xmin - 1; x++)
            {
                for (int y = 1; y < ymax - ymin - 1; y++)
                {
                    result[x, y] = gradient[x, y][0];

                    if (Math.Abs(gradient[x, y][1]) < Math.PI / 8)
                    {
                        if (gradient[x, y][0] < gradient[x + 1, y][0] || gradient[x, y][0] < gradient[x - 1, y][0])
                            result[x, y] = 0;
                    }
                    else if (Math.Abs(gradient[x, y][1]) > Math.PI * 3 / 8)
                    {
                        if (gradient[x, y][0] < gradient[x, y + 1][0] || gradient[x, y][0] < gradient[x, y - 1][0])
                            result[x, y] = 0;
                    }
                    else if (gradient[x, y][1] > 0)
                    {
                        if (gradient[x, y][0] < gradient[x + 1, y + 1][0] || gradient[x, y][0] < gradient[x - 1, y - 1][0])
                            result[x, y] = 0;
                        continue;
                    }
                    else if (gradient[x, y][1] < 0)
                    {
                        if (gradient[x, y][0] < gradient[x + 1, y - 1][0] || gradient[x, y][0] < gradient[x - 1, y + 1][0])
                            result[x, y] = 0;
                    }
                    else
                        throw new Exception("Program shouldn't reach this piece of code");
                }
            }

            return result;
        }

        public static int[] DistributionFunction(double[,] edges, int xmin, int xmax, int ymin, int ymax)
        {
            int[] result = new int[65536];
            for (int x = 0; x < xmax - xmin; x++)
                for (int y = 0; y < ymax - ymin; y++)
                    result[(int)edges[x, y] < 65536 ? (int)edges[x, y] : 65535]++;
            return result;
        }

        public static int[] CumulativeDistributionFunction(int[] distribution)
        {
            for (int i = 1; i < 65536; i++)
                distribution[i] += distribution[i - 1];
            return distribution;
        }

        public static (int, int) ChooseThreshold(int[] cumulativeDistribution, double lowerThreshold, double higherThreshold)
        {
            int min = 0;
            int max = 0;

            for (int i = 0; i < 65536; i++)
            {
                if (cumulativeDistribution[i] < cumulativeDistribution[65535] * lowerThreshold)
                    min = i;
                if (cumulativeDistribution[i] < cumulativeDistribution[65535] * higherThreshold)
                    max = i;
            }

            return (min, max);
        }

        public static int[,] HysteresisThreshold(double[,] edges, int xmin, int xmax, int ymin, int ymax, int min, int max)
        {
            int width = xmax - xmin;
            int height = ymax - ymin;
            int[,] result = new int[width, height];
            Queue<(int, int)> queue = new Queue<(int, int)>();

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (edges[x, y] > max)
                        queue.Enqueue((x, y));

            while (queue.Count != 0)
            {
                int x, y;
                (x, y) = queue.Dequeue();

                if (edges[x, y] > max)
                {
                    result[x, y] = 1;

                    for (int i = x - 1 < 0 ? 0 : x - 1; i < (x + 1 < width ? x + 1 : width); i++)
                        for (int j = y - 1 < 0 ? 0 : y - 1; y < (y + 1 < height ? y + 1 : height); y++)
                            if (i != x && j != y && result[i, j] == 0)
                            {
                                queue.Enqueue((i, j));
                                result[i, j] = 2;
                            }

                }
                else if (edges[x, y] > min && result[x, y] == 2)
                {
                    result[x, y] = 1;

                    for (int i = x - 1 < 0 ? 0 : x - 1; i < (x + 1 < width ? x + 1 : width); i++)
                        for (int j = y - 1 < 0 ? 0 : y - 1; y < (y + 1 < height ? y + 1 : height); y++)
                            if (i != x && j != y && result[i, j] == 0)
                            {
                                queue.Enqueue((i, j));
                                result[i, j] = 2;
                            }
                }
                else
                    result[x, y] = 0;
            }

            return result;
        }

        public static int[,] Make4ConnectedMatrix(int[,] matrix, int xmin, int xmax, int ymin, int ymax)
        {
            int added = 0;
            for (int i = 1; i < xmax - xmin - 1; i++)
            {
                for (int j = 1; j < ymax - ymin - 1; j++)
                {
                    if (matrix[i, j] != 0 && matrix[i + 1, j + 1] != 0 && matrix[i + 1, j] == 0 && matrix[i, j + 1] == 0)
                    {
                        matrix[i + 1, j] = 1;
                        added++;
                    }
                    if (matrix[i, j] == 0 && matrix[i + 1, j + 1] == 0 && matrix[i + 1, j] != 0 && matrix[i, j + 1] != 0)
                    {
                        matrix[i, j] = 1;
                        added++;
                    }
                }
            }
            return matrix;
        }

        public static int[,] FindEndsOfEdges(int[,] edges, int width, int height)
        {
            //thinning
            int[,] result = KMMAlgorithm.KMM(edges, width, height);



            return result;
        }

        public static List<Point> FindPoints(int[,] edges, int width, int height)
        {
            List<Point> result = new List<Point>();

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (edges[x, y] == 1)
                        result.Add(new Point(x, y));

            return result;
        }

        public static int[,] MakeMatrixFromPoints(int width, int height, List<Point> points)
        {
            int[,] matrix = new int[width, height];
            foreach (var p in points)
                matrix[p.x, p.y] = 1;
            return matrix;
        }

        public static int[,] ReadMatrixFromBitmap(System.Drawing.Bitmap bitmap)
        {
            int[,] matrix = new int[bitmap.Width, bitmap.Height];
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    matrix[x, y] = bitmap.GetPixel(x, y).R;
                }
            }
            return matrix;
        }
    }
}
