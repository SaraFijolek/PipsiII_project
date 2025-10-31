using Microsoft.EntityFrameworkCore;
using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using Schematics.API.DTOs.Books;

namespace Schematics.API.Service
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _repository;

        public BookService(IBookRepository repository)
        {
            _repository = repository;
        }

        public async Task AddBookAsync(AddBookDto model)
        {
            var newBook = new BookDb()
            {
                Name = model.Name
            };

            await _repository.AddAsync(newBook);
        }

        public async Task<IList<BookDto>> GetAllBooksAsync()
        {
            var books = await _repository.GetAllAsync();

            return books.Select(book => new BookDto
            {
                Id = book.Id,
                Name = book.Name
            }).ToList();
        }
    }
}
