using Moq;
using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using Schematics.API.DTOs.Books;
using Schematics.API.Service;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

public class BookServiceTests
{
    [Fact]
    public async Task AddBookAsync_CallsRepositoryAdd()
    {
        var mockRepo = new Mock<IBookRepository>();
        var service = new BookService(mockRepo.Object);
        var dto = new AddBookDto { Name = "Test Book" };

        await service.AddBookAsync(dto);

        mockRepo.Verify(r => r.AddAsync(It.Is<BookDb>(b => b.Name == "Test Book")), Times.Once);
    }

    [Fact]
    public async Task GetAllBooksAsync_ReturnsMappedBooks()
    {
        var mockRepo = new Mock<IBookRepository>();
        mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<BookDb>
        {
            new BookDb { Id = 1, Name = "Book1" },
            new BookDb { Id = 2, Name = "Book2" }
        });

        var service = new BookService(mockRepo.Object);

        var result = await service.GetAllBooksAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Book1", result[0].Name);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("Book2", result[1].Name);
        Assert.Equal(2, result[1].Id);
    }

    [Fact]
    public async Task GetAllBooksAsync_ReturnsEmptyList_WhenNoBooks()
    {
        var mockRepo = new Mock<IBookRepository>();
        mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<BookDb>());

        var service = new BookService(mockRepo.Object);

        var result = await service.GetAllBooksAsync();

        Assert.Empty(result);
    }
}

