namespace Schematics.API.Data.Entities
{
    public class LineDb
    {
        public int Id { get; set; }
        public int SchemaId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int LineNumber { get; set; }
        public string Color { get; set; }
        public bool IsCircular { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
