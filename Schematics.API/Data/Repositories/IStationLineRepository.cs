
using Schematics.API.Data.Entities;

namespace Schematics.API.Data.Repositories
{
    public interface IStationLineRepository
    {
        Task AddAsync(StationLineDb stationLine);
        Task<IList<StationLineDb>> GetByStationIdAsync(int stationId);
        Task<IList<StationLineDb>> GetByLineIdAsync(int lineId);
        Task RemoveAsync(int stationId, int lineId);
        Task SaveChangesAsync();
    }
}
