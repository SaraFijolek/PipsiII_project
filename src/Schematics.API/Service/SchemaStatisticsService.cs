using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using Schematics.API.DTOs.SchemaStatistics;

namespace Schematics.API.Service
{
    public class SchemaStatisticsService : ISchemaStatisticsService
    {
        private readonly ISchemaStatisticsRepository _statisticsRepository;
        private readonly ISchamaRepository _schemaRepository;

        public SchemaStatisticsService(
            ISchemaStatisticsRepository statisticsRepository,
            ISchamaRepository schemaRepository)
        {
            _statisticsRepository = statisticsRepository;
            _schemaRepository = schemaRepository;
        }

        public async Task<SchemaStatisticsDto?> GetStatisticsBySchemaIdAsync(int schemaId)
        {
            var statistics = await _statisticsRepository.GetBySchemaIdAsync(schemaId);

            if (statistics == null)
                return null;

            return MapToDto(statistics);
        }

        public async Task<IList<SchemaStatisticsDto>> GetAllStatisticsAsync()
        {
            var statisticsList = await _statisticsRepository.GetAllAsync();
            return statisticsList.Select(MapToDto).ToList();
        }

        public async Task UpdateStatisticsAsync(int schemaId, UpdateStatisticsDto model)
        {
            var statistics = await _statisticsRepository.GetBySchemaIdAsync(schemaId);

            if (statistics == null)
            {
                
                statistics = new SchemaStatisticsDb
                {
                    SchemaId = schemaId
                };
                UpdateStatisticsFromDto(statistics, model);
                await _statisticsRepository.AddAsync(statistics);
            }
            else
            {
                
                UpdateStatisticsFromDto(statistics, model);
                await _statisticsRepository.UpdateAsync(statistics);
            }
        }

        public async Task RecalculateStatisticsAsync(int schemaId)
        {
            

            var schema = await _schemaRepository.GetByIdAsync(schemaId);
            if (schema == null)
                throw new Exception("Schema not found");

            
            var updateDto = new UpdateStatisticsDto
            {
                StationCount = 0, 
                LineCount = 0, 
                ConnectionCount = 0, 
                TotalNetworkLength = 0.0 
            };

            await UpdateStatisticsAsync(schemaId, updateDto);
        }

        private SchemaStatisticsDto MapToDto(SchemaStatisticsDb statistics)
        {
            return new SchemaStatisticsDto
            {
                Id = statistics.Id,
                SchemaId = statistics.SchemaId,
                SchemaName = statistics.Schema.Name,
                StationCount = statistics.StationCount,
                LineCount = statistics.LineCount,
                ConnectionCount = statistics.ConnectionCount,
                LongestLineName = statistics.LongestLineName,
                LongestLineStationCount = statistics.LongestLineStationCount,
                MostConnectedStationName = statistics.MostConnectedStationName,
                MostConnectedStationConnectionCount = statistics.MostConnectedStationConnectionCount,
                TotalNetworkLength = statistics.TotalNetworkLength,
                CalculatedAt = statistics.CalculatedAt
            };
        }

        private void UpdateStatisticsFromDto(SchemaStatisticsDb statistics, UpdateStatisticsDto dto)
        {
            statistics.StationCount = dto.StationCount;
            statistics.LineCount = dto.LineCount;
            statistics.ConnectionCount = dto.ConnectionCount;
            statistics.LongestLineName = dto.LongestLineName;
            statistics.LongestLineStationCount = dto.LongestLineStationCount;
            statistics.MostConnectedStationName = dto.MostConnectedStationName;
            statistics.MostConnectedStationConnectionCount = dto.MostConnectedStationConnectionCount;
            statistics.TotalNetworkLength = dto.TotalNetworkLength;
            statistics.CalculatedAt = DateTime.UtcNow;
        }
    }
}