using System;
using DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;

namespace Logic
{
        public interface ILogic<TEntity> where TEntity : class
    {
        List<TEntity> FetchAll();
        IQueryable<TEntity> Query { get; }
        void Add(TEntity entity);
        void Delete(TEntity entity);
        void Save();
    }

    public class ManualContourLogic
    {
        private readonly ManualContourRepository repository = new ManualContourRepository();

        public List<ManualContourDTO> FetchAll()
        {
            List<ManualContourDTO> contours = new List<ManualContourDTO>();
            foreach(Guid guid in repository.FetchAll())
            {
                contours.Add(repository.Load(guid));
            }
            return contours;
        }

        public ManualContourDTO Get(Guid guid)
        {
            ManualContourDTO result = null;
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

        public void Add(ManualContourDTO contour)
        {
            repository.Save(contour);
        }

        public void Delete(Guid guid)
        {
            repository.Delete(guid);
        }

        public void Edit(ManualContourDTO contour)
        {
            repository.Edit(contour);
        }
    }
}
