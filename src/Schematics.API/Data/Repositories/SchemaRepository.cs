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


        public async Task<SchamaDb?> GetByIdAsync(int schemaId)
        {
            return await _context.Schamas
                .FirstOrDefaultAsync(s => s.Id == schemaId);
        }
        public async Task UpdateAsync(SchamaDb schama)
        {
            _context.Schamas.Update(schama);
            await _context.SaveChangesAsync();
        }
        public async Task<IList<SchamaDb>> GetAllAsync(bool includeDeleted = false)
        {
            if (includeDeleted)
                return await _context.Schamas.ToListAsync();

            return await _context.Schamas.Where(s => s.DeletedAt == null).ToListAsync();
        }
    }
}