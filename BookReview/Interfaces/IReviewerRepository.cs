using BookReview.Models;

namespace BookReview.Interfaces
{
    public interface IReviewerRepository
    {
        Task<ICollection<Reviewer>> GetReviewersAsync();
        Task<Reviewer?> GetReviewerAsync(int reviewerId);
        Task<ICollection<Review>> GetReviewsByReviewerAsync(int reviewerId);
        Task<bool> ReviewerExistsAsync(int reviewerId);
        Task<bool> UpdateReviewerAsync(Reviewer reviewer);
        Task<bool> CreateReviewerAsync(Reviewer reviewer);
        Task<bool> DeleteReviewerAsync(Reviewer reviewer);
        Task<bool> SaveAsync();
    }
}
