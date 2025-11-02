namespace Schematics.API.DTOs.Station
{
    public class AddStation
    {
        public int SchemaId { get; set; }
        public string Name { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal PositionX { get; set; }
        public decimal PositionY { get; set; }
        public bool IsTransfer { get; set; } = false;
        public string Description { get; set; }
    }
}
