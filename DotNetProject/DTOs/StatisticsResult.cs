using System;
using DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DTOs
{
public class StatisticsResult
    {
        public Point CenterOfMass;
        public int[] Histogram;
        public int HistogramMin;
        public int HistogramMax;
        public double HistogramMean;
        public double Area;
        public double Permieter;
        public int NumberOfPixelsInsideContour;
        public int NumberOfPixelsOfContour;
    }
}