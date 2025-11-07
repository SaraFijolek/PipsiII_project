using FluentAssertions;
using Moq;
using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using Schematics.API.DTOs.Books;
using Schematics.API.Service;

namespace SchematicsTests
{
    [TestClass]
    public sealed class TestBookService
    {
        private Mock<IBookRepository> _bookRepository;
        private BookService _bookService;

        [TestInitialize]
        public void Setup()
        {
            _bookRepository = new Mock<IBookRepository>();
            _bookService = new BookService(_bookRepository.Object);
        }

        [TestMethod]
        public async Task TestAddBookAsync()
        {
            AddBookDto book = new AddBookDto { Name = "test" };
            await _bookService.AddBookAsync(book);

            Func<Task> act = async () => await _bookService.AddBookAsync(null);
            await act.Should().ThrowAsync<NullReferenceException>();
        }

        [TestMethod]
        public async Task TestGetAllBooksAsync()
        {
            List<BookDb> books = new List<BookDb>();
            books.Add(new BookDb { Id = 0, Name = "test" });
            books.Add(new BookDb { Id = 1, Name = "test2" });
            _bookRepository.Setup(repository => repository.GetAllAsync()).ReturnsAsync(books);

            IList<BookDto> result = await _bookService.GetAllBooksAsync();
           result.Should().HaveCount(2);
        }
    }
}
//public async Task<IList<BookDto>> GetAllBooksAsync()
//{
//    var books = await _repository.GetAllAsync();

//    return books.Select(book => new BookDto
//    {
//        Id = book.Id,
//        Name = book.Name
//    }).ToList();
//}