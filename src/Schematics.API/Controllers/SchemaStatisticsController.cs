using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schematics.API.DTOs.SchemaStatistics;
using Schematics.API.Service;

namespace Schematics.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("statistics")]
    public class SchemaStatisticsController : ControllerBase
    {
        private readonly ISchemaStatisticsService _statisticsService;

        public SchemaStatisticsController(ISchemaStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet("{schemaId}")]
        public async Task<ActionResult<SchemaStatisticsDto>> GetStatisticsBySchemaId(int schemaId)
        {
            var statistics = await _statisticsService.GetStatisticsBySchemaIdAsync(schemaId);

            if (statistics == null)
                return NotFound();

            return Ok(statistics);
        }

        [HttpGet]
        public async Task<ActionResult<IList<SchemaStatisticsDto>>> GetAllStatistics()
        {
            var statistics = await _statisticsService.GetAllStatisticsAsync();
            return Ok(statistics);
        }

        [HttpPut("{schemaId}")]
        public async Task<ActionResult> UpdateStatistics(int schemaId, [FromBody] UpdateStatisticsDto model)
        {
            await _statisticsService.UpdateStatisticsAsync(schemaId, model);
            return Ok();
        }

        [HttpPost("{schemaId}/recalculate")]
        public async Task<ActionResult> RecalculateStatistics(int schemaId)
        {
            await _statisticsService.RecalculateStatisticsAsync(schemaId);
            return Ok();
        }
    }
}

