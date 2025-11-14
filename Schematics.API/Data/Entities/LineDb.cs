namespace Schematics.API.Data.Entities
{
    public class LineDb
    {
        public int Id { get; set; }
        public int SchemaId { get; set; }
        public int LineCategoryId { get; set; }
        public LineCategoryDb LineCategory { get; set; } = null!;
        public ICollection<StationLineDb> StationLines { get; set; } = new List<StationLineDb>();
        public string Name { get; set; }
        public int LineNumber { get; set; }
        public string Color { get; set; }
        public bool IsCircular { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
