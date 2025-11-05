namespace Schematics.API.DTOs.SharedSchema
{
    public class ShareSchemaRequestDto
    {
        public int SchemaId { get; set; }
        public string SharedWithUserId { get; set; }
        public string AccessLevel { get; set; }
    }
}
