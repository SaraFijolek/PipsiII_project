namespace Schematics.API.DTOs.Schemas
{
    public class SchemaDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public bool isPublic { get; set; }
    }
}
