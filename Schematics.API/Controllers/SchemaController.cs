using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schematics.API.Data;
using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using Schematics.API.DTOs.Schemas;
using Schematics.API.Service;
using Schematics.API.Service.Infrastructure;




namespace Schematics.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("schemas")]
    public class SchemaController : ControllerBase
    {
        private readonly ISchemaService _schemaService;
        private readonly UserManager<User> _userManager;

        public SchemaController(ISchemaService schemaService, UserManager<User> userManager)
        {
            _schemaService = schemaService;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<ActionResult> AddSchema([FromBody] AddSchemaDto model)
        {
            var user = await _userManager.GetUserAsync(User);
            await _schemaService.AddSchemaAsync(model, user.Id);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IList<SchemaDto>>> GetAllSchemas()
        {
            var schemas = await _schemaService.GetAllSchemasAsync();
            return Ok(schemas);
        }
    }
}

