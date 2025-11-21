using Schematics.API.DTOs.Station;


namespace Schematics.API.Service
{
    public interface IStationService
    {
        Task AddStationAsync(AddStation model);
        Task<IList<StationDto>> GetStationsBySchemaIdAsync(int schemaId);
        Task UpdateStationAsync(int id, EditStation model);
        Task DeleteStationAsync(int id);
    }
}
