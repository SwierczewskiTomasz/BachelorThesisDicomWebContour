using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class Contour
    {
        public Guid guid;
        public Guid DICOMguid;
        (int, int, int) color;
        string tag;

        List<(int,int)> pixels;

        public Contour(Guid _DICOMguid, List<(int, int)> _pixels, (int, int, int) _color, string _tag)
        {
            guid = Guid.NewGuid();
            DICOMguid = _DICOMguid;
            pixels = new List<(int, int)>(_pixels);
            tag = _tag;
        }
    }

    public interface IRepository<TEntity> where TEntity : class
    {
        List<TEntity> FetchAll();
        IQueryable<TEntity> Query { get; }
        void Add(TEntity entity);
        void Delete(TEntity entity);
        void Save();
    }

    public class ContourRepository : IRepository<Contour>
    {
        public IQueryable<Contour> Query => throw new NotImplementedException();

        public void Add(Contour entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Contour entity)
        {
            throw new NotImplementedException();
        }

        public List<Contour> FetchAll()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ContourController : ControllerBase
    {
        private readonly ContourRepository repository;

        public ContourController(ContourRepository _repository)
        {
            repository = _repository;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<Contour>> Get()
        {
            return repository.FetchAll();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        [ProducesResponseType(404)]
        public ActionResult<Contour> Get(Guid guid)
        {
            Contour contour = (Contour)(repository.Query.Where(c => c.guid == guid));

            if(contour == null)
                return NotFound();

            return contour;
        }

        // POST api/values
        [HttpPost]
        [ProducesResponseType(400)]
        public ActionResult<Contour> Post([FromBody] Contour contour)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            repository.Add(contour);
            repository.Save();
            return CreatedAtAction(nameof(Get), new { guid = contour.guid }, contour);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
