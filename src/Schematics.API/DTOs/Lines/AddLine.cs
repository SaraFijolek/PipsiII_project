using Schematics.API.Data.Entities;

namespace Schematics.API.DTOs.Lines
{
    public class AddLine
    {
        public int SchemaId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int LineNumber { get; set; }
        public string Color { get; set; }
        public bool IsCircular { get; set; }
        public LineCategoryDb LinecaCaterory { get; set; } = null!;
        public ICollection<StationLineDb> StationLines { get; set; } = new List<StationLineDb>();
    }
}
