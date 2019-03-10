using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;
using DTOs;
using Logic;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowSpecificOrigin")]
    [ApiController]
    public class SemiAutomaticPreviewController : ControllerBase
    {
        readonly private SemiAutomaticPreviewLogic logic;

        private readonly ILogger<SemiAutomaticPreviewController> _logger;


        public SemiAutomaticPreviewController(ILogger<SemiAutomaticPreviewController> logger)
        {
            _logger = logger;
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
            _logger.LogInformation("Post");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (points == null)
                return BadRequest();

            if (points.lines == null)
                return BadRequest();

            if (points.lines.Count == 0)
                return BadRequest();

            foreach (var l in points.lines)
            {
                if (l.points == null)
                    return BadRequest();

                if (l.points.Count < 3)
                    return BadRequest();
            }

            SemiAutomaticPreviewDTO result = logic.Add(points);

            return CreatedAtAction(nameof(Get),
                new { guid = result.guid }, result);
        }

        [Route("[action]/")]
        [HttpPut]
        [ProducesResponseType(400)]
        public ActionResult<SemiAutomaticPreviewDTO> Put([FromBody] SemiAutomaticPreviewDTO contour)
        {
            SemiAutomaticPreviewDTO result;
            bool boolResult;
            (boolResult, result) = logic.Edit(contour);
            if (!boolResult)
                return BadRequest();
            if (result == null)
                return BadRequest();
            return CreatedAtAction(nameof(Get),
                new { guid = result.guid }, result);
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
