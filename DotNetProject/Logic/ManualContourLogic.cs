using System;
using DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

    public class ManualContourLogic : ILogic<ManualContourDTO>
    {
        public List<ManualContourDTO> FetchAll()
        {
            throw new NotImplementedException();
        }

        public IQueryable<ManualContourDTO> Query => throw new NotImplementedException();

        public void Add(ManualContourDTO contour)
        {
            throw new NotImplementedException();
        }

        public void Delete(ManualContourDTO contour)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
