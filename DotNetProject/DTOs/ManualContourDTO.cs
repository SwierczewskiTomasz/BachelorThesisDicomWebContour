using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DTOs
{
    public class Point
    {
        public int x;
        public int y;

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
        public Guid guid;
        public string dicomid;
        public string tag;
        public List<Line> lines;
        public int width;
        public int height;

        public ManualContourDTO(Guid _guid, string _DICOMid, string _tag, List<Line> _lines, int _width, int _height)
        {
            guid = _guid;
            dicomid = _DICOMid;
            tag = _tag;
            lines = new List<Line>(_lines);
            width = _width;
            height = _height;
        }

        public ManualContourDTO(string _DICOMid, string _tag, List<Line> _lines, int _width, int _height)
        {
            guid = Guid.NewGuid();
            dicomid = _DICOMid;
            tag = _tag;
            lines = new List<Line>(_lines);
            width = _width;
            height = _height;
        }
    }
}
