using BookReview.Controllers;
using BookReview.Dto;
using BookReview.Interfaces;
using BookReview.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookReview.Tests.Controllers;

public class BooksControllerTests
{
    private readonly Mock<IBookRepository> _repo = new();

    private BooksController CreateController() => new(_repo.Object);

    [Fact]
    public async Task GetBooks_ReturnsOk_WithAllBooks()
    {
        _repo.Setup(r => r.GetAllBooksAsync()).ReturnsAsync(new List<Book>
        {
            new() { Id = 1, Title = "A" },
            new() { Id = 2, Title = "B" }
        });

        var result = await CreateController().GetBooks();

        var ok = Assert.IsType<OkObjectResult>(result);
        var dtos = Assert.IsAssignableFrom<IEnumerable<BookDto>>(ok.Value);
        Assert.Equal(2, dtos.Count());
    }

    [Fact]
    public async Task GetBook_ReturnsNotFound_WhenMissing()
    {
        _repo.Setup(r => r.GetBookAsync(99)).ReturnsAsync((Book?)null);

        var result = await CreateController().GetBook(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetBook_ReturnsOk_WhenFound()
    {
        _repo.Setup(r => r.GetBookAsync(1)).ReturnsAsync(new Book { Id = 1, Title = "A" });

        var result = await CreateController().GetBook(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<BookDto>(ok.Value);
        Assert.Equal(1, dto.Id);
    }

    [Fact]
    public async Task GetBookRating_ReturnsNotFound_WhenBookMissing()
    {
        _repo.Setup(r => r.BookExistsAsync(99)).ReturnsAsync(false);

        var result = await CreateController().GetBookRating(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetBookRating_ReturnsOk_WhenBookExists()
    {
        _repo.Setup(r => r.BookExistsAsync(1)).ReturnsAsync(true);
        _repo.Setup(r => r.GetBookRatingAsync(1)).ReturnsAsync(4.5m);

        var result = await CreateController().GetBookRating(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(4.5m, ok.Value);
    }

    [Fact]
    public async Task UpdateBook_ReturnsBadRequest_WhenIdMismatch()
    {
        var result = await CreateController().UpdateBook(1, new BookDto { Id = 2, Title = "X" });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateBook_ReturnsNotFound_WhenMissing()
    {
        _repo.Setup(r => r.GetBookAsync(1)).ReturnsAsync((Book?)null);

        var result = await CreateController().UpdateBook(1, new BookDto { Id = 1, Title = "X" });

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task UpdateBook_ReturnsNoContent_OnSuccess()
    {
        _repo.Setup(r => r.GetBookAsync(1)).ReturnsAsync(new Book { Id = 1, Title = "X" });
        _repo.Setup(r => r.UpdateBookAsync(It.IsAny<Book>())).ReturnsAsync(true);

        var result = await CreateController().UpdateBook(1, new BookDto { Id = 1, Title = "X" });

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task CreateBook_ReturnsConflict_WhenDuplicateTitle()
    {
        _repo.Setup(r => r.GetAllBooksAsync()).ReturnsAsync(new List<Book> { new() { Id = 1, Title = "Dup" } });

        var result = await CreateController().CreateBook(new BookDto { Title = "dup" });

        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task CreateBook_Returns500_WhenSaveFails()
    {
        _repo.Setup(r => r.GetAllBooksAsync()).ReturnsAsync(new List<Book>());
        _repo.Setup(r => r.CreateBookAsync(It.IsAny<Book>())).ReturnsAsync(false);

        var result = await CreateController().CreateBook(new BookDto { Title = "New" });

        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, obj.StatusCode);
    }

    [Fact]
    public async Task CreateBook_ReturnsCreated_OnSuccess()
    {
        _repo.Setup(r => r.GetAllBooksAsync()).ReturnsAsync(new List<Book>());
        _repo.Setup(r => r.CreateBookAsync(It.IsAny<Book>())).ReturnsAsync(true);

        var result = await CreateController().CreateBook(new BookDto { Title = "New" });

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task DeleteBook_ReturnsNotFound_WhenMissing()
    {
        _repo.Setup(r => r.GetBookAsync(99)).ReturnsAsync((Book?)null);

        var result = await CreateController().DeleteBook(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteBook_ReturnsNoContent_OnSuccess()
    {
        _repo.Setup(r => r.GetBookAsync(1)).ReturnsAsync(new Book { Id = 1 });
        _repo.Setup(r => r.DeleteBookAsync(It.IsAny<Book>())).ReturnsAsync(true);

        var result = await CreateController().DeleteBook(1);

        Assert.IsType<NoContentResult>(result);
    }
}
