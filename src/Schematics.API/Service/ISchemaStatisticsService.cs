using Schematics.API.DTOs.SchemaStatistics;

namespace Schematics.API.Service
{
    public interface ISchemaStatisticsService
    {
        Task<SchemaStatisticsDto?> GetStatisticsBySchemaIdAsync(int schemaId);
        Task<IList<SchemaStatisticsDto>> GetAllStatisticsAsync();
        Task UpdateStatisticsAsync(int schemaId, UpdateStatisticsDto model);
        Task RecalculateStatisticsAsync(int schemaId);
    }
}
