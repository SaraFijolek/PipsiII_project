
using Microsoft.EntityFrameworkCore;
using Schematics.API.Data.Entities;

namespace Schematics.API.Data.Repositories
{
    public class StationLineRepository : IStationLineRepository
    {
        private readonly ApplicationDbContext _context;

        public StationLineRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(StationLineDb stationLine)
        {
            await _context.StationLines.AddAsync(stationLine);
        }

        public async Task<IList<StationLineDb>> GetByStationIdAsync(int stationId)
        {
            return await _context.StationLines
                .Include(sl => sl.Line)
                .Include(sl => sl.Station)
                .Where(sl => sl.StationId == stationId)
                .ToListAsync();
        }

        public async Task<IList<StationLineDb>> GetByLineIdAsync(int lineId)
        {
            return await _context.StationLines
                .Include(sl => sl.Line)
                .Include(sl => sl.Station)
                .Where(sl => sl.LineId == lineId)
                .ToListAsync();
        }

        public async Task RemoveAsync(int stationId, int lineId)
        {
            var stationLine = await _context.StationLines
                .FirstOrDefaultAsync(sl => sl.StationId == stationId && sl.LineId == lineId);

            if (stationLine != null)
            {
                _context.StationLines.Remove(stationLine);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}