
namespace Schematics.API.Data.Entities
{
    public class StationLineDb
    {
        public int StationId { get; set; }
        public StationDb Station { get; set; } = null!;

        public int LineId { get; set; }
        public LineDb Line { get; set; } = null!;
    }
}