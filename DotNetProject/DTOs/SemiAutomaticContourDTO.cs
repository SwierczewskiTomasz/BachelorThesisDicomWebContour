using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DTOs
{
    public class LinePointsAndPixels
    {
        public List<Point> points;
        public List<Point> pixels;
        public string brushColor;
    }
    public class SemiAutomaticContourDTO
    {
        public Guid guid { get; set; }
        public string dicomid { get; set; }
        public string tag { get; set; }
        public List<LinePointsAndPixels> lines { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public StatisticsResult statistics { get; set; }
        public List<Point> centralPoints {get; set; }

        public SemiAutomaticContourDTO()
        {
            guid = Guid.NewGuid();
        }

        public SemiAutomaticContourDTO(Guid _guid, string _DICOMid, string _tag, List<LinePointsAndPixels> _lines, int _width, int _height, 
        StatisticsResult _statistics, List<Point> _centralPoints)
        {
            guid = _guid;
            dicomid = _DICOMid;
            tag = _tag;
            lines = new List<LinePointsAndPixels>(_lines);
            width = _width;
            height = _height;
            statistics = _statistics;
            if(_centralPoints != null)
                centralPoints = new List<Point>(_centralPoints);
        }

        public SemiAutomaticContourDTO(string _DICOMid, string _tag, List<LinePointsAndPixels> _lines, int _width, int _height, 
        StatisticsResult _statistics, List<Point> _centralPoints)
        {
            guid = Guid.NewGuid();
            dicomid = _DICOMid;
            tag = _tag;
            lines = new List<LinePointsAndPixels>(_lines);
            width = _width;
            height = _height;
            statistics = _statistics;
            if(_centralPoints != null)
                centralPoints = new List<Point>(_centralPoints);
        }
    }
}