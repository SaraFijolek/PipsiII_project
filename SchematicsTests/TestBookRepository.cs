using Microsoft.EntityFrameworkCore;
using Schematics.API.Data;
using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchematicsTests
{
    [TestClass]
    public class TestBookRepository
    {
        private ApplicationDbContext _context;
        private BookRepository _bookRepository;

        [TestInitialize]
        public void Setup()
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb" + Guid.NewGuid()).Options;
            _context = new ApplicationDbContext(options);
            _bookRepository =  new BookRepository(_context);
        }

        [TestMethod]
        public async Task TestGetAllAsync()
        {
            BookDb bookDb = new BookDb { Id = 0, Name = "test" };
            await _bookRepository.AddAsync(bookDb);
            IList<BookDb> books = await _bookRepository.GetAllAsync();
            Assert.AreEqual(bookDb.Id, books.First().Id);
        }
    }
}

//public async Task<IList<BookDb>> GetAllAsync()
//{
//    return await _context.Books.ToListAsync();
//}

