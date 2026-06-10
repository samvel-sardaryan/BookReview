using BookReview.Controllers;
using BookReview.Dto;
using BookReview.Interfaces;
using BookReview.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookReview.Tests.Controllers;

public class CategoriesControllerTests
{
    private readonly Mock<ICategoryRepository> _repo = new();

    private CategoriesController CreateController() => new(_repo.Object);

    [Fact]
    public async Task GetCategory_ReturnsNotFound_WhenMissing()
    {
        _repo.Setup(r => r.GetCategoryAsync(99)).ReturnsAsync((Category?)null);

        var result = await CreateController().GetCategory(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetBooksByCategory_ReturnsNotFound_WhenCategoryMissing()
    {
        _repo.Setup(r => r.CategoryExistsAsync(99)).ReturnsAsync(false);

        var result = await CreateController().GetBooksByCategory(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task UpdateCategory_ReturnsBadRequest_WhenIdMismatch()
    {
        var result = await CreateController().UpdateCategory(1, new CategoryDto { Id = 2, Name = "X" });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateCategory_ReturnsNotFound_WhenMissing()
    {
        _repo.Setup(r => r.GetCategoryAsync(1)).ReturnsAsync((Category?)null);

        var result = await CreateController().UpdateCategory(1, new CategoryDto { Id = 1, Name = "X" });

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task UpdateCategory_ReturnsNoContent_OnSuccess()
    {
        _repo.Setup(r => r.GetCategoryAsync(1)).ReturnsAsync(new Category { Id = 1, Name = "X" });
        _repo.Setup(r => r.UpdateCategoryAsync(It.IsAny<Category>())).ReturnsAsync(true);

        var result = await CreateController().UpdateCategory(1, new CategoryDto { Id = 1, Name = "X" });

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task CreateCategory_ReturnsConflict_WhenDuplicate()
    {
        _repo.Setup(r => r.GetCategoriesAsync()).ReturnsAsync(new List<Category> { new() { Id = 1, Name = "Dup" } });

        var result = await CreateController().CreateCategory(new CategoryDto { Name = "dup" });

        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task CreateCategory_Returns500_WhenSaveFails()
    {
        _repo.Setup(r => r.GetCategoriesAsync()).ReturnsAsync(new List<Category>());
        _repo.Setup(r => r.CreateCategoryAsync(It.IsAny<Category>())).ReturnsAsync(false);

        var result = await CreateController().CreateCategory(new CategoryDto { Name = "New" });

        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, obj.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_ReturnsCreated_OnSuccess()
    {
        _repo.Setup(r => r.GetCategoriesAsync()).ReturnsAsync(new List<Category>());
        _repo.Setup(r => r.CreateCategoryAsync(It.IsAny<Category>())).ReturnsAsync(true);

        var result = await CreateController().CreateCategory(new CategoryDto { Name = "New" });

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task DeleteCategory_ReturnsNotFound_WhenMissing()
    {
        _repo.Setup(r => r.GetCategoryAsync(99)).ReturnsAsync((Category?)null);

        var result = await CreateController().DeleteCategory(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteCategory_ReturnsNoContent_OnSuccess()
    {
        _repo.Setup(r => r.GetCategoryAsync(1)).ReturnsAsync(new Category { Id = 1, Name = "X" });
        _repo.Setup(r => r.DeleteCategoryAsync(It.IsAny<Category>())).ReturnsAsync(true);

        var result = await CreateController().DeleteCategory(1);

        Assert.IsType<NoContentResult>(result);
    }
}
