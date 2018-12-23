using System;
using DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;

namespace Logic
{
    public class SemiAutomaticContourLogic
    {
        private readonly SemiAutomaticContourRepository repository = new SemiAutomaticContourRepository();

        public List<SemiAutomaticContourDTO> FetchAllToDTOs()
        {
            List<SemiAutomaticContourDTO> contours = new List<SemiAutomaticContourDTO>();
            foreach (Guid guid in repository.FetchAll())
            {
                contours.Add(repository.Load(guid));
            }
            return contours;
        }

        public List<SemiAutomaticContourDTO> FetchByDicomIdToDTOs(string dicomid)
        {
            List<SemiAutomaticContourDTO> contours = new List<SemiAutomaticContourDTO>();
            foreach (Guid guid in repository.FetchByDicomId(dicomid))
            {
                contours.Add(repository.Load(guid));
            }
            return contours;
        }

        public List<Guid> FetchAll() => repository.FetchAll();

        public List<Guid> FetchByDicomId(string dicomid) => repository.FetchByDicomId(dicomid);


        public SemiAutomaticContourDTO Get(Guid guid)
        {
            SemiAutomaticContourDTO result = null;
            try
            {
                result = repository.Load(guid);
            }
            catch (Exception e)
            {
                Console.WriteLine("Catched exception: ");
                Console.WriteLine(e.ToString());
            }
            return result;
        }

        public SemiAutomaticContourDTO Add(SemiAutomaticPointsDTO contour)
        {
            SemiAutomaticContourDTO result = SemiAutomatic.Default(contour);
            repository.Save(result);
            return result;
        }

        public bool Delete(Guid guid)
        {
            return repository.Delete(guid);
        }

        public void Edit(SemiAutomaticContourDTO contour)
        {
            repository.Edit(contour);
        }
    }
}
