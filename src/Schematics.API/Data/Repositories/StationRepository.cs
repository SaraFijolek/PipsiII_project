using Microsoft.EntityFrameworkCore;
using Schematics.API.Data.Entities;
using static System.Collections.Specialized.BitVector32;

namespace Schematics.API.Data.Repositories
{
    public class StationRepository : IStationRepository
    {
        private readonly ApplicationDbContext _context;

        public StationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(StationDb station)
        {
            await _context.Stations.AddAsync(station);
            await _context.SaveChangesAsync();
        }

        public async Task<IList<StationDb>> GetAllBySchemaIdAsync(int schemaId)
        {
            return await _context.Stations
                .Where(s => s.SchemaId == schemaId)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<StationDb?> GetByIdAsync(int id)
        {
            return await _context.Stations.FirstOrDefaultAsync(s => s.Id == id);
            
        }

        public async Task UpdateAsync(StationDb station)
        {
            _context.Stations.Update(station);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var station = await _context.Stations.FindAsync(id);
            if (station != null)
            {
                _context.Stations.Remove(station);
                await _context.SaveChangesAsync();
            }
        }
    }
}

