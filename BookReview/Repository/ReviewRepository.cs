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
        public ICollection<Review> GetReviews()
        {
            return _context.Reviews.Include(r => r.Book).Include(a => a.Reviewer).OrderBy(r => r.Id).ToList();
        }
        public Review GetReview(int reviewId)
        {
            return _context.Reviews.Include(r => r.Book).Include(r => r.Reviewer).Where(r => r.Id == reviewId).FirstOrDefault();
        }

        public ICollection<Review> GetReviewsOfBook(int bookId)
        {
            return _context.Reviews.Include(r => r.Book).Include(r => r.Reviewer).Where(r => r.Book.Id == bookId).ToList();
        }

        public bool ReviewExists(int reviewId)
        {
            return _context.Reviews.Any(r => r.Id == reviewId);
        }

        public bool UpdateReview(Review review)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == review.Book.Id);
            var reviewer = _context.Reviewers.FirstOrDefault(r => r.Id == review.Reviewer.Id);
            if (book == null || reviewer == null)
                return false;
            review.Book = book;
            review.Reviewer = reviewer;
            _context.Reviews.Update(review);
            return Save();
        }

        public bool CreateReview(Review review)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == review.Book.Id);
            var reviewer = _context.Reviewers.FirstOrDefault(r => r.Id == review.Reviewer.Id);
            if (book == null || reviewer == null)
                return false;
            review.Book = book;
            review.Reviewer = reviewer;
            _context.Reviews.Add(review);
            return Save();
        }

        public bool DeleteReview(Review review)
        {
            _context.Reviews.Remove(review);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
