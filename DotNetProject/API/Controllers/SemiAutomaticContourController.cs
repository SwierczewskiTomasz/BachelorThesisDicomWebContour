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

        public SemiAutomaticContourController()
        {
            logic = new SemiAutomaticContourLogic();
        }

        [Route("api/[controller]/fetchall")]
        [HttpGet]
        public ActionResult<IEnumerable<Guid>> FetchAll()
        {
            return logic.FetchAll();
        }

        [Route("api/[controller]/fetchall/todtos")]
        [HttpGet]
        public ActionResult<IEnumerable<SemiAutomaticContourDTO>> FetchAllToDTOs()
        {
            return logic.FetchAllToDTOs();
        }

        [Route("api/[controller]/fetchall/{id}")]
        [HttpGet]
        public ActionResult<IEnumerable<Guid>> FetchByDicomId(string id)
        {
            return logic.FetchByDicomId(id);
        }

        [Route("api/[controller]/fetchall/todtos/{id}")]
        [HttpGet]
        public ActionResult<IEnumerable<SemiAutomaticContourDTO>> FetchByDicomIdToDTOs(string id)
        {
            return logic.FetchByDicomIdToDTOs(id);
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
            // if(logic.Delete(guid))
            //     return OK();
            // return NotFound();
            logic.Delete(guid);
        }
    }
}
