using Schematics.API.Data.Entities;

namespace Schematics.API.DTOs.LineCategories
{
    public class LineCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Color { get; set; }
        public ICollection<LineDb> Lines { get; set; } = new List<LineDb>();
    }
}
