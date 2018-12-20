using System;
using DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;

namespace Logic
{
    public static class BresenhamClass
    {
        static int radius = 0;

        public static List<Point> Bresenham(List<Point> pixels, int x1, int y1, int x2, int y2)
        {
            if (x1 > x2)
            {
                int c = x2;
                x2 = x1;
                x1 = c;

                c = y2;
                y2 = y1;
                y1 = c;
            }

            if (y1 > y2)
            {
                if (x2 - x1 > y1 - y2)
                    pixels.Concat(BresenhamNE(pixels, x1, x2, y1, y2));
                else
                    pixels.Concat(BresenhamNNE(pixels, x1, x2, y1, y2));
            }
            else
            {
                if (x2 - x1 > y2 - y1)
                    pixels.Concat(BresenhamSE(pixels, x1, x2, y1, y2));
                else
                    pixels.Concat(BresenhamSSE(pixels, x1, x2, y1, y2));
            }
            return pixels;
        }

        public static List<Point> BresenhamNNE(List<Point> pixels, int x1, int x2, int y1, int y2)
        {
            int dy = x2 - x1;
            int dx = y1 - y2;
            int d = 2 * dy - dx;        //initial value of d
            int incrE = 2 * dy;         //increment used for move to E
            int incrNE = 2 * (dy - dx); //increment used for move to NE
            int x = x2;
            int y = y2;

            for (int i = 0; i < radius; i++)
            {
                if (d < 0) //choose E
                {
                    d += incrE;
                    y++;
                }
                else //choose NE
                {
                    d += incrNE;
                    x--;
                    y++;
                }
            }

            while (y < y1 - radius)
            {
                if (d < 0) //choose E
                {
                    d += incrE;
                    y++;
                    pixels.Add(new Point(x, y));
                }
                else //choose NE
                {
                    d += incrNE;
                    x--;
                    y++;
                    pixels.Add(new Point(x, y));
                }
            }
            return pixels;
        }

        public static List<Point> BresenhamNE(List<Point> pixels, int x1, int x2, int y1, int y2)
        {
            int dx = x2 - x1;
            int dy = y1 - y2;
            int d = 2 * dy - dx;        //initial value of d
            int incrE = 2 * dy;         //increment used for move to E
            int incrNE = 2 * (dy - dx); //increment used for move to NE
            int x = x1;
            int y = y1;

            for (int i = 0; i < radius; i++)
            {
                if (d < 0) //choose E
                {
                    d += incrE;
                    x++;
                }
                else //choose NE
                {
                    d += incrNE;
                    x++;
                    y--;
                }
            }

            while (x < x2 - radius)
            {
                if (d < 0) //choose E
                {
                    d += incrE;
                    x++;
                    pixels.Add(new Point(x, y));
                }
                else //choose NE
                {
                    d += incrNE;
                    x++;
                    y--;
                    pixels.Add(new Point(x, y));
                }
            }
            return pixels;
        }

        public static List<Point> BresenhamSE(List<Point> pixels, int x1, int x2, int y1, int y2)
        {
            int dx = x2 - x1;
            int dy = y2 - y1;
            int d = 2 * dy - dx;        //initial value of d
            int incrE = 2 * dy;         //increment used for move to E
            int incrNE = 2 * (dy - dx); //increment used for move to NE
            int x = x1;
            int y = y1;

            for (int i = 0; i < radius; i++)
            {
                if (d < 0) //choose E
                {
                    d += incrE;
                    x++;
                }
                else //choose NE
                {
                    d += incrNE;
                    x++;
                    y++;
                }
            }

            while (x < x2 - radius)
            {
                if (d < 0) //choose E
                {
                    d += incrE;
                    x++;
                    pixels.Add(new Point(x, y));
                }
                else //choose NE
                {
                    d += incrNE;
                    x++;
                    y++;
                    pixels.Add(new Point(x, y));
                }
            }
            return pixels;
        }

        public static List<Point> BresenhamSSE(List<Point> pixels, int x1, int x2, int y1, int y2)
        {
            int dy = x2 - x1;
            int dx = y2 - y1;
            int d = 2 * dy - dx;        //initial value of d
            int incrE = 2 * dy;         //increment used for move to E
            int incrNE = 2 * (dy - dx); //increment used for move to NE
            int x = x1;
            int y = y1;
         
            for (int i = 0; i < radius; i++)
            {
                if (d < 0) //choose E
                {
                    d += incrE;
                    y++;
                }
                else //choose NE
                {
                    d += incrNE;
                    x++;
                    y++;
                }
            }

            while (y < y2 - radius)
            {
                if (d < 0) //choose E
                {
                    d += incrE;
                    y++;
                    pixels.Add(new Point(x, y));
                }
                else //choose NE
                {
                    d += incrNE;
                    x++;
                    y++;
                    pixels.Add(new Point(x, y));
                }
            }
            return pixels;
        }
    }
}