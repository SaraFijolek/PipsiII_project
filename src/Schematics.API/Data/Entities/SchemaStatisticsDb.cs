namespace Schematics.API.Data.Entities
{
    public class SchemaStatisticsDb
    {
        public int Id { get; set; }
        public int SchemaId { get; set; }
        public SchamaDb Schema { get; set; } = null!;

        
        public int StationCount { get; set; }
        public int LineCount { get; set; }
        public int ConnectionCount { get; set; }

        
        public string? LongestLineName { get; set; }
        public int LongestLineStationCount { get; set; }
        public string? MostConnectedStationName { get; set; }
        public int MostConnectedStationConnectionCount { get; set; }

       
        public double TotalNetworkLength { get; set; } 
        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
