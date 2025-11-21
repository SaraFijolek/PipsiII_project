using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Schematics.API.Data.Entities;

namespace Schematics.API.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public DbSet<BookDb> Books { get; set; }
    public DbSet<SchamaDb> Schamas { get; set; }
    public DbSet<LineDb> Lines { get; set; }
    public DbSet<StationDb> Stations { get; set; }
    public DbSet<SharedSchemaDb> SharedSchemas { get; set; }
    public DbSet<LineCategoryDb> LineCategories { get; set; }
    public DbSet<StationLineDb> StationLines { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

       
        modelBuilder.Entity<StationLineDb>()
            .HasKey(sl => new { sl.StationId, sl.LineId });

       
        modelBuilder.Entity<StationLineDb>()
            .HasOne(sl => sl.Station)
            .WithMany(s => s.StationLines)
            .HasForeignKey(sl => sl.StationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StationLineDb>()
            .HasOne(sl => sl.Line)
            .WithMany(l => l.StationLines)
            .HasForeignKey(sl => sl.LineId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
