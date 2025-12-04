using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Schematics.API.Data.Entities;

public class SchemaStatisticsDbConfiguration : IEntityTypeConfiguration<SchemaStatisticsDb>
{
    public void Configure(EntityTypeBuilder<SchemaStatisticsDb> builder)
    {
        builder.HasOne(s => s.Schema)
               .WithOne()
               .HasForeignKey<SchemaStatisticsDb>(s => s.SchemaId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
