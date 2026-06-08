using BookReview.Models;

namespace BookReview.Interfaces
{
    public interface IAuthorRepository
    {
        Task<ICollection<Author>> GetAllAuthorsAsync();
        Task<Author?> GetAuthorByIdAsync(int authorId);
        Task<ICollection<Author>> GetAuthorsOfBookAsync(int bookId);
        Task<ICollection<Book>> GetBooksByAuthorAsync(int authorId);
        Task<bool> AuthorExistsAsync(int authorId);
        Task<bool> UpdateAuthorAsync(Author author);
        Task<bool> CreateAuthorAsync(Author author);
        Task<bool> DeleteAuthorAsync(Author author);
        Task<bool> SaveAsync();
    }
}
