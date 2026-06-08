using BookReview.Models;

namespace BookReview.Interfaces
{
    public interface IReviewRepository
    {
        Task<ICollection<Review>> GetReviewsAsync();
        Task<Review?> GetReviewAsync(int reviewId);
        Task<ICollection<Review>> GetReviewsOfBookAsync(int bookId);
        Task<bool> UpdateReviewAsync(Review review);
        Task<bool> CreateReviewAsync(Review review);
        Task<bool> DeleteReviewAsync(Review review);
        Task<bool> SaveAsync();
    }
}
