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

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int lines = 200)
        {
            var content = await _logService.ReadLastLinesAsync(lines);
            return Ok(content);
        }
    }
}
