using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DTOs;
using Logic;

namespace API.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class ManualContourController : ControllerBase
    {
        //readonly private ManualContourRepository repository;
        readonly private ManualContourLogic logic;

        public ManualContourController(ManualContourLogic _logic)
        {
            logic = _logic;
        }

        public ManualContourController()
        {
            logic = new ManualContourLogic();
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<ManualContourDTO>> Get()
        {
            return logic.FetchAll();
        }

        // GET api/values/5
        [HttpGet("{guid}")]
        [ProducesResponseType(404)]
        public ActionResult<ManualContourDTO> Get(Guid guid)
        {
            ManualContourDTO contour = logic.Query.Where(c => c.guid == guid).First();

            if(contour == null)
                return NotFound();

            return contour;
        }

        // POST api/values
        [HttpPost]
        [ProducesResponseType(400)]
        public ActionResult<ManualContourDTO> Post([FromBody] ManualContourDTO contour)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            logic.Add(contour);

            return CreatedAtAction(nameof(Get),
                new { guid = contour.guid }, contour);
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
