using Schematics.API.Data.Entities;

namespace Schematics.API.Data.Repositories
{
    public interface ISchemaStatisticsRepository
    {
        Task<SchemaStatisticsDb?> GetBySchemaIdAsync(int schemaId);
        Task<IList<SchemaStatisticsDb>> GetAllAsync();
        Task AddAsync(SchemaStatisticsDb statistics);
        Task UpdateAsync(SchemaStatisticsDb statistics);
        Task DeleteBySchemaIdAsync(int schemaId);
    }
}
