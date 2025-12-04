using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Schematics.API.Data.Entities;

namespace Schematics.API.Configurations
{
    public class StationLineDbConfiguration : IEntityTypeConfiguration<StationLineDb>
    {
        public void Configure(EntityTypeBuilder<StationLineDb> builder)
        {
            builder.HasKey(sl => new { sl.StationId, sl.LineId });

            builder.HasOne(sl => sl.Station)
                   .WithMany(s => s.StationLines)
                   .HasForeignKey(sl => sl.StationId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sl => sl.Line)
                   .WithMany(l => l.StationLines)
                   .HasForeignKey(sl => sl.LineId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
         

