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
    }
}
