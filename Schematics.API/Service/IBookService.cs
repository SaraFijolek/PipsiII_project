using Schematics.API.DTOs.Books;

namespace Schematics.API.Service
{
    public interface IBookService
    {
        Task AddBookAsync(AddBookDto model);
        Task<IList<BookDto>> GetAllBooksAsync();
    }
}
