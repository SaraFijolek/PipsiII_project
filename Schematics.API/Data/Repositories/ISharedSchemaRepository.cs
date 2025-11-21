using Schematics.API.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Schematics.API.Data.Repositories
{
    public interface ISharedSchemaRepository
    {
        Task<SharedSchemaDb?> GetByIdAsync(int id);
        Task<SharedSchemaDb?> GetBySchemaAndUserAsync(int schemaId, string sharedWithUserId);
        Task<IEnumerable<SharedSchemaDb>> GetSharedWithUserAsync(string userId);
        Task<IEnumerable<SharedSchemaDb>> GetSharedByOwnerAsync(string ownerId);
        Task AddAsync(SharedSchemaDb sharedSchema);
        void Update(SharedSchemaDb sharedSchema);
        void Remove(SharedSchemaDb sharedSchema);
        Task SaveChangesAsync();
    }
}
