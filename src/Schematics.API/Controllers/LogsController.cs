using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schematics.API.Service;

namespace Schematics.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("admin/logs")]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogsController(ILogService logService)
        {
            _logService = logService;
        }


        [HttpGet("{date}")]
        public async Task<IActionResult> GetLogs(string date, [FromQuery] int lines = 200)
        {
            
            if (!DateTime.TryParseExact(date, "yyyyMMdd", null,
                System.Globalization.DateTimeStyles.None, out _))
            {
                return BadRequest("Invalid date format. Use yyyyMMdd (e.g., 20250124)");
            }

            var logs = await _logService.ReadLastLinesAsync(date, lines);
            return Ok(logs);


        }
    }
}
