using Schematics.API.DTOs.Users;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Schematics.API.Service
{
    public interface IUserService
    {
        Task<IList<UserDto>> GetAllUsers();
        Task<UserDto> GetUserById(string id);
        Task<bool> UpdateUser(EditUserDto dto);
        Task<bool> LockUser(string id);
        Task<bool> UnlockUser(string id);
        Task<bool> AddUserToRole(string userId, string roleName);
        Task<bool> RemoveUserFromRole(string userId, string roleName);
    }
}
