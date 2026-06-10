using BookReview.Models;
using BookReview.Repository;

namespace BookReview.Tests.Repositories;

public class CountryRepositoryTests
{
    [Fact]
    public async Task GetCountryByAuthorAsync_ReturnsAuthorsCountry()
    {
        using var ctx = InMemoryContext.Create();
        var country = new Country { Name = "United States" };
        var author = new Author { Name = "John", Country = country };
        ctx.Countries.Add(country);
        ctx.Authors.Add(author);
        await ctx.SaveChangesAsync();
        var repo = new CountryRepository(ctx);

        var result = await repo.GetCountryByAuthorAsync(author.Id);

        Assert.NotNull(result);
        Assert.Equal("United States", result!.Name);
    }

    [Fact]
    public async Task GetCountryByAuthorAsync_ReturnsNull_WhenAuthorMissing()
    {
        using var ctx = InMemoryContext.Create();
        var repo = new CountryRepository(ctx);

        var result = await repo.GetCountryByAuthorAsync(999999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAuthorsFromCountryAsync_ReturnsAuthorsInThatCountry()
    {
        using var ctx = InMemoryContext.Create();
        var country = new Country { Name = "Japan" };
        ctx.Countries.Add(country);
        ctx.Authors.Add(new Author { Name = "Kenji", Country = country });
        await ctx.SaveChangesAsync();
        var repo = new CountryRepository(ctx);

        var authors = await repo.GetAuthorsFromCountryAsync(country.Id);

        Assert.Single(authors);
    }

    [Fact]
    public async Task CountryExistsAsync_ReturnsExpected()
    {
        using var ctx = InMemoryContext.Create();
        var country = new Country { Name = "France" };
        ctx.Countries.Add(country);
        await ctx.SaveChangesAsync();
        var repo = new CountryRepository(ctx);

        Assert.True(await repo.CountryExistsAsync(country.Id));
        Assert.False(await repo.CountryExistsAsync(999999));
    }
}
