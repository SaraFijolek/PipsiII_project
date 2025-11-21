using Schematics.API.Data.Entities;

namespace Schematics.API.Data.Repositories
{
    public interface ILineRepository
    {
        Task AddAsync(LineDb line);
        Task<IList<LineDb>> GetAllAsync();
        Task<IList<LineDb>> GetBySchemaIdAsync(int schemaId);

    }
}
