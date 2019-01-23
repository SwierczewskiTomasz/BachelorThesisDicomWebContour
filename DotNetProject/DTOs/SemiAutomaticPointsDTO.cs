using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DTOs
{
    public class LinePoints
    {
        public List<Point> points;
        public string brushColor;
    }

    public class SemiAutomaticPointsDTO
    {
        public Guid guid { get; set; }
        public string dicomid { get; set; }
        public string tag { get; set; }
        public List<LinePoints> lines { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public List<Point> centralPoints { get; set; }
        public string pixelSpacing { get; set; }

        public SemiAutomaticPointsDTO()
        {
            guid = Guid.NewGuid();
        }

        public SemiAutomaticPointsDTO(Guid _guid, string _dicomid, string _tag, List<LinePoints> _lines, int _width, int _height, List<Point> _centralPoints,
            string _pixelSpacing)
        {
            guid = _guid;
            dicomid = _dicomid;
            tag = _tag;
            lines = new List<LinePoints>(_lines);
            width = _width;
            height = _height;
            if (_centralPoints != null)
            {
                centralPoints = new List<Point>(_centralPoints);
            }
            pixelSpacing = _pixelSpacing;
        }

        public SemiAutomaticPointsDTO(string _dicomid, string _tag, List<LinePoints> _lines, int _width, int _height, List<Point> _centralPoints,
            string _pixelSpacing)
        {
            guid = Guid.NewGuid();
            dicomid = _dicomid;
            tag = _tag;
            lines = new List<LinePoints>(_lines);
            width = _width;
            height = _height;
            if (_centralPoints != null)
            {
                centralPoints = new List<Point>(_centralPoints);
            }
            pixelSpacing = _pixelSpacing;
        }
    }
}