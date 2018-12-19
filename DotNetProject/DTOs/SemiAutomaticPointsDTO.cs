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
        public Guid guid;
        public string dicomid;
        public string tag;
        public List<LinePoints> lines;
        public int width;
        public int height;

        public SemiAutomaticPointsDTO(Guid _guid, string _DICOMid, string _tag, List<LinePoints> _lines, int _width, int _height)
        {
            guid = _guid;
            dicomid = _DICOMid;
            tag = _tag;
            lines = new List<LinePoints>(_lines);
            width = _width;
            height = _height;
        }

        public SemiAutomaticPointsDTO(string _DICOMid, string _tag, List<LinePoints> _lines, int _width, int _height)
        {
            guid = Guid.NewGuid();
            dicomid = _DICOMid;
            tag = _tag;
            lines = new List<LinePoints>(_lines);
            width = _width;
            height = _height;
        }
    }
}