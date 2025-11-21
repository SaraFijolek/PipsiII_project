using Schematics.API.Data.Entities;

namespace Schematics.API.DTOs.LineCategories
{
    public class EditLineCategory
    {
        public string Name { get; set; } = null!;
        public string Color { get; set; }
        public ICollection<LineDb> Lines { get; set; } = new List<LineDb>();
    }
}
