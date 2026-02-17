using BookReview.Data;
using BookReview.Interfaces;
using BookReview.Models;
using System.Linq;

namespace BookReview.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly DataContext _context;
        public BookRepository(DataContext context)
        {
            _context = context;
        }

        public bool BookExists(int id)
        {
            return _context.Books.Any(b => b.Id == id);
        }

        public ICollection<Book> GetAllBooks()
        {
            return _context.Books.OrderBy(b => b.Id).ToList();
        }

        public Book GetBook(int id)
        {
            return _context.Books.Where(b => b.Id == id).FirstOrDefault();
        }

        public Book GetBookByTitle(string title)
        {
            return _context.Books.Where(b => b.Title.Trim().ToUpper() == title.Trim().ToUpper()).FirstOrDefault();
        }

        public decimal GetBookRating(int bookId)
        {
            var reviews = _context.Reviews.Where(r => r.Book.Id == bookId);
            if (reviews.Count() == 0)
                return 0;
            return (decimal)reviews.Average(r => r.Rating);
        }

        public bool UpdateBook(Book book)
        {
            _context.Books.Update(book);
            return Save();
        }

        public bool CreateBook(Book book)
        {
            _context.Books.Add(book);
            return Save();
        }

        public bool DeleteBook(Book book)
        {
            _context.Books.Remove(book);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
