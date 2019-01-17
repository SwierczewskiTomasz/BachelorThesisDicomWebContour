using System;
using DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DTOs
{
    /// <summary>
    /// Class for statistics about contour
    /// </summary>
    public class StatisticsResult
    {
        /// <summary>
        /// Barycentrum, Center of Mass, Center of Contour
        /// </summary>
        public Point CenterOfMass;
        /// <summary>
        /// Histogram, array, where values are number of pixels that have instensivity equals number of index in this histogram inside contour
        /// Often this array have 256 lenght
        /// </summary>
        public int[] Histogram;
        /// <summary>
        /// Min value in histogram
        /// </summary>
        public int HistogramMin;
        /// <summary>
        /// Max value in histogram
        /// </summary>
        public int HistogramMax;
        /// <summary>
        /// Mean value in histogram
        /// </summary>
        public double HistogramMean;
        /// <summary>
        /// Area in mms inside contour
        /// </summary>
        public double Area;
        /// <summary>
        /// Lenght of contour in mms
        /// </summary>
        public double Permieter;
        /// <summary>
        /// Number od Pixels inside contour
        /// </summary>
        public int NumberOfPixelsInsideContour;
        /// <summary>
        /// Number of Pixels on contour
        /// </summary>
        public int NumberOfPixelsOfContour;
    }
}