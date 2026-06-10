using BookReview.Controllers;
using BookReview.Dto;
using BookReview.Interfaces;
using BookReview.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookReview.Tests.Controllers;

public class ReviewersControllerTests
{
    private readonly Mock<IReviewerRepository> _repo = new();

    private ReviewersController CreateController() => new(_repo.Object);

    [Fact]
    public async Task GetReviewer_ReturnsNotFound_WhenMissing()
    {
        _repo.Setup(r => r.GetReviewerAsync(99)).ReturnsAsync((Reviewer?)null);

        var result = await CreateController().GetReviewer(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetReviewsByReviewer_ReturnsNotFound_WhenReviewerMissing()
    {
        _repo.Setup(r => r.ReviewerExistsAsync(99)).ReturnsAsync(false);

        var result = await CreateController().GetReviewsByReviewer(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task UpdateReviewer_ReturnsNotFound_WhenMissing()
    {
        _repo.Setup(r => r.GetReviewerAsync(1)).ReturnsAsync((Reviewer?)null);

        var result = await CreateController().UpdateReviewer(1, new ReviewerDto { Id = 1, FirstName = "A", LastName = "B" });

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task UpdateReviewer_ReturnsNoContent_OnSuccess()
    {
        _repo.Setup(r => r.GetReviewerAsync(1)).ReturnsAsync(new Reviewer { Id = 1, FirstName = "A", LastName = "B" });
        _repo.Setup(r => r.UpdateReviewerAsync(It.IsAny<Reviewer>())).ReturnsAsync(true);

        var result = await CreateController().UpdateReviewer(1, new ReviewerDto { Id = 1, FirstName = "A", LastName = "B" });

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task CreateReviewer_ReturnsConflict_WhenDuplicate()
    {
        _repo.Setup(r => r.GetReviewersAsync()).ReturnsAsync(new List<Reviewer>
        {
            new() { Id = 1, FirstName = "Alice", LastName = "Johnson" }
        });

        var result = await CreateController().CreateReviewer(new ReviewerDto { FirstName = "alice", LastName = "johnson" });

        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task CreateReviewer_ReturnsCreated_OnSuccess()
    {
        _repo.Setup(r => r.GetReviewersAsync()).ReturnsAsync(new List<Reviewer>());
        _repo.Setup(r => r.CreateReviewerAsync(It.IsAny<Reviewer>())).ReturnsAsync(true);

        var result = await CreateController().CreateReviewer(new ReviewerDto { FirstName = "New", LastName = "Person" });

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task DeleteReviewer_ReturnsNotFound_WhenMissing()
    {
        _repo.Setup(r => r.GetReviewerAsync(99)).ReturnsAsync((Reviewer?)null);

        var result = await CreateController().DeleteReviewer(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteReviewer_ReturnsNoContent_OnSuccess()
    {
        _repo.Setup(r => r.GetReviewerAsync(1)).ReturnsAsync(new Reviewer { Id = 1, FirstName = "A", LastName = "B" });
        _repo.Setup(r => r.DeleteReviewerAsync(It.IsAny<Reviewer>())).ReturnsAsync(true);

        var result = await CreateController().DeleteReviewer(1);

        Assert.IsType<NoContentResult>(result);
    }
}
