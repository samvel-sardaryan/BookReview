using BookReview.Controllers;
using BookReview.Dto;
using BookReview.Interfaces;
using BookReview.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookReview.Tests.Controllers;

public class AuthorsControllerTests
{
    private readonly Mock<IAuthorRepository> _authors = new();
    private readonly Mock<IBookRepository> _books = new();

    private AuthorsController CreateController() => new(_authors.Object, _books.Object);

    private static Author SampleAuthor(int id = 1) =>
        new() { Id = id, Name = "A", Bio = "b", Country = new Country { Id = 1, Name = "United States" } };

    [Fact]
    public async Task GetAuthorsOfBook_ReturnsNotFound_WhenBookMissing()
    {
        _books.Setup(r => r.BookExistsAsync(99)).ReturnsAsync(false);

        var result = await CreateController().GetAuthorsOfBook(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetAuthor_ReturnsNotFound_WhenMissing()
    {
        _authors.Setup(r => r.GetAuthorByIdAsync(99)).ReturnsAsync((Author?)null);

        var result = await CreateController().GetAuthor(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetAuthor_ReturnsOk_WithCountryName()
    {
        _authors.Setup(r => r.GetAuthorByIdAsync(1)).ReturnsAsync(SampleAuthor());

        var result = await CreateController().GetAuthor(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<AuthorDto>(ok.Value);
        Assert.Equal("United States", dto.CountryName);
    }

    [Fact]
    public async Task UpdateAuthor_ReturnsNotFound_WhenMissing()
    {
        _authors.Setup(r => r.GetAuthorByIdAsync(1)).ReturnsAsync((Author?)null);

        var result = await CreateController().UpdateAuthor(1, new AuthorDto { Id = 1, Name = "A", CountryName = "US" });

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task UpdateAuthor_ReturnsBadRequest_WhenCountryUnknown()
    {
        _authors.Setup(r => r.GetAuthorByIdAsync(1)).ReturnsAsync(SampleAuthor());
        _authors.Setup(r => r.UpdateAuthorAsync(It.IsAny<Author>())).ReturnsAsync(false);

        var result = await CreateController().UpdateAuthor(1, new AuthorDto { Id = 1, Name = "A", CountryName = "Nowhere" });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateAuthor_ReturnsNoContent_OnSuccess()
    {
        _authors.Setup(r => r.GetAuthorByIdAsync(1)).ReturnsAsync(SampleAuthor());
        _authors.Setup(r => r.UpdateAuthorAsync(It.IsAny<Author>())).ReturnsAsync(true);

        var result = await CreateController().UpdateAuthor(1, new AuthorDto { Id = 1, Name = "A", CountryName = "France" });

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task CreateAuthor_ReturnsConflict_WhenDuplicate()
    {
        _authors.Setup(r => r.GetAllAuthorsAsync()).ReturnsAsync(new List<Author>
        {
            new() { Id = 1, Name = "Dup", Country = new Country { Name = "US" } }
        });

        var result = await CreateController().CreateAuthor(new AuthorDto { Name = "dup", CountryName = "US" });

        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task CreateAuthor_ReturnsBadRequest_WhenCountryUnknown()
    {
        _authors.Setup(r => r.GetAllAuthorsAsync()).ReturnsAsync(new List<Author>());
        _authors.Setup(r => r.CreateAuthorAsync(It.IsAny<Author>())).ReturnsAsync(false);

        var result = await CreateController().CreateAuthor(new AuthorDto { Name = "New", CountryName = "Nowhere" });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateAuthor_ReturnsCreated_OnSuccess()
    {
        _authors.Setup(r => r.GetAllAuthorsAsync()).ReturnsAsync(new List<Author>());
        _authors.Setup(r => r.CreateAuthorAsync(It.IsAny<Author>())).ReturnsAsync(true);

        var result = await CreateController().CreateAuthor(new AuthorDto { Name = "New", CountryName = "United States" });

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task DeleteAuthor_ReturnsNotFound_WhenMissing()
    {
        _authors.Setup(r => r.GetAuthorByIdAsync(99)).ReturnsAsync((Author?)null);

        var result = await CreateController().DeleteAuthor(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteAuthor_ReturnsNoContent_OnSuccess()
    {
        _authors.Setup(r => r.GetAuthorByIdAsync(1)).ReturnsAsync(SampleAuthor());
        _authors.Setup(r => r.DeleteAuthorAsync(It.IsAny<Author>())).ReturnsAsync(true);

        var result = await CreateController().DeleteAuthor(1);

        Assert.IsType<NoContentResult>(result);
    }
}
