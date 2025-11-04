using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Schematics.API.Data.Entities;
using Schematics.API.DTOs.Users;

namespace Schematics.API.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IList<UserDto>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return users.Select(user => new UserDto
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                IsLockout = user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.Now
            }).ToList();
        }

        public async Task<UserDto> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                IsLockout = user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.Now
            };
        }

        public async Task<bool> UpdateUser(EditUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.Id);
            if (user == null) return false;

            user.Firstname = dto.Firstname;
            user.Lastname = dto.Lastname;
            user.Email = dto.Email;
            user.UserName = dto.Name;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> LockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

           
            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(1);
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> UnlockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            user.LockoutEnd = null;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> AddUserToRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

           
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result.Succeeded;
        }

        public async Task<bool> RemoveUserFromRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            return result.Succeeded;
        }
    }
}
