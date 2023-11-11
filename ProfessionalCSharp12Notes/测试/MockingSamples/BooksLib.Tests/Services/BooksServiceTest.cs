using BooksLib.Models;
using BooksLib.Repositories;
using BooksLib.Services;
using Moq;
using Xunit.Abstractions;

namespace BooksLib.Tests.Services;

public class BooksServiceTest : TestBase, IDisposable
{
    private const string TestTitle = "Test Title";
    private const string UpdatedTestTitle = "Updated Test Title";
    private const string APublisher = "A Publisher";
    private readonly Guid currentGuid = Guid.NewGuid();

    private readonly BooksService _booksService;

    private Book _newBook = new()
    {
        BookId = 0,
        Title = TestTitle,
        Publisher = APublisher
    };

    private Book _expectedBook = new()
    {
        BookId = 1,
        Title = TestTitle,
        Publisher = APublisher
    };

    private Book _notInRepositoryBook = new()
    {
        BookId = 42,
        Title = TestTitle,
        Publisher = APublisher
    };

    private Book _updatedBook = new()
    {
        BookId = 1,
        Title = UpdatedTestTitle,
        Publisher = APublisher
    };

    public BooksServiceTest(ITestOutputHelper tempOutput) : base(tempOutput)
    {
        Mock<IBooksRepository> mock = new();
        mock.Setup(repository => repository.AddAsync(_newBook)).ReturnsAsync(_expectedBook);
        mock.Setup(repository => repository.UpdateAsync(_notInRepositoryBook)).ReturnsAsync(null as Book);
        mock.Setup(repository => repository.UpdateAsync(_updatedBook)).ReturnsAsync(_updatedBook);

        _booksService = new(mock.Object);
        Output.WriteLine($"【{currentGuid}】初始化完成");
    }

    [Fact]
    public async Task GetBook_ReturnsExistingBook()
    {
        // arrange
        await _booksService.AddOrUpdateBookAsync(_newBook);
        // act
        Book? actualBook = _booksService.GetBook(1);
        // assert
        Assert.Equal(_expectedBook, actualBook);
    }

    [Fact]
    public void GetBook_ReturnsNullForNotExistingBook()
    {
        // arrange in constructor
        // act
        Book? actualBook = _booksService.GetBook(42);
        // assert
        Assert.Null(actualBook);
    }

    [Fact]
    public async Task AddOrUpdateBookAsync_ThrowsForNull()
    {
        // arrange
        Book? nullBook = null;
        // act and assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _booksService.AddOrUpdateBookAsync(nullBook!));
    }

    [Fact]
    public async Task AddOrUpdateBook_AddedBookReturnsFromRepository()
    {
        // arrange in constructor

        // act
        Book actualAdded = await _booksService.AddOrUpdateBookAsync(_newBook);

        // assert
        Assert.Equal(_expectedBook, actualAdded);
        Assert.Contains(_expectedBook, _booksService.Books);
    }

    [Fact]
    public async Task AddOrUpdateBook_UpdateNotExistingBookThrows()
    {
        // arrange in constructor

        // act and assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _booksService.AddOrUpdateBookAsync(_notInRepositoryBook));
    }

    [Fact]
    public async Task AddOrUpdateBook_UpdateBook()
    {
        // arrange
        await _booksService.AddOrUpdateBookAsync(_newBook);

        // act
        Book updatedBook = await _booksService.AddOrUpdateBookAsync(_updatedBook);

        // assert
        Assert.Equal(_updatedBook, updatedBook);
        Assert.Contains(_updatedBook, _booksService.Books);
    }

    public void Dispose()
    {
        Output.WriteLine($"【{currentGuid}】已被释放");
    }
}
