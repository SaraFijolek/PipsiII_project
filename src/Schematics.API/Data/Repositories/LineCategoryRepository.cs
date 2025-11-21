
using Microsoft.EntityFrameworkCore;
using Schematics.API.Data.Entities;

namespace Schematics.API.Data.Repositories
{
    public class LineCategoryRepository : ILineCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public LineCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LineCategoryDb?> GetByIdAsync(int id)
        {
            return await _context.LineCategories
                .Include(c => c.Lines)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IList<LineCategoryDb>> GetAllAsync()
        {
            return await _context.LineCategories
                .Include(c => c.Lines)
                .ToListAsync();
        }

        public async Task AddAsync(LineCategoryDb category)
        {
            await _context.LineCategories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LineCategoryDb category)
        {
            _context.LineCategories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.LineCategories.FindAsync(id);
            if (category != null)
            {
                _context.LineCategories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}