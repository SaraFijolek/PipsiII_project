using Schematics.API.Data.Entities;
namespace Schematics.API.Data.Repositories
{
    public interface ISchamaRepository
    {
        Task AddAsync(SchamaDb schama);
        Task<IList<SchamaDb>> GetAllAsync();
        
    }
}
