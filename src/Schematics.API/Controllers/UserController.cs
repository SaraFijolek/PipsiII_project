using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schematics.API.DTOs.Schemas;
using Schematics.API.DTOs.Users;
using Schematics.API.Service;


namespace Schematics.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISchemaService _schemaService;
        private readonly ILogService _logService;

        public UserController(IUserService userService,ISchemaService schemaService,ILogService logService)
        {
            _userService = userService;
            _schemaService = schemaService;
            _logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] EditUserDto dto)
        {
            var result = await _userService.UpdateUser(dto);
            if (!result)
                return BadRequest();

            return Ok();
        }

        [HttpPut("lock/{id}")]
        public async Task<IActionResult> LockUser(string id)
        {
            var result = await _userService.LockUser(id);
            if (!result)
                return NotFound();

            return Ok();
        }

        [HttpPut("unlock/{id}")]
        public async Task<IActionResult> UnlockUser(string id)
        {
            var result = await _userService.UnlockUser(id);
            if (!result)
                return NotFound();

            return Ok();
        }

        [HttpPost("{id}/role/{roleName}")]
        public async Task<IActionResult> AddRole(string id, string roleName)
        {
            var result = await _userService.AddUserToRole(id, roleName);
            if (!result)
                return BadRequest();

            return Ok();
        }

        [HttpDelete("{id}/role/{roleName}")]
        public async Task<IActionResult> RemoveRole(string id, string roleName)
        {
            var result = await _userService.RemoveUserFromRole(id, roleName);
            if (!result)
                return BadRequest();

            return Ok();
        }
        [HttpGet("schemas")]
        public async Task<IActionResult> GetAllSchemas() => Ok(await _schemaService.GetAllSchemasAsync());


        [HttpGet("schemas/{id}")]
        public async Task<IActionResult> GetSchemaById(int id)
        {
            var schema = await _schemaService.GetSchemaByIdAsync(id);
            if (schema == null) return NotFound();
            return Ok(schema);
        }


        [HttpPut("schemas/{id}")]
        public async Task<IActionResult> UpdateSchema(int id, [FromBody] AddSchemaDto dto)
        {
            var result = await _schemaService.UpdateSchemaAsync(id, dto);
            if (!result) return NotFound();
            return Ok();
        }


        [HttpDelete("schemas/{id}")]
        public async Task<IActionResult> DeleteSchema(int id)
        {
            var result = await _schemaService.DeleteSchemaAsync(id);
            if (!result) return NotFound();
            return Ok();
        }


        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs([FromQuery] int lines = 200)
        {
            var logs = await _logService.ReadLastLinesAsync(lines);
            return Ok(logs);
        }
    }
}

