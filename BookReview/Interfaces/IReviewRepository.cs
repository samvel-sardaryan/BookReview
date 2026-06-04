using BookReview.Models;

namespace BookReview.Interfaces
{
    public interface IReviewRepository
    {
        ICollection<Review> GetReviews();
        Review? GetReview(int reviewId);
        ICollection<Review> GetReviewsOfBook(int bookId);
        bool UpdateReview(Review review);
        bool CreateReview(Review review);
        bool DeleteReview(Review review);
        bool Save();
    }
}
