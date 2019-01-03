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

        public static List<Point> Canny(string dicomId, List<Point> points)
        {
            System.Drawing.Bitmap bitmap = OrthancConnection.GetBitmapByInstanceId(dicomId);

            double[,][] sobel;
            int width, height;

            (sobel, width, height) = SobelOperator(bitmap);
            double[,][] gradient = FindIntensityGradient(sobel, width, height);
            double[,] edges = NonMaximumSuppression(gradient, width, height);
            int[] distribution = DistributionFunction(edges, width, height);
            distribution = CumulativeDistributionFunction(distribution);

            int min, max;
            double lowerThreshold = 0.70, higherThreshold = 0.95;

            (min, max) = ChooseThreshold(distribution, lowerThreshold, higherThreshold);
            int[,] foundedEdges = HysteresisThreshold(edges, width, height, min, max);

            //All pixels, all edges, no connection from one point to other one!!!
            List<Point> pixels = new List<Point>(FindPoints(foundedEdges, width, height));

            return pixels;
        }

        public static (double[,][], int, int) SobelOperator(System.Drawing.Bitmap bitmap)
        {
            double[,][] result = new double[bitmap.Width, bitmap.Height][];

            for (int x = 1; x < bitmap.Width - 1; x++)
            {
                for (int y = 1; y < bitmap.Height - 1; y++)
                {
                    double s1, s2;

                    s1 = bitmap.GetPixel(x - 1, y + 1).R + 2 * bitmap.GetPixel(x, y + 1).R + bitmap.GetPixel(x + 1, y + 1).R;
                    s1 -= bitmap.GetPixel(x - 1, y - 1).R + 2 * bitmap.GetPixel(x, y - 1).R + bitmap.GetPixel(x + 1, y - 1).R;

                    s2 = bitmap.GetPixel(x + 1, y - 1).R + 2 * bitmap.GetPixel(x + 1, y).R + bitmap.GetPixel(x + 1, y + 1).R;
                    s2 -= bitmap.GetPixel(x - 1, y - 1).R + 2 * bitmap.GetPixel(x - 1, y).R + bitmap.GetPixel(x - 1, y + 1).R;

                    result[x, y] = new double[2] { s1, s2 };
                }
            }

            return (result, bitmap.Width, bitmap.Height);
        }

        public static double[,][] FindIntensityGradient(double[,][] sobel, int width, int height)
        {
            double[,][] result = (double[,][])sobel.Clone();

            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
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

        public static double[,] NonMaximumSuppression(double[,][] gradient, int width, int height)
        {
            double[,] result = new double[width, height];

            for (int x = 2; x < width - 2; x++)
            {
                for (int y = 2; y < height - 2; y++)
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

        public static int[] DistributionFunction(double[,] edges, int width, int height)
        {
            int[] result = new int[numberOfColors];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    result[(int)edges[x, y] < 256 ? (int)edges[x, y] : 255]++;
            return result;
        }

        public static int[] CumulativeDistributionFunction(int[] distribution)
        {
            for (int i = 1; i < numberOfColors; i++)
                distribution[i] += distribution[i - 1];
            return distribution;
        }

        public static (int, int) ChooseThreshold(int[] cumulativeDistribution, double lowerThreshold, double higherThreshold)
        {
            int min = 0;
            int max = 0;

            for (int i = 0; i < numberOfColors; i++)
            {
                if (cumulativeDistribution[i] < cumulativeDistribution[numberOfColors - 1] * lowerThreshold)
                    min = i;
                if (cumulativeDistribution[i] < cumulativeDistribution[numberOfColors - 1] * higherThreshold)
                    max = i;
            }

            return (min, max);
        }

        public static int[,] HysteresisThreshold(double[,] edges, int width, int height, int min, int max)
        {
            int[,] result = new int[width, height];
            List<(int, int)> list = new List<(int, int)>();

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (edges[x, y] > max)
                        list.Add((x, y));

            while (list.Count != 0)
            {
                int x, y;
                (x, y) = list.First();
                list.Remove(list.First());

                if (edges[x, y] > max)
                {
                    result[x, y] = 1;

                    for (int i = x - 1 < 0 ? 0 : x - 1; i < (x + 1 < width ? x + 1 : width); i++)
                        for (int j = y - 1 < 0 ? 0 : y - 1; y < (y + 1 < height ? y + 1 : height); y++)
                            if (i != x && j != y && result[i, j] != 1)
                            {
                                list.Insert(0, (i, j));
                                result[i, j] = 2;
                            }

                }
                else if (edges[x, y] > min && result[x, y] == 2)
                {
                    result[x, y] = 1;

                    for (int i = x - 1 < 0 ? 0 : x - 1; i < (x + 1 < width ? x + 1 : width); i++)
                        for (int j = y - 1 < 0 ? 0 : y - 1; y < (y + 1 < height ? y + 1 : height); y++)
                            if (i != x && j != y && result[i, j] != 1)
                            {
                                list.Insert(0, (i, j));
                                result[i, j] = 2;
                            }
                }
                else
                    result[x, y] = 0;
            }

            return result;
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
    }
}
