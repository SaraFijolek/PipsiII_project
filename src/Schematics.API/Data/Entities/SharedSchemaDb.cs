namespace Schematics.API.Data.Entities
{
    public class SharedSchemaDb
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public User Owner { get; set; }

        
        public string SharedWithUserId { get; set; }
        public User SharedWithUser { get; set; }

        
        public int SchemaId { get; set; }
        

        public string AccessLevel { get; set; } = "View";
       

        public DateTime SharedAt { get; set; } = DateTime.UtcNow;
    }
}
