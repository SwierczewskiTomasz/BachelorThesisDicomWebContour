using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DTOs
{
    public class SemiAutomaticContourDTO
    {
        public Guid guid;
        public Guid DICOMguid;
        uint pointColor;
        uint pixelColor;
        string tag;

        List<(int, int)> points;
        List<(int, int)> pixels;

        public SemiAutomaticContourDTO(Guid _DICOMguid, List<(int, int)> _points, 
        List<(int, int)> _pixels, uint _pointColor, uint _pixelColor, string _tag)
        {
            guid = Guid.NewGuid();
            DICOMguid = _DICOMguid;
            points = new List<(int, int)>(_points);
            pixels = new List<(int, int)>(_pixels);
            pointColor = _pointColor;
            pixelColor = _pixelColor;
            tag = _tag;
        }
    }
}