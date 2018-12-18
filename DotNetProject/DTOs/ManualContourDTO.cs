using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DTOs
{
    public class ManualContourDTO
    {
        public Guid guid;
        public Guid DICOMguid;
        public uint color;
        public string tag;

        public List<(int, int)> pixels;

        public ManualContourDTO(Guid _guid, Guid _DICOMguid, List<(int, int)> _pixels, uint _color, string _tag)
        {
            guid = _guid;
            DICOMguid = _DICOMguid;
            pixels = new List<(int, int)>(_pixels);
            tag = _tag;
        }

        public ManualContourDTO(Guid _DICOMguid, List<(int, int)> _pixels, uint _color, string _tag)
        {
            guid = Guid.NewGuid();
            DICOMguid = _DICOMguid;
            pixels = new List<(int, int)>(_pixels);
            tag = _tag;
        }
    }
}
