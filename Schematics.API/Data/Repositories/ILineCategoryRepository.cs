using Schematics.API.Data.Entities;
using Schematics.API.DTOs.LineCategories;

namespace Schematics.API.Data.Repositories
{
    public interface ILineCategoryRepository
    {
        Task<LineCategoryDb?> GetByIdAsync(int id);
        Task<IList<LineCategoryDb>> GetAllAsync();
        Task AddAsync(LineCategoryDb category);
        Task UpdateAsync(LineCategoryDb category);
        Task DeleteAsync(int id);

    }
}
