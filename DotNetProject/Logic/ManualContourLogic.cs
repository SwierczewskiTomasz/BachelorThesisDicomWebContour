using System;
using DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;

namespace Logic
{
    public class ManualContourLogic
    {
        private readonly ManualContourRepository repository = new ManualContourRepository();

        public List<ManualContourDTO> FetchAllToDTOs()
        {
            List<ManualContourDTO> contours = new List<ManualContourDTO>();
            foreach (Guid guid in repository.FetchAll())
            {
                contours.Add(repository.Load(guid));
            }
            return contours;
        }

        public List<ManualContourDTO> FetchByDicomIdToDTOs(string dicomid)
        {
            List<ManualContourDTO> contours = new List<ManualContourDTO>();
            foreach (Guid guid in repository.FetchByDicomId(dicomid))
            {
                contours.Add(repository.Load(guid));
            }
            return contours;
        }

        public List<Guid> FetchAll() => repository.FetchAll();

        public List<Guid> FetchByDicomId(string dicomid) => repository.FetchByDicomId(dicomid);

        public ManualContourDTO Get(Guid guid)
        {
            ManualContourDTO result = null;
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

        public void Add(ManualContourDTO contour)
        {
            repository.Save(contour);
        }

        public bool Delete(Guid guid)
        {
            return repository.Delete(guid);
        }

        public void Edit(ManualContourDTO contour)
        {
            repository.Edit(contour);
        }
    }
}
