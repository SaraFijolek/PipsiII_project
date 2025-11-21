using Microsoft.EntityFrameworkCore;
using Schematics.API.Data.Entities;
using System;

namespace Schematics.API.Data.Repositories
{
    public class SharedSchemaRepository : ISharedSchemaRepository
    {
        private readonly ApplicationDbContext _context;

        public SharedSchemaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SharedSchemaDb?> GetByIdAsync(int id)
        {
            return await _context.SharedSchemas
                .Include(s => s.SchemaId)
                .Include(s => s.SharedWithUser)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<SharedSchemaDb?> GetBySchemaAndUserAsync(int schemaId, string sharedWithUserId)
        {
            return await _context.SharedSchemas
                .FirstOrDefaultAsync(s =>
                    s.SchemaId == schemaId &&
                    s.SharedWithUserId == sharedWithUserId);
        }

        public async Task<IEnumerable<SharedSchemaDb>> GetSharedWithUserAsync(string userId)
        {
            return await _context.SharedSchemas
                .Include(s => s.SchemaId)
                .Include(s => s.SharedWithUser)
                .Where(s => s.SharedWithUserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<SharedSchemaDb>> GetSharedByOwnerAsync(string ownerId)
        {
            return await _context.SharedSchemas
                .Include(s => s.SchemaId)
                .Include(s => s.SharedWithUser)
                .Where(s => s.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task AddAsync(SharedSchemaDb sharedSchema)
        {
            await _context.SharedSchemas.AddAsync(sharedSchema);
        }

        public void Update(SharedSchemaDb sharedSchema)
        {
            _context.SharedSchemas.Update(sharedSchema);
        }

        public void Remove(SharedSchemaDb sharedSchema)
        {
            _context.SharedSchemas.Remove(sharedSchema);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
