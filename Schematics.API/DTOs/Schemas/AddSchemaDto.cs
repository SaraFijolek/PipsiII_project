namespace Schematics.API.DTOs.Schemas
{
    public class AddSchemaDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public bool  IsPublic { get; set; }
    }
}
