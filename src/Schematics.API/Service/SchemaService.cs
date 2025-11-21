using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using Schematics.API.DTOs.Schemas;

namespace Schematics.API.Service
{
    public class SchemaService : ISchemaService
    {
        private readonly ISchamaRepository _repository;
        public SchemaService(ISchamaRepository repository)
        {
            _repository = repository;
        }

        public async Task AddSchemaAsync(AddSchemaDto model, string ownerId)
        {
            var newSchema = new SchamaDb
            {
                Name = model.Name,
                Description = model.Description,
                City = model.City,
                Country = model.Country,
                IsPublic = model.IsPublic,
                OwnerId = ownerId
            };
            await _repository.AddAsync(newSchema);
        }

        public async Task<IList<SchemaDto>> GetAllSchemasAsync()
        {
            var schemas = await _repository.GetAllAsync();
            return schemas.Select(schema => new SchemaDto
            {
                Id = schema.Id,
                Name = schema.Name,
                Description = schema.Description,
                City = schema.City,
                Country = schema.Country,
                isPublic = schema.IsPublic
            }).ToList();
        }

        public async Task<SchemaDto?> GetSchemaByIdAsync(int schemaId)
        {
            var s = await _repository.GetByIdAsync(schemaId);
            if (s == null) return null;
            return new SchemaDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                City = s.City,
                Country = s.Country,
                isPublic = s.IsPublic
            };
        }

        public async Task<bool> DeleteSchemaAsync(int schemaId)
        {
            var s = await _repository.GetByIdAsync(schemaId);
            if (s == null) return false;
            s.DeletedAt = DateTime.UtcNow;
            s.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(s);
            return true;
        }

        public async Task<bool> UpdateSchemaAsync(int schemaId, AddSchemaDto model)
        {
            var s = await _repository.GetByIdAsync(schemaId);
            if (s == null) return false;
            s.Name = model.Name;
            s.Description = model.Description;
            s.City = model.City;
            s.Country = model.Country;
            s.IsPublic = model.IsPublic;
            s.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(s);
            return true;
        }
    }
}