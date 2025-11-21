using Schematics.API.Data.Entities;

namespace Schematics.API.DTOs.StationLines
{
    public class StationlineDto
    {
        public int StationId { get; set; }
        public StationDb Station { get; set; } = null!;

        public int LineId { get; set; }
        public LineDb Line { get; set; } = null!;
    }
}
