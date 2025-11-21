using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schematics.API.Data.Entities;
using Schematics.API.DTOs.SharedSchema;
using Schematics.API.Service;
using System.Security.Claims;

namespace Schematics.API.Controllers
{
    [ApiController]
    [Route("sharedschema")]
    [Authorize]
    public class SharedSchemaController : ControllerBase
    {
        private readonly ISharedSchemaService _sharedService;

        public SharedSchemaController(ISharedSchemaService sharedService)
        {
            _sharedService = sharedService;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpPost("share")]
        public async Task<IActionResult> ShareSchema([FromBody] ShareSchemaRequestDto dto)
        {
            var ownerId = GetUserId();
            var result = await _sharedService.ShareSchemaAsync(ownerId, dto);

            if (!result)
                return BadRequest();

            return Ok();
        }

        [HttpGet("shared-by-me")]
        public async Task<IActionResult> GetSharedByMe()
        {
            var ownerId = GetUserId();
            var result = await _sharedService.GetSharedByOwnerAsync(ownerId);
            return Ok(result);
        }

        [HttpGet("shared-with-me")]
        public async Task<IActionResult> GetSharedWithMe()
        {
            var userId = GetUserId();
            var result = await _sharedService.GetSharedWithUserAsync(userId);
            return Ok(result);
        }

        [HttpDelete("{sharedId}")]
        public async Task<IActionResult> RemoveShare(int sharedId)
        {
            var ownerId = GetUserId();
            var result = await _sharedService.RemoveShareAsync(sharedId, ownerId);

            if (!result)
                return NotFound();

            return Ok();
        }
    }
}
