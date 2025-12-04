using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Schematics.API.Data.Entities;

namespace Schematics.API.Configurations
{
    public class SharedSchemaDbConfiguration : IEntityTypeConfiguration<SharedSchemaDb>
    {
       public void Configure(EntityTypeBuilder<SharedSchemaDb> builder)
        {
            builder.HasOne(sl => sl.Owner)
               .WithMany(l => l.OwnSchemas)
               .HasForeignKey(sl => sl.OwnerId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(sl => sl.SharedWithUser)
                   .WithMany(l => l.SharedSchemas)
                   .HasForeignKey(sl => sl.SharedWithUserId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
