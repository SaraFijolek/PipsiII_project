using Schematics.API.Data.Entities;

namespace Schematics.API.Data.Repositories
{
    public interface IBookRepository
    {
        Task AddAsync(BookDb bookD);
        Task<IList<BookDb>> GetAllAsync();
    }
}
