using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schematics.API.Data.Entities;
using Schematics.API.DTOs.Books;
using Schematics.API.DTOs.Users;
using Schematics.API.Service.Infrastructure;

namespace Schematics.API.Service
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<User> _roleManager;

        public UserService(RoleManager<User> roleManager,
           UserManager<User> userManager)
        {

            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IList<UserDto>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            var usersDto = new List<UserDto>();

            foreach (var user in users)
            {
                var userDto = new UserDto()
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Email = user.Email,
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,

                };

                usersDto.Add(userDto);
            }

            return usersDto;

        }


    }
}
