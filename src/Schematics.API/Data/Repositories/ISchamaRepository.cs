using Schematics.API.Data.Entities;
using Schematics.API.DTOs.Schemas;
namespace Schematics.API.Data.Repositories
{
    public interface ISchamaRepository
    {
        Task AddAsync(SchamaDb schama);
        Task<IList<SchamaDb>> GetAllAsync();
        Task<SchamaDb?> GetByIdAsync(int schemaId);
        Task UpdateAsync(SchamaDb schama);
        Task<IList<SchamaDb>> GetAllAsync(bool includeDeleted = false);
    }
}
