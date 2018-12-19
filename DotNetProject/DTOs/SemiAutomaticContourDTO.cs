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
        public Guid guid;
        public string dicomid;
        public string tag;
        public List<LinePointsAndPixels> lines;
        public int width;
        public int height;

        public SemiAutomaticContourDTO(Guid _guid, string _DICOMid, string _tag, List<LinePointsAndPixels> _lines, int _width, int _height)
        {
            guid = _guid;
            dicomid = _DICOMid;
            tag = _tag;
            lines = new List<LinePointsAndPixels>(_lines);
            width = _width;
            height = _height;
        }

        public SemiAutomaticContourDTO(string _DICOMid, string _tag, List<LinePointsAndPixels> _lines, int _width, int _height)
        {
            guid = Guid.NewGuid();
            dicomid = _DICOMid;
            tag = _tag;
            lines = new List<LinePointsAndPixels>(_lines);
            width = _width;
            height = _height;
        }
    }
}