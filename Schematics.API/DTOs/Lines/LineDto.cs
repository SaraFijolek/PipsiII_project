namespace Schematics.API.DTOs.Lines
{
    public class LineDto
    {
        public int Id { get; set; }
        public int SchemaId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int LineNumber { get; set; }
        public string Color { get; set; }
        public bool IsCircular { get; set; }
    }
}
