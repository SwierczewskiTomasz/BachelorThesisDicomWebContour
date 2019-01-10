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

            logic.Add(contour);

            return CreatedAtAction(nameof(Get),
                new { guid = contour.guid }, contour);
        }

        [Route("[action]/{DICOMid}")]
        [HttpPost]
        public ActionResult<Guid> Post(string DICOMid)
        {
            List<Line> lines = new List<Line>();
            Line line = new Line();
            line.points = new List<Point>();
            line.points.Add(new Point(0, 0));
            line.points.Add(new Point(1, 0));
            line.points.Add(new Point(2, 0));
            line.points.Add(new Point(2, 1));
            line.points.Add(new Point(2, 2));
            line.points.Add(new Point(1, 2));
            line.points.Add(new Point(0, 2));
            line.points.Add(new Point(0, 1));
            line.brushColor = "#f00";
            lines.Add(line);

            ManualContourDTO contour = new ManualContourDTO(DICOMid, "Test", lines, 500, 500);
            logic.Add(contour);
            return Ok(contour.guid);
        }

        [Route("[action]/")]
        [HttpPut]
        public void Put([FromBody] ManualContourDTO contour)
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
