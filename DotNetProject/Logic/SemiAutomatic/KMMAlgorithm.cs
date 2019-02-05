using System;
using DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;

namespace Logic
{
    public static class KMMAlgorithm
    {
        public static int[,] KMM(int[,] tab, int xLength, int yLength)
        {
            int numberOfDeleted;
            do
            {
                numberOfDeleted = 0;
                tab = Set2(tab, xLength, yLength);
                tab = Set3(tab, xLength, yLength);
                tab = Set4(tab, xLength, yLength);
                tab = Delete4(tab, xLength, yLength, ref numberOfDeleted);
                tab = Loop(2, tab, xLength, yLength, ref numberOfDeleted);
                tab = Loop(3, tab, xLength, yLength, ref numberOfDeleted);
            } while (numberOfDeleted != 0);

            return tab;
        }

        private static int[,] Set2(int[,] tab, int xLength, int yLength)
        {
            for(int x = 0; x < xLength; x++)
            {
                for(int y = 0; y < yLength; y++)
                {
                    if (tab[x, y] == 1)
                    {
                        if (x - 1 > 0)
                            if (tab[x - 1, y] == 0)
                                tab[x, y] = 2;
                        if (x + 1 < xLength)
                            if (tab[x + 1, y] == 0)
                                tab[x, y] = 2;
                        if (y - 1 > 0)
                            if (tab[x, y - 1] == 0)
                                tab[x, y] = 2;
                        if (y + 1 < yLength)
                            if (tab[x, y + 1] == 0)
                                tab[x, y] = 2;
                    }
                }
            }
            return tab;
        }

        private static int[,] Set3(int[,] tab, int xLength, int yLength)
        {
            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    if (tab[x, y] == 1)
                    {
                        if (x - 1 > 0 && y - 1 > 0)
                            if (tab[x - 1, y - 1] == 0)
                                tab[x, y] = 3;
                        if (x + 1 < xLength && y - 1 > 0)
                            if (tab[x + 1, y - 1] == 0)
                                tab[x, y] = 3;
                        if (x - 1 > 0 && y + 1 < yLength)
                            if (tab[x - 1, y + 1] == 0)
                                tab[x, y] = 3;
                        if (x + 1 < xLength && y + 1 < yLength)
                            if (tab[x + 1, y + 1] == 0)
                                tab[x, y] = 3;
                    }
                }
            }
            return tab;
        }

        private static int[,] Set4(int[,] tab, int xLength, int yLength)
        {
            int[] tab2 = new int[]
            {
                3, 6, 12, 24, 48, 96, 192, 129,
                7, 14, 28, 56, 112, 224, 193, 131,
                15, 30, 60, 120, 240, 225, 195, 135
            };

            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    if (tab[x, y] == 2)
                    {
                        int sum = 0;
                        if (y - 1 > 0)
                            if (tab[x + 0, y - 1] != 0)
                                sum += 1;
                        if (x + 1 < xLength && y - 1 > 0)
                            if (tab[x + 1, y - 1] != 0)
                                sum += 2;
                        if (x + 1 < xLength)
                            if (tab[x + 1, y + 0] != 0)
                                sum += 4;
                        if (x + 1 < xLength && y + 1 < yLength)
                            if (tab[x + 1, y + 1] != 0)
                                sum += 8;
                        if (y + 1 < yLength)
                            if (tab[x + 0, y + 1] != 0)
                                sum += 16;
                        if (x - 1 > 0 && y + 1 < yLength)
                            if (tab[x - 1, y + 1] != 0)
                                sum += 32;
                        if (x - 1 > 0)
                            if (tab[x - 1, y + 0] != 0)
                                sum += 64;
                        if (x - 1 > 0 && y - 1 > 0)
                            if (tab[x - 1, y - 1] != 0)
                                sum += 128;
                        if (tab2.Any(t => t == sum))
                        {
                            tab[x, y] = 4;
                        }
                    }
                }
            }
            return tab;
        }

        private static int[,] Delete4(int[,] tab, int xLength, int yLength, ref int numberOfDeleted)
        {
            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    if (tab[x, y] == 4)
                    {
                        tab[x, y] = 0;
                        numberOfDeleted++;
                    }
                }
            }
            return tab;
        }

        private static int[,] Loop(int N, int[,] tab, int xLength, int yLength, ref int numberOfDeleted)
        {
            int[] tab2 = new int[] { 3, 5, 7, 12, 13, 14, 15, 20,
                                    21, 22, 23, 28, 29, 30, 31, 48,
                                    52, 53, 54, 55, 56, 60, 61, 62,
                                    63, 65, 67, 69, 71, 77, 79, 80,
                                    81, 83, 84, 85, 86, 87, 88, 89,
                                    91, 92, 93, 94, 95, 97, 99, 101,
                                    103, 109, 111, 112, 113, 115, 116, 117,
                                    118, 119, 120, 121, 123, 124, 125, 126,
                                    127, 131, 133, 135, 141, 143, 149, 151,
                                    157, 159, 181, 183, 189, 191, 192, 193,
                                    195, 197, 199, 205, 207, 208, 209, 211,
                                    212, 213, 214, 215, 216, 217, 219, 220,
                                    221, 222, 223, 224, 225, 227, 229, 231,
                                    237, 239, 240, 241, 243, 244, 245, 246,
                                    247, 248, 249, 251, 252, 253, 254, 255 };

            for (int x = 0; x < xLength; x++)
            {
                for(int y=0; y < yLength; y++)
                {
                    if(tab[x, y] == N)
                    {
                        int sum = 0;
                        if (y - 1 > 0)
                            if (tab[x + 0, y - 1] != 0)
                                sum += 1;
                        if (x + 1 < xLength && y - 1 > 0)
                            if (tab[x + 1, y - 1] != 0)
                                sum += 2;
                        if (x + 1 < xLength)
                            if (tab[x + 1, y + 0] != 0)
                                sum += 4;
                        if (x + 1 < xLength && y + 1 < yLength)
                            if (tab[x + 1, y + 1] != 0)
                                sum += 8;
                        if (y + 1 < yLength)
                            if (tab[x + 0, y + 1] != 0)
                                sum += 16;
                        if (x - 1 > 0 && y + 1 < yLength)
                            if (tab[x - 1, y + 1] != 0)
                                sum += 32;
                        if (x - 1 > 0)
                            if (tab[x - 1, y + 0] != 0)
                                sum += 64;
                        if (x - 1 > 0 && y - 1 > 0)
                            if (tab[x - 1, y - 1] != 0)
                                sum += 128;
                        if (tab2.Any(t => t == sum))
                        {
                            tab[x, y] = 0;
                            numberOfDeleted++;
                        }
                    }
                }
            }
            return tab;
        }
    }
}