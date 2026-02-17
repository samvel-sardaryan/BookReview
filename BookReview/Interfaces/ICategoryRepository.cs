using BookReview.Models;

namespace BookReview.Interfaces
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategory(int categoryId);
        ICollection<Book> GetBooksByCategory(int categoryId);
        bool CategoryExists(int categoryId);
        bool UpdateCategory(Category category);
        bool CreateCategory(Category category);
        bool DeleteCategory(Category category);
        bool Save();
    }
}
