using Schematics.API.DTOs.Schemas;
namespace Schematics.API.Service
{
    public interface ISchemaService
    {
        Task AddSchemaAsync(AddSchemaDto model,string ownerId);
        Task<IList<SchemaDto>> GetAllSchemasAsync();
    }
}
