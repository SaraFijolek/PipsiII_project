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
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}
