using BookReview.Models;
namespace BookReview.Interfaces
{
    public interface IBookRepository
    {
        ICollection<Book> GetAllBooks();
        Book GetBook(int id);
        Book GetBookByTitle(string title);
        decimal GetBookRating(int bookId);
        bool BookExists(int id);
        bool UpdateBook(Book book);
        bool CreateBook(Book book);
        bool DeleteBook(Book book);
        bool Save();
    }
}
