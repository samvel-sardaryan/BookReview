using BookReview.Controllers;
using BookReview.Dto;
using BookReview.Interfaces;
using BookReview.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookReview.Tests.Controllers;

public class ReviewsControllerTests
{
    private readonly Mock<IReviewRepository> _reviews = new();
    private readonly Mock<IBookRepository> _books = new();

    private ReviewsController CreateController() => new(_reviews.Object, _books.Object);

    private static Review SampleReview(int id = 1) =>
        new()
        {
            Id = id,
            Title = "t",
            Text = "x",
            Rating = 5,
            Book = new Book { Id = 2 },
            Reviewer = new Reviewer { Id = 3 }
        };

    [Fact]
    public async Task GetReview_ReturnsNotFound_WhenMissing()
    {
        _reviews.Setup(r => r.GetReviewAsync(99)).ReturnsAsync((Review?)null);

        var result = await CreateController().GetReview(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetReview_ReturnsOk_WithBookAndReviewerIds()
    {
        _reviews.Setup(r => r.GetReviewAsync(1)).ReturnsAsync(SampleReview());

        var result = await CreateController().GetReview(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<ReviewDto>(ok.Value);
        Assert.Equal(2, dto.BookId);
        Assert.Equal(3, dto.ReviewerId);
    }

    [Fact]
    public async Task GetReviewsOfBook_ReturnsNotFound_WhenBookMissing()
    {
        _books.Setup(r => r.BookExistsAsync(99)).ReturnsAsync(false);

        var result = await CreateController().GetReviewsOfBook(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task UpdateReview_ReturnsBadRequest_WhenIdMismatch()
    {
        var result = await CreateController().UpdateReview(1, new ReviewDto { Id = 2, Title = "t", Text = "x", Rating = 3, BookId = 1, ReviewerId = 1 });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateReview_ReturnsNotFound_WhenMissing()
    {
        _reviews.Setup(r => r.GetReviewAsync(1)).ReturnsAsync((Review?)null);

        var result = await CreateController().UpdateReview(1, new ReviewDto { Id = 1, Title = "t", Text = "x", Rating = 3, BookId = 1, ReviewerId = 1 });

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task UpdateReview_ReturnsBadRequest_WhenForeignKeysInvalid()
    {
        _reviews.Setup(r => r.GetReviewAsync(1)).ReturnsAsync(SampleReview());
        _reviews.Setup(r => r.UpdateReviewAsync(It.IsAny<Review>())).ReturnsAsync(false);

        var result = await CreateController().UpdateReview(1, new ReviewDto { Id = 1, Title = "t", Text = "x", Rating = 3, BookId = 999, ReviewerId = 999 });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateReview_ReturnsNoContent_OnSuccess()
    {
        _reviews.Setup(r => r.GetReviewAsync(1)).ReturnsAsync(SampleReview());
        _reviews.Setup(r => r.UpdateReviewAsync(It.IsAny<Review>())).ReturnsAsync(true);

        var result = await CreateController().UpdateReview(1, new ReviewDto { Id = 1, Title = "t", Text = "x", Rating = 3, BookId = 2, ReviewerId = 3 });

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task CreateReview_ReturnsBadRequest_WhenForeignKeysInvalid()
    {
        _reviews.Setup(r => r.CreateReviewAsync(It.IsAny<Review>())).ReturnsAsync(false);

        var result = await CreateController().CreateReview(new ReviewDto { Title = "t", Text = "x", Rating = 3, BookId = 999, ReviewerId = 999 });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateReview_ReturnsCreated_OnSuccess()
    {
        _reviews.Setup(r => r.CreateReviewAsync(It.IsAny<Review>())).ReturnsAsync(true);

        var result = await CreateController().CreateReview(new ReviewDto { Title = "t", Text = "x", Rating = 3, BookId = 2, ReviewerId = 3 });

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task DeleteReview_ReturnsNotFound_WhenMissing()
    {
        _reviews.Setup(r => r.GetReviewAsync(99)).ReturnsAsync((Review?)null);

        var result = await CreateController().DeleteReview(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteReview_ReturnsNoContent_OnSuccess()
    {
        _reviews.Setup(r => r.GetReviewAsync(1)).ReturnsAsync(SampleReview());
        _reviews.Setup(r => r.DeleteReviewAsync(It.IsAny<Review>())).ReturnsAsync(true);

        var result = await CreateController().DeleteReview(1);

        Assert.IsType<NoContentResult>(result);
    }
}
