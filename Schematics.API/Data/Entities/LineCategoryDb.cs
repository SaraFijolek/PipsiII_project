
namespace Schematics.API.Data.Entities
{
    public class LineCategoryDb
    {
        public  int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Color { get; set; }
        public ICollection<LineDb> Lines { get; set; } = new List<LineDb>();

    }
}
