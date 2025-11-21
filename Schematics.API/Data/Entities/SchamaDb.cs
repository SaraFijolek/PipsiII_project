namespace Schematics.API.Data.Entities
{
    public class SchamaDb
    {
        public int Id { get; set; }
        public string OwnerId { get; set; } = null!; 
        public User Owner { get; set; } = null!;
        public string Name { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public string  Country { get; set; }
        public bool IsPublic { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }
    }
}
