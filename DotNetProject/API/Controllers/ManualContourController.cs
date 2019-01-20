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
    //[ApiController]
    public class ManualContourController : ControllerBase
    {
        readonly private ManualContourLogic logic;

        public ManualContourController()
        {
            logic = new ManualContourLogic();
        }

        [Route("[action]")]
        [HttpGet]
        public ActionResult<IEnumerable<Guid>> FetchAll()
        {
            return logic.FetchAll();
        }

        [Route("[action]")]
        [HttpGet]
        public ActionResult<IEnumerable<ManualContourDTO>> FetchAllToDTOs()
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
        public ActionResult<IEnumerable<ManualContourDTO>> FetchByDicomIdToDTOs(string id)
        {
            return logic.FetchByDicomIdToDTOs(id);
        }

        [Route("[action]/{guid}")]
        [HttpGet]
        [ProducesResponseType(404)]
        public ActionResult<ManualContourDTO> Get(Guid guid)
        {
            ManualContourDTO contour = logic.Get(guid);

            if (contour == null)
                return NotFound();

            return contour;
        }

        [Route("[action]/")]
        [HttpPost]
        [ProducesResponseType(400)]
        public ActionResult<ManualContourDTO> Post([FromBody] ManualContourDTO contour)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (contour == null)
                return BadRequest();

            if (contour.lines == null)
                return BadRequest();

            if (contour.lines.Count == 0)
                return BadRequest();

            foreach (var l in contour.lines)
            {
                if (l.points == null)
                    return BadRequest();

                if (l.points.Count < 3)
                    return BadRequest();
            }

            logic.Add(contour);

            return CreatedAtAction(nameof(Get),
                new { guid = contour.guid }, contour);
        }

        [Route("[action]/")]
        [HttpPut]
        [ProducesResponseType(400)]
        public ActionResult Put([FromBody] ManualContourDTO contour)
        {
            if (logic.Edit(contour))
                return Ok();
            return BadRequest();
        }

        [Route("[action]/{guid}")]
        [HttpDelete]
        [ProducesResponseType(404)]
        public ActionResult Delete(Guid guid)
        {
            if (logic.Delete(guid))
                return Ok();
            return NotFound();
        }
    }
}
