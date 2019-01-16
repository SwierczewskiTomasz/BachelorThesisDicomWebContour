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
    public class SemiAutomaticPreviewController : ControllerBase
    {
        readonly private SemiAutomaticPreviewLogic logic;

        public SemiAutomaticPreviewController()
        {
            logic = new SemiAutomaticPreviewLogic();
        }

        [Route("[action]/{guid}")]
        [HttpGet]
        [ProducesResponseType(404)]
        public ActionResult<SemiAutomaticPreviewDTO> Get(Guid guid)
        {
            SemiAutomaticPreviewDTO contour = logic.Get(guid);

            if (contour == null)
                return NotFound();

            return contour;
        }

        [Route("[action]/")]
        [HttpPost]
        [ProducesResponseType(400)]
        public ActionResult<SemiAutomaticPreviewDTO> Post([FromBody] SemiAutomaticPreviewDTO points)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SemiAutomaticPreviewDTO result = logic.Add(points);

            return CreatedAtAction(nameof(Get),
                new { guid = result.guid }, result);
        }

        [Route("[action]/")]
        [HttpPut]
        public ActionResult<SemiAutomaticPreviewDTO> Put([FromBody] SemiAutomaticPreviewDTO contour)
        {
            SemiAutomaticPreviewDTO result = logic.Edit(contour);
            return CreatedAtAction(nameof(Get),
                new { guid = result.guid }, result);
        }

        [Route("[action]/{guid}")]
        [HttpDelete]
        public void Delete(Guid guid)
        {
            logic.Delete(guid);
        }
    }
}
