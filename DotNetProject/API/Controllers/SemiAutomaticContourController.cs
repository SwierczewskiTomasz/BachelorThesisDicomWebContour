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
    public class SemiAutomaticContourController : ControllerBase
    {
        readonly private SemiAutomaticContourLogic logic;

        // public ManualContourController(ManualContourLogic _logic)
        // {
        //     logic = _logic;
        // }

        public SemiAutomaticContourController()
        {
            logic = new SemiAutomaticContourLogic();
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<SemiAutomaticContourDTO>> Get()
        {
            return logic.FetchAll();
        }

        // GET api/values/5
        [HttpGet("{guid}")]
        [ProducesResponseType(404)]
        public ActionResult<SemiAutomaticContourDTO> Get(Guid guid)
        {
            SemiAutomaticContourDTO contour = logic.Get(guid);

            if (contour == null)
                return NotFound();

            return contour;
        }

        // POST api/values
        [HttpPost]
        [ProducesResponseType(400)]
        public ActionResult<SemiAutomaticContourDTO> Post([FromBody] SemiAutomaticPointsDTO points)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SemiAutomaticContourDTO result = logic.Add(points);

            return CreatedAtAction(nameof(Get),
                new { guid = result.guid }, result);
        }

        // PUT api/values/5
        [HttpPut]
        public void Put([FromBody] SemiAutomaticContourDTO contour)
        {
            logic.Edit(contour);
        }

        // DELETE api/values/5
        [HttpDelete("{guid}")]
        public void Delete(Guid guid)
        {
            logic.Delete(guid);
        }
    }
}
