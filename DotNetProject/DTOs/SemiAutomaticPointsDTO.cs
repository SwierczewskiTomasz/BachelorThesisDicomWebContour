using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DTOs
{
    public class SemiAutomaticPointsDTO
    {
        public Guid guid;
        public Guid DICOMguid;
        uint color;
        string tag;

        List<(int, int)> points;

        public SemiAutomaticPointsDTO(Guid _DICOMguid, List<(int, int)> _points, uint _color, string _tag)
        {
            guid = Guid.NewGuid();
            DICOMguid = _DICOMguid;
            points = new List<(int, int)>(_points);
            tag = _tag;
        }
    }
}