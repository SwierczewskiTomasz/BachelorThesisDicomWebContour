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

        public List<SemiAutomaticContourDTO> FetchAll()
        {
            List<SemiAutomaticContourDTO> contours = new List<SemiAutomaticContourDTO>();
            foreach(Guid guid in repository.FetchAll())
            {
                contours.Add(repository.Load(guid));
            }
            return contours;
        }

        public SemiAutomaticContourDTO Get(Guid guid)
        {
            SemiAutomaticContourDTO result = null;
            try
            {
                result = repository.Load(guid);
            }
            catch(Exception e)
            {
                Console.WriteLine("Catched exception: ");
                Console.WriteLine(e.ToString());
            }
            return result;
        }

        public void Add(SemiAutomaticPointsDTO contour)
        {
            SemiAutomaticContourDTO result = SemiAutomatic.Default(contour);
            repository.Save(result);
        }

        public void Delete(Guid guid)
        {
            repository.Delete(guid);
        }

        public void Edit(SemiAutomaticContourDTO contour)
        {
            repository.Edit(contour);
        }
    }
}
