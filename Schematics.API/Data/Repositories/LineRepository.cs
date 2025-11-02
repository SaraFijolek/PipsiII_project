using Microsoft.EntityFrameworkCore;
using Schematics.API.Data.Entities;

namespace Schematics.API.Data.Repositories
{

    public class LineRepository : ILineRepository
    {

        private readonly ApplicationDbContext _context;

        public LineRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(LineDb line)
        {
            await _context.AddAsync(line);
            await _context.SaveChangesAsync();
        }

        public async Task<IList<LineDb>> GetAllAsync()
        {
            return await _context.Lines.ToListAsync();
        }

        public async Task<IList<LineDb>> GetBySchemaIdAsync(int schemaId)
        {
            return await _context.Lines
                .Where(l => l.SchemaId == schemaId)
                .ToListAsync();
        }

    }
}
