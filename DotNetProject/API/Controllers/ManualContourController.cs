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

        // public ManualContourController(ManualContourLogic _logic)
        // {
        //     logic = _logic;
        // }

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
            ManualContourDTO contour = logic.Get(guid);

            if (contour == null)
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

        [HttpPost("{DICOMid}")]
        public ActionResult<Guid> Post(string DICOMid)
        {
            List<Line> lines = new List<Line>();
            Line line = new Line();
            line.pixels = new List<Point>();
            line.pixels.Add(new Point(0, 0));
            line.pixels.Add(new Point(1, 0));
            line.pixels.Add(new Point(2, 0));
            line.pixels.Add(new Point(2, 1));
            line.pixels.Add(new Point(2, 2));
            line.pixels.Add(new Point(1, 2));
            line.pixels.Add(new Point(0, 2));
            line.pixels.Add(new Point(0, 1));
            line.brushColor = "#f00";
            lines.Add(line);

            ManualContourDTO contour = new ManualContourDTO(DICOMid, "Test", lines, 500, 500);
            logic.Add(contour);
            return Ok(contour.guid);
        }

        // PUT api/values/5
        [HttpPut]
        public void Put([FromBody] ManualContourDTO contour)
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
