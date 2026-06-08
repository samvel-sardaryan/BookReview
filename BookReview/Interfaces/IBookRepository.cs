using BookReview.Models;

namespace BookReview.Interfaces
{
    public interface IBookRepository
    {
        Task<ICollection<Book>> GetAllBooksAsync();
        Task<Book?> GetBookAsync(int id);
        Task<decimal> GetBookRatingAsync(int bookId);
        Task<bool> BookExistsAsync(int id);
        Task<bool> UpdateBookAsync(Book book);
        Task<bool> CreateBookAsync(Book book);
        Task<bool> DeleteBookAsync(Book book);
        Task<bool> SaveAsync();
    }
}
