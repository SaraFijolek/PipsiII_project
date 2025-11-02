using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schematics.API.DTOs.Lines;
using Schematics.API.Service;

namespace Schematics.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("lines")]
    public class LineController : ControllerBase
    {
        private readonly ILineService _lineService;

        public LineController(ILineService lineService)
        {
            _lineService = lineService;
        }

        [HttpPost]
        public async Task<ActionResult> AddLine([FromBody] LineDto model)
        {
            await _lineService.AddLineAsync(model);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IList<LineDto>>> GetAllLines()
        {
            var lines = await _lineService.GetAllLinesAsync();
            return Ok(lines);
        }

        [HttpGet("schema/{schemaId}")]
        public async Task<ActionResult<IList<LineDto>>> GetLinesBySchemaId(int schemaId)
        {
            var lines = await _lineService.GetLinesBySchemaIdAsync(schemaId);
            return Ok(lines);
        }
    }
}

