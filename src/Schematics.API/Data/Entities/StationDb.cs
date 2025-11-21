namespace Schematics.API.Data.Entities
{
    public class StationDb
    {
        public int Id { get; set; }
        public int SchemaId {  get; set; }
        public ICollection<StationLineDb> StationLines { get; set; } = new List<StationLineDb>();
        public string Name { get; set; }
        public decimal Latitude {  get; set; }
        public decimal Longitude { get; set; }
        public decimal PositionX {  get; set; }
        public decimal PositionY { get; set; }
        public bool IsTransfer { get; set; } = false;
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
       
    }
}
