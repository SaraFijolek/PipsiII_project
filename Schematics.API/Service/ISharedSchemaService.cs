using Schematics.API.DTOs.SharedSchema;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Schematics.API.Service
{
    public interface ISharedSchemaService
    {
        Task<bool> ShareSchemaAsync(string ownerId, ShareSchemaRequestDto dto);
        Task<IList<SharedSchemaDto>> GetSharedWithUserAsync(string userId);
        Task<IList<SharedSchemaDto>> GetSharedByOwnerAsync(string ownerId);
        Task<bool> RemoveShareAsync(int sharedId, string ownerId);
    }
}
