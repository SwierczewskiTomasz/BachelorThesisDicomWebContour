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

        public SemiAutomaticPreviewDTO()
        {
            guid = Guid.NewGuid();
        }

        public SemiAutomaticPreviewDTO(Guid _guid, string _DICOMid, string _tag, List<LinePointsAndPixels> _lines, int _width, int _height)
        {
            guid = _guid;
            dicomid = _DICOMid;
            tag = _tag;
            lines = new List<LinePointsAndPixels>(_lines);
            width = _width;
            height = _height;
        }

        public SemiAutomaticPreviewDTO(string _DICOMid, string _tag, List<LinePointsAndPixels> _lines, int _width, int _height)
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