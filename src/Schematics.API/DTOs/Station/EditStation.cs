namespace Schematics.API.DTOs.Station
{
    public class EditStation
    {
       
        public string Name { get; set; }
        public decimal? Latitude { get; set; }       
        public decimal? Longitude { get; set; }      
        public decimal? PositionX { get; set; }      
        public decimal? PositionY { get; set; }      
        public bool? IsTransfer { get; set; }
        public string Description { get; set; }
    }
}
