using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schematics.API.DTOs.Station;
using Schematics.API.Service;

namespace Schematics.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("stations")]
    public class StationController : ControllerBase
    {
        private readonly IStationService _stationService;

        public StationController(IStationService stationService)
        {
            _stationService = stationService;
        }

        [HttpPost]
        public async Task<ActionResult> AddStation([FromBody] AddStation model)
        {
            await _stationService.AddStationAsync(model);
            return Ok();
        }

        [HttpGet("schema/{schemaId}")]
        public async Task<ActionResult<IList<StationDto>>> GetStationsBySchema(int schemaId)
        {
            var stations = await _stationService.GetStationsBySchemaIdAsync(schemaId);
            return Ok(stations);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateStation(int id, [FromBody] EditStation model)
        {
            await _stationService.UpdateStationAsync(id, model);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStation(int id)
        {
            await _stationService.DeleteStationAsync(id);
            return Ok();
        }
    }
}

