using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using Schematics.API.DTOs.Station;

namespace Schematics.API.Service
{
    public class StationService : IStationService
    {
        private readonly IStationRepository _repository;
        private readonly IStationLineRepository _stationLineRepository;

        public StationService(IStationRepository repository, IStationLineRepository stationLineRepository)
        {
            _repository = repository;
            _stationLineRepository = stationLineRepository;
        }

        public async Task AddStationAsync(AddStation model)
        {
            var station = new StationDb
            {
                SchemaId = model.SchemaId,
                Name = model.Name,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                PositionX = model.PositionX,
                PositionY = model.PositionY,
                IsTransfer = model.IsTransfer,
                Description = model.Description
            };

            await _repository.AddAsync(station);

            if (model.LineIds?.Any() == true)
            {
                foreach (var lineId in model.LineIds)
                {
                    station.StationLines.Add(new StationLineDb
                    {
                        StationId = station.Id,
                        LineId = lineId
                    });
                }
                await _stationLineRepository.SaveChangesAsync();
            }
        }

        public async Task<IList<StationDto>> GetStationsBySchemaIdAsync(int schemaId)
        {
            var stations = await _repository.GetAllBySchemaIdAsync(schemaId);

            return stations.Select(s => new StationDto
            {
                Id = s.Id,
                SchemaId = s.SchemaId,
                Name = s.Name,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                PositionX = s.PositionX,
                PositionY = s.PositionY,
                IsTransfer = s.IsTransfer,
                Description = s.Description
            }).ToList();
        }

        public async Task UpdateStationAsync(int id, EditStation model)
        {
            var station = await _repository.GetByIdAsync(id);
            if (station == null)
                throw new KeyNotFoundException("Stacja nie została znaleziona.");

            if (!string.IsNullOrEmpty(model.Name))
                station.Name = model.Name;
            if (model.Latitude.HasValue)
                station.Latitude = model.Latitude.Value;
            if (model.Longitude.HasValue)
                station.Longitude = model.Longitude.Value;
            if (model.PositionX.HasValue)
                station.PositionX = model.PositionX.Value;
            if (model.PositionY.HasValue)
                station.PositionY = model.PositionY.Value;
            if (model.IsTransfer.HasValue)
                station.IsTransfer = model.IsTransfer.Value;
            if (!string.IsNullOrEmpty(model.Description))
                station.Description = model.Description;

            station.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(station);
        }

        public async Task DeleteStationAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}