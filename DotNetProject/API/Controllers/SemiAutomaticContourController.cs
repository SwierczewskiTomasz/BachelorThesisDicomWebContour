using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DTOs;
using Logic;
using Microsoft.AspNetCore.Cors;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowSpecificOrigin")]
    [ApiController]
    public class SemiAutomaticContourController : ControllerBase
    {
        readonly private SemiAutomaticContourLogic logic;

        public SemiAutomaticContourController()
        {
            logic = new SemiAutomaticContourLogic();
        }

        [Route("[action]")]
        [HttpGet]
        public ActionResult<IEnumerable<Guid>> FetchAll()
        {
            return logic.FetchAll();
        }

        [Route("[action]")]
        [HttpGet]
        public ActionResult<IEnumerable<SemiAutomaticContourDTO>> FetchAllToDTOs()
        {
            return logic.FetchAllToDTOs();
        }

        [Route("[action]/{id}")]
        [HttpGet]
        public ActionResult<IEnumerable<Guid>> FetchByDicomId(string id)
        {
            return logic.FetchByDicomId(id);
        }

        [Route("[action]/{id}")]
        [HttpGet]
        public ActionResult<IEnumerable<SemiAutomaticContourDTO>> FetchByDicomIdToDTOs(string id)
        {
            return logic.FetchByDicomIdToDTOs(id);
        }

        [Route("[action]/{guid}")]
        [HttpGet]
        [ProducesResponseType(404)]
        public ActionResult<SemiAutomaticContourDTO> Get(Guid guid)
        {
            SemiAutomaticContourDTO contour = logic.Get(guid);

            if (contour == null)
                return NotFound();

            return contour;
        }

        [Route("[action]/")]
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

        [Route("[action]/")]
        [HttpPut]
        public void Put([FromBody] SemiAutomaticContourDTO contour)
        {
            logic.Edit(contour);
        }

        [Route("[action]/{guid}")]
        [HttpDelete]
        public void Delete(Guid guid)
        {
            // if(logic.Delete(guid))
            //     return OK();
            // return NotFound();
            logic.Delete(guid);
        }
    }
}
