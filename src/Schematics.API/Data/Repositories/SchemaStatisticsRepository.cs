using Microsoft.EntityFrameworkCore;
using Schematics.API.Data.Entities;
using System;

namespace Schematics.API.Data.Repositories
{
    public class SchemaStatisticsRepository : ISchemaStatisticsRepository
    {
        private readonly ApplicationDbContext _context;

        public SchemaStatisticsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SchemaStatisticsDb?> GetBySchemaIdAsync(int schemaId)
        {
            return await _context.SchemaStatistics
                .Include(s => s.Schema)
                .FirstOrDefaultAsync(s => s.SchemaId == schemaId);
        }

        public async Task<IList<SchemaStatisticsDb>> GetAllAsync()
        {
            return await _context.SchemaStatistics
                .Include(s => s.Schema)
                .ToListAsync();
        }

        public async Task AddAsync(SchemaStatisticsDb statistics)
        {
            await _context.SchemaStatistics.AddAsync(statistics);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SchemaStatisticsDb statistics)
        {
            statistics.UpdatedAt = DateTime.UtcNow;
            _context.SchemaStatistics.Update(statistics);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBySchemaIdAsync(int schemaId)
        {
            var statistics = await GetBySchemaIdAsync(schemaId);
            if (statistics != null)
            {
                _context.SchemaStatistics.Remove(statistics);
                await _context.SaveChangesAsync();
            }
        }
    }
}