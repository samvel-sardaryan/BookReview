using BookReview.Models;
using BookReview.Repository;

namespace BookReview.Tests.Repositories;

public class BookRepositoryTests
{
    [Fact]
    public async Task GetBookRatingAsync_ReturnsAverageOfReviews()
    {
        using var ctx = InMemoryContext.Create();
        var book = new Book { Title = "Rated" };
        var reviewer = new Reviewer { FirstName = "A", LastName = "B" };
        ctx.Books.Add(book);
        ctx.Reviewers.Add(reviewer);
        ctx.Reviews.AddRange(
            new Review { Title = "r1", Text = "x", Rating = 4, Book = book, Reviewer = reviewer },
            new Review { Title = "r2", Text = "x", Rating = 5, Book = book, Reviewer = reviewer });
        await ctx.SaveChangesAsync();
        var repo = new BookRepository(ctx);

        var rating = await repo.GetBookRatingAsync(book.Id);

        Assert.Equal(4.5m, rating);
    }

    [Fact]
    public async Task GetBookRatingAsync_ReturnsZero_WhenNoReviews()
    {
        using var ctx = InMemoryContext.Create();
        var book = new Book { Title = "Unrated" };
        ctx.Books.Add(book);
        await ctx.SaveChangesAsync();
        var repo = new BookRepository(ctx);

        var rating = await repo.GetBookRatingAsync(book.Id);

        Assert.Equal(0m, rating);
    }

    [Fact]
    public async Task BookExistsAsync_ReturnsTrueForExisting_AndFalseOtherwise()
    {
        using var ctx = InMemoryContext.Create();
        var book = new Book { Title = "Exists" };
        ctx.Books.Add(book);
        await ctx.SaveChangesAsync();
        var repo = new BookRepository(ctx);

        Assert.True(await repo.BookExistsAsync(book.Id));
        Assert.False(await repo.BookExistsAsync(999999));
    }

    [Fact]
    public async Task CreateBookAsync_PersistsBook()
    {
        using var ctx = InMemoryContext.Create();
        var repo = new BookRepository(ctx);

        var created = await repo.CreateBookAsync(new Book { Title = "Fresh" });

        Assert.True(created);
        var all = await repo.GetAllBooksAsync();
        Assert.Contains(all, b => b.Title == "Fresh");
    }

    [Fact]
    public async Task DeleteBookAsync_RemovesBook()
    {
        using var ctx = InMemoryContext.Create();
        var book = new Book { Title = "Doomed" };
        ctx.Books.Add(book);
        await ctx.SaveChangesAsync();
        var repo = new BookRepository(ctx);

        var deleted = await repo.DeleteBookAsync(book);

        Assert.True(deleted);
        Assert.False(await repo.BookExistsAsync(book.Id));
    }
}
