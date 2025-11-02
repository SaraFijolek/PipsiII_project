using Schematics.API.Data.Entities;
namespace Schematics.API.Data.Repositories
{
    public interface IStationRepository
    {
        Task AddAsync(StationDb station);
        Task<IList<StationDb>> GetAllBySchemaIdAsync(int schemaId);
        Task<StationDb> GetByIdAsync(int id);
        Task UpdateAsync(StationDb station);
        Task DeleteAsync(int id);
    }
}
