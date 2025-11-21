using Microsoft.EntityFrameworkCore;
using Schematics.API.Data;
using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using Schematics.API.DTOs.SharedSchema;
using Schematics.API.Service.Infrastructure;
using System;
namespace Schematics.API.Service
{
    public class SharedSchemaService : ISharedSchemaService
    {
        private readonly ISharedSchemaRepository _repository;
        private readonly ApplicationDbContext _context;

        public SharedSchemaService(ISharedSchemaRepository repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<bool> ShareSchemaAsync(string ownerId, ShareSchemaRequestDto dto)
        {
            var schema = await _context.Schamas
                .FirstOrDefaultAsync(s => s.Id == dto.SchemaId && s.OwnerId == ownerId);

            if (schema == null) return false;

            var existing = await _repository.GetBySchemaAndUserAsync(dto.SchemaId, dto.SharedWithUserId);

            if (existing != null)
            {
                existing.AccessLevel = dto.AccessLevel;
                _repository.Update(existing);
            }
            else
            {
                var share = new SharedSchemaDb
                {
                    OwnerId = ownerId,
                    SharedWithUserId = dto.SharedWithUserId,
                    SchemaId = dto.SchemaId,
                    AccessLevel = dto.AccessLevel
                };
                await _repository.AddAsync(share);
            }

            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<IList<SharedSchemaDto>> GetSharedWithUserAsync(string userId)
        {
            var shares = await _repository.GetSharedWithUserAsync(userId);
            return shares.Select(s => new SharedSchemaDto
            {
                Id = s.Id,
                SchemaId = s.SchemaId,
                SharedWithUserId = s.SharedWithUserId,
                SharedWithEmail = s.SharedWithUser.Email,
                AccessLevel = s.AccessLevel,
                SharedAt = s.SharedAt
            }).ToList();
        }

        public async Task<IList<SharedSchemaDto>> GetSharedByOwnerAsync(string ownerId)
        {
            var shares = await _repository.GetSharedByOwnerAsync(ownerId);
            return shares.Select(s => new SharedSchemaDto
            {
                Id = s.Id,
                SchemaId = s.SchemaId,
                SharedWithUserId = s.SharedWithUserId,
                SharedWithEmail = s.SharedWithUser.Email,
                AccessLevel = s.AccessLevel,
                SharedAt = s.SharedAt
            }).ToList();
        }

        public async Task<bool> RemoveShareAsync(int sharedId, string ownerId)
        {
            var share = await _repository.GetByIdAsync(sharedId);
            if (share == null || share.OwnerId != ownerId)
                return false;

            _repository.Remove(share);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
