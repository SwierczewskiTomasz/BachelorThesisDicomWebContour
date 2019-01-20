using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DTOs
{
    public class SemiAutomaticPreviewDTO
    {
        public Guid guid { get; set; }
        public string dicomid { get; set; }
        public string tag { get; set; }
        public List<LinePointsAndPixels> lines { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public double pixelSpacing {get; set;}
        public int runAlgorithm {get; set;}

        public SemiAutomaticPreviewDTO()
        {
            guid = Guid.NewGuid();
        }

        public SemiAutomaticPreviewDTO(Guid _guid, string _DICOMid, string _tag, List<LinePointsAndPixels> _lines, int _width, int _height, 
            double _pixelSpacing, int _runAlgorithm)
        {
            guid = _guid;
            dicomid = _DICOMid;
            tag = _tag;
            lines = new List<LinePointsAndPixels>(_lines);
            width = _width;
            height = _height;
            pixelSpacing = _pixelSpacing;
            runAlgorithm = _runAlgorithm;
        }

        public SemiAutomaticPreviewDTO(string _DICOMid, string _tag, List<LinePointsAndPixels> _lines, int _width, int _height, 
            double _pixelSpacing, int _runAlgorithm)
        {
            guid = Guid.NewGuid();
            dicomid = _DICOMid;
            tag = _tag;
            lines = new List<LinePointsAndPixels>(_lines);
            width = _width;
            height = _height;
            pixelSpacing = _pixelSpacing;
            runAlgorithm = _runAlgorithm;
        }
    }
}