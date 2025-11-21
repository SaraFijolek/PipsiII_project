using Microsoft.EntityFrameworkCore;
using Schematics.API.Data.Entities;

namespace Schematics.API.Data.Repositories
{
    public class SchemaRepository : ISchamaRepository
    {
        
            private readonly ApplicationDbContext _context;

            public SchemaRepository(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task AddAsync(SchamaDb schama)
            {
                await _context.AddAsync(schama);
                await _context.SaveChangesAsync();
            }

            public async Task<IList<SchamaDb>> GetAllAsync()
            {
                return await _context.Schamas.ToListAsync();
            }
        }
    }

