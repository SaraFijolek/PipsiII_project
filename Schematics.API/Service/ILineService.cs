using Schematics.API.DTOs.Lines;
namespace Schematics.API.Service
{
    public interface ILineService
    {
        Task AddLineAsync(LineDto model);
        Task<IList<LineDto>> GetAllLinesAsync();
        Task<IList<LineDto>> GetLinesBySchemaIdAsync(int schemaId);
    }
}
