using BookReview.Controllers;
using BookReview.Dto;
using BookReview.Interfaces;
using BookReview.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookReview.Tests.Controllers;

public class CountriesControllerTests
{
    private readonly Mock<ICountryRepository> _repo = new();

    private CountriesController CreateController() => new(_repo.Object);

    [Fact]
    public async Task GetCountry_ReturnsNotFound_WhenMissing()
    {
        _repo.Setup(r => r.GetCountryAsync(99)).ReturnsAsync((Country?)null);

        var result = await CreateController().GetCountry(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetCountryByAuthor_ReturnsNotFound_WhenNoneForAuthor()
    {
        _repo.Setup(r => r.GetCountryByAuthorAsync(99)).ReturnsAsync((Country?)null);

        var result = await CreateController().GetCountryByAuthor(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetCountryByAuthor_ReturnsOk_WhenFound()
    {
        _repo.Setup(r => r.GetCountryByAuthorAsync(1)).ReturnsAsync(new Country { Id = 1, Name = "United States" });

        var result = await CreateController().GetCountryByAuthor(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<CountryDto>(ok.Value);
        Assert.Equal("United States", dto.Name);
    }

    [Fact]
    public async Task GetAuthorsFromCountry_ReturnsNotFound_WhenCountryMissing()
    {
        _repo.Setup(r => r.CountryExistsAsync(99)).ReturnsAsync(false);

        var result = await CreateController().GetAuthorsFromCountry(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task UpdateCountry_ReturnsNotFound_WhenMissing()
    {
        _repo.Setup(r => r.GetCountryAsync(1)).ReturnsAsync((Country?)null);

        var result = await CreateController().UpdateCountry(1, new CountryDto { Id = 1, Name = "X" });

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task UpdateCountry_ReturnsNoContent_OnSuccess()
    {
        _repo.Setup(r => r.GetCountryAsync(1)).ReturnsAsync(new Country { Id = 1, Name = "X" });
        _repo.Setup(r => r.UpdateCountryAsync(It.IsAny<Country>())).ReturnsAsync(true);

        var result = await CreateController().UpdateCountry(1, new CountryDto { Id = 1, Name = "X" });

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task CreateCountry_ReturnsConflict_WhenDuplicate()
    {
        _repo.Setup(r => r.GetCountriesAsync()).ReturnsAsync(new List<Country> { new() { Id = 1, Name = "Dup" } });

        var result = await CreateController().CreateCountry(new CountryDto { Name = "dup" });

        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task CreateCountry_ReturnsCreated_OnSuccess()
    {
        _repo.Setup(r => r.GetCountriesAsync()).ReturnsAsync(new List<Country>());
        _repo.Setup(r => r.CreateCountryAsync(It.IsAny<Country>())).ReturnsAsync(true);

        var result = await CreateController().CreateCountry(new CountryDto { Name = "New" });

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task DeleteCountry_ReturnsNotFound_WhenMissing()
    {
        _repo.Setup(r => r.GetCountryAsync(99)).ReturnsAsync((Country?)null);

        var result = await CreateController().DeleteCountry(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
