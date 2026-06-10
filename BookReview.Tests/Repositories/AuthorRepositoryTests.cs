using BookReview.Models;
using BookReview.Repository;
using Microsoft.EntityFrameworkCore;

namespace BookReview.Tests.Repositories;

public class AuthorRepositoryTests
{
    [Fact]
    public async Task CreateAuthorAsync_ReturnsFalse_WhenCountryDoesNotExist()
    {
        using var ctx = InMemoryContext.Create();
        var repo = new AuthorRepository(ctx);

        var created = await repo.CreateAuthorAsync(new Author
        {
            Name = "Ghost",
            Country = new Country { Name = "Atlantis" }
        });

        Assert.False(created);
    }

    [Fact]
    public async Task CreateAuthorAsync_ReturnsTrue_AndReusesExistingCountry()
    {
        using var ctx = InMemoryContext.Create();
        ctx.Countries.Add(new Country { Name = "Japan" });
        await ctx.SaveChangesAsync();
        var repo = new AuthorRepository(ctx);

        var created = await repo.CreateAuthorAsync(new Author
        {
            Name = "Kenji",
            Country = new Country { Name = "japan" } // different casing — should still match
        });

        Assert.True(created);
        // The existing country was reused, not duplicated.
        Assert.Equal(1, await ctx.Countries.CountAsync());
    }

    [Fact]
    public async Task UpdateAuthorAsync_ReturnsFalse_WhenCountryDoesNotExist()
    {
        using var ctx = InMemoryContext.Create();
        var country = new Country { Name = "France" };
        var author = new Author { Name = "Marie", Country = country };
        ctx.Countries.Add(country);
        ctx.Authors.Add(author);
        await ctx.SaveChangesAsync();
        var repo = new AuthorRepository(ctx);

        author.Country = new Country { Name = "Nowhere" };
        var updated = await repo.UpdateAuthorAsync(author);

        Assert.False(updated);
    }

    [Fact]
    public async Task UpdateAuthorAsync_ReturnsTrue_AndReassignsToExistingCountry()
    {
        using var ctx = InMemoryContext.Create();
        var us = new Country { Name = "United States" };
        var france = new Country { Name = "France" };
        var author = new Author { Name = "Switcher", Country = us };
        ctx.Countries.AddRange(us, france);
        ctx.Authors.Add(author);
        await ctx.SaveChangesAsync();
        var repo = new AuthorRepository(ctx);

        author.Country = new Country { Name = "France" };
        var updated = await repo.UpdateAuthorAsync(author);

        Assert.True(updated);
        Assert.Equal("France", author.Country.Name);
        Assert.Equal(2, await ctx.Countries.CountAsync()); // no new country added
    }
}
