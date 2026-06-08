using BookReview.Data;
using BookReview.Interfaces;
using BookReview.Models;
using Microsoft.EntityFrameworkCore;

namespace BookReview.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly DataContext _context;
        public BookRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> BookExistsAsync(int id)
        {
            return await _context.Books.AnyAsync(b => b.Id == id);
        }

        public async Task<ICollection<Book>> GetAllBooksAsync()
        {
            return await _context.Books.OrderBy(b => b.Id).ToListAsync();
        }

        public async Task<Book?> GetBookAsync(int id)
        {
            return await _context.Books.Where(b => b.Id == id).FirstOrDefaultAsync();
        }

        public async Task<decimal> GetBookRatingAsync(int bookId)
        {
            var reviews = _context.Reviews.Where(r => r.Book.Id == bookId);
            if (await reviews.CountAsync() == 0)
                return 0;
            var average = await reviews.AverageAsync(r => r.Rating);
            return (decimal)average;
        }

        public async Task<bool> UpdateBookAsync(Book book)
        {
            _context.Books.Update(book);
            return await SaveAsync();
        }

        public async Task<bool> CreateBookAsync(Book book)
        {
            _context.Books.Add(book);
            return await SaveAsync();
        }

        public async Task<bool> DeleteBookAsync(Book book)
        {
            _context.Books.Remove(book);
            return await SaveAsync();
        }

        public async Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0;
        }
    }
}
