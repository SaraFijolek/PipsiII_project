namespace Schematics.API.DTOs.SharedSchema
{
    public class SharedSchemaDto
    {
        public int Id { get; set; }
        public int SchemaId { get; set; }
        public string SchemaName { get; set; }
        public string SharedWithUserId { get; set; }
        public string SharedWithEmail { get; set; }
        public string AccessLevel { get; set; }
        public DateTime SharedAt { get; set; }
    }
}
