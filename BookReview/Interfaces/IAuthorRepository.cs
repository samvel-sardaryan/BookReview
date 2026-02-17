using BookReview.Models;

namespace BookReview.Interfaces
{
    public interface IAuthorRepository
    {
        ICollection<Author> GetAllAuthors();
        Author GetAuthorById(int authorId);
        ICollection<Author> GetAuthorsOfBook(int bookId);
        ICollection<Book> GetBooksByAuthor(int authorId);
        bool AuthorExists(int authorId);
        bool UpdateAuthor(Author author);
        bool CreateAuthor(Author author);
        bool DeleteAuthor(Author author);
        bool Save();
    }
}
