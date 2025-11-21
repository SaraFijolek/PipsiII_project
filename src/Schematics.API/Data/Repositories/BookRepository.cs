using Microsoft.EntityFrameworkCore;
using Schematics.API.Data.Entities;

namespace Schematics.API.Data.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(BookDb bookD)
        {
            await _context.AddAsync(bookD);
            await _context.SaveChangesAsync();
        }

        public async Task<IList<BookDb>> GetAllAsync()
        {
            return await _context.Books.ToListAsync();
        }
    }
}
