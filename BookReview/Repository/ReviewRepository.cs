using BookReview.Data;
using BookReview.Interfaces;
using BookReview.Models;
using Microsoft.EntityFrameworkCore;

namespace BookReview.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly DataContext _context;
        public ReviewRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<ICollection<Review>> GetReviewsAsync()
        {
            return await _context.Reviews.Include(r => r.Book).Include(a => a.Reviewer).OrderBy(r => r.Id).ToListAsync();
        }
        public async Task<Review?> GetReviewAsync(int reviewId)
        {
            return await _context.Reviews.Include(r => r.Book).Include(r => r.Reviewer).Where(r => r.Id == reviewId).FirstOrDefaultAsync();
        }

        public async Task<ICollection<Review>> GetReviewsOfBookAsync(int bookId)
        {
            return await _context.Reviews.Include(r => r.Book).Include(r => r.Reviewer).Where(r => r.Book.Id == bookId).ToListAsync();
        }

        public async Task<bool> UpdateReviewAsync(Review review)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == review.Book.Id);
            var reviewer = await _context.Reviewers.FirstOrDefaultAsync(r => r.Id == review.Reviewer.Id);
            if (book == null || reviewer == null)
                return false;
            review.Book = book;
            review.Reviewer = reviewer;
            _context.Reviews.Update(review);
            return await SaveAsync();
        }

        public async Task<bool> CreateReviewAsync(Review review)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == review.Book.Id);
            var reviewer = await _context.Reviewers.FirstOrDefaultAsync(r => r.Id == review.Reviewer.Id);
            if (book == null || reviewer == null)
                return false;
            review.Book = book;
            review.Reviewer = reviewer;
            _context.Reviews.Add(review);
            return await SaveAsync();
        }

        public async Task<bool> DeleteReviewAsync(Review review)
        {
            _context.Reviews.Remove(review);
            return await SaveAsync();
        }

        public async Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0;
        }
    }
}
