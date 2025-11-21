using Microsoft.AspNetCore.Mvc;
using Schematics.API.Service;
using Schematics.API.DTOs.Users;


namespace Schematics.API.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
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
    }
}

