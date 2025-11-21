
namespace Schematics.API.DTOs.SchemaStatistics
{
    public class SchemaStatisticsDto
    {
        public int Id { get; set; }
        public int SchemaId { get; set; }
        public string SchemaName { get; set; }

        
        public int StationCount { get; set; }
        public int LineCount { get; set; }
        public int ConnectionCount { get; set; }

       
        public string? LongestLineName { get; set; }
        public int LongestLineStationCount { get; set; }
        public string? MostConnectedStationName { get; set; }
        public int MostConnectedStationConnectionCount { get; set; }

        
        public double TotalNetworkLength { get; set; }
        public DateTime CalculatedAt { get; set; }
    }
}
