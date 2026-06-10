using BookReview.Data;
using BookReview.Models;
using BookReview.Repository;

namespace BookReview.Tests.Repositories;

public class ReviewRepositoryTests
{
    private static (Book book, Reviewer reviewer) SeedBookAndReviewer(DataContext ctx)
    {
        var book = new Book { Title = "Reviewed" };
        var reviewer = new Reviewer { FirstName = "Alice", LastName = "Johnson" };
        ctx.Books.Add(book);
        ctx.Reviewers.Add(reviewer);
        ctx.SaveChanges();
        return (book, reviewer);
    }

    [Fact]
    public async Task CreateReviewAsync_ReturnsFalse_WhenBookMissing()
    {
        using var ctx = InMemoryContext.Create();
        var (_, reviewer) = SeedBookAndReviewer(ctx);
        var repo = new ReviewRepository(ctx);

        var created = await repo.CreateReviewAsync(new Review
        {
            Title = "t", Text = "x", Rating = 3,
            Book = new Book { Id = 999999 },
            Reviewer = new Reviewer { Id = reviewer.Id }
        });

        Assert.False(created);
    }

    [Fact]
    public async Task CreateReviewAsync_ReturnsFalse_WhenReviewerMissing()
    {
        using var ctx = InMemoryContext.Create();
        var (book, _) = SeedBookAndReviewer(ctx);
        var repo = new ReviewRepository(ctx);

        var created = await repo.CreateReviewAsync(new Review
        {
            Title = "t", Text = "x", Rating = 3,
            Book = new Book { Id = book.Id },
            Reviewer = new Reviewer { Id = 999999 }
        });

        Assert.False(created);
    }

    [Fact]
    public async Task CreateReviewAsync_ReturnsTrue_WhenBookAndReviewerExist()
    {
        using var ctx = InMemoryContext.Create();
        var (book, reviewer) = SeedBookAndReviewer(ctx);
        var repo = new ReviewRepository(ctx);

        var created = await repo.CreateReviewAsync(new Review
        {
            Title = "t", Text = "x", Rating = 3,
            Book = new Book { Id = book.Id },
            Reviewer = new Reviewer { Id = reviewer.Id }
        });

        Assert.True(created);
    }

    [Fact]
    public async Task UpdateReviewAsync_ReturnsFalse_WhenBookMissing()
    {
        using var ctx = InMemoryContext.Create();
        var (book, reviewer) = SeedBookAndReviewer(ctx);
        var existing = new Review { Title = "t", Text = "x", Rating = 3, Book = book, Reviewer = reviewer };
        ctx.Reviews.Add(existing);
        await ctx.SaveChangesAsync();
        var repo = new ReviewRepository(ctx);

        existing.Book = new Book { Id = 999999 };
        var updated = await repo.UpdateReviewAsync(existing);

        Assert.False(updated);
    }
}
