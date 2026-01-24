using Microsoft.EntityFrameworkCore;
using Schematics.API.Data;
using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

public class BookRepositoryTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"BooksDb_{System.Guid.NewGuid()}")
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task AddAsync_AddsBookToDatabase()
    {
        var context = GetInMemoryDbContext();
        var repo = new BookRepository(context);

        var book = new BookDb { Name = "Test Book" };

        await repo.AddAsync(book);

        var booksInDb = await context.Books.ToListAsync();
        Assert.Single(booksInDb);
        Assert.Equal("Test Book", booksInDb.First().Name);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllBooks()
    {
        var context = GetInMemoryDbContext();
        await context.Books.AddRangeAsync(new List<BookDb>
        {
            new BookDb { Name = "Book 1" },
            new BookDb { Name = "Book 2" }
        });
        await context.SaveChangesAsync();

        var repo = new BookRepository(context);

        var result = await repo.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, b => b.Name == "Book 1");
        Assert.Contains(result, b => b.Name == "Book 2");
    }
}

