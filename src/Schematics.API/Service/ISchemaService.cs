using Schematics.API.DTOs.Schemas;
namespace Schematics.API.Service
{
    public interface ISchemaService
    {
        Task AddSchemaAsync(AddSchemaDto model,string ownerId);
        Task<IList<SchemaDto>> GetAllSchemasAsync();
        Task<bool> DeleteSchemaAsync(int schemaId);
        Task<bool> UpdateSchemaAsync(int schemaId, AddSchemaDto model);
        Task<SchemaDto?> GetSchemaByIdAsync(int schemaId);
    }
}
