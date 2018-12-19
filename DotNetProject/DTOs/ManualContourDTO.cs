using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DTOs
{
    public class ManualContourDTO
    {
        public Guid guid;
        public string DICOMid;
        public uint color;
        public string tag;

        public List<(int, int)> pixels;

        public ManualContourDTO(Guid _guid, string _DICOMid, List<(int, int)> _pixels, uint _color, string _tag)
        {
            guid = _guid;
            DICOMid = _DICOMid;
            pixels = new List<(int, int)>(_pixels);
            tag = _tag;
        }

        public ManualContourDTO(string _DICOMguid, List<(int, int)> _pixels, uint _color, string _tag)
        {
            guid = Guid.NewGuid();
            DICOMid = _DICOMguid;
            pixels = new List<(int, int)>(_pixels);
            tag = _tag;
        }
    }
}
