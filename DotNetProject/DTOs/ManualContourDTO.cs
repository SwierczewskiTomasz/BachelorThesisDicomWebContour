using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DTOs
{
    public class Point
    {
        public int x { get; set; }
        public int y { get; set; }

        public Point()
        {

        }

        public Point(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }
    public class Line
    {
        public List<Point> pixels;
        public string brushColor;
    }

    public class ManualContourDTO
    {
        public Guid guid { get; set; }
        public string dicomid { get; set; }
        public string tag { get; set; }
        public List<Line> lines { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public StatisticsResult statistics { get; set; }

        public ManualContourDTO()
        {
            guid = Guid.NewGuid();
        }

        public ManualContourDTO(Guid _guid, string _DICOMid, string _tag, List<Line> _lines, int _width, int _height, StatisticsResult _statistics)
        {
            guid = _guid;
            dicomid = _DICOMid;
            tag = _tag;
            lines = new List<Line>(_lines);
            width = _width;
            height = _height;
            statistics = _statistics;
        }

        public ManualContourDTO(string _DICOMid, string _tag, List<Line> _lines, int _width, int _height, StatisticsResult _statistics)
        {
            guid = Guid.NewGuid();
            dicomid = _DICOMid;
            tag = _tag;
            lines = new List<Line>(_lines);
            width = _width;
            height = _height;
            statistics = _statistics;
        }
    }
}
