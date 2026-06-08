using BookReview.Models;

namespace BookReview.Interfaces
{
    public interface ICategoryRepository
    {
        Task<ICollection<Category>> GetCategoriesAsync();
        Task<Category?> GetCategoryAsync(int categoryId);
        Task<ICollection<Book>> GetBooksByCategoryAsync(int categoryId);
        Task<bool> CategoryExistsAsync(int categoryId);
        Task<bool> UpdateCategoryAsync(Category category);
        Task<bool> CreateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(Category category);
        Task<bool> SaveAsync();
    }
}
