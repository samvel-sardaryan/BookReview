using BookReview.Data;
using BookReview.Interfaces;
using BookReview.Models;
using Microsoft.EntityFrameworkCore;

namespace BookReview.Repository
{
    public class ReviewerRepository : IReviewerRepository
    {
        private readonly DataContext _context;
        public ReviewerRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> ReviewerExistsAsync(int reviewerId)
        {
            return await _context.Reviewers.AnyAsync(r => r.Id == reviewerId);
        }
        public async Task<ICollection<Reviewer>> GetReviewersAsync()
        {
            return await _context.Reviewers.OrderBy(r => r.Id).ToListAsync();
        }
        public async Task<Reviewer?> GetReviewerAsync(int reviewerId)
        {
            return await _context.Reviewers.Where(r => r.Id == reviewerId).FirstOrDefaultAsync();
        }
        public async Task<ICollection<Review>> GetReviewsByReviewerAsync(int reviewerId)
        {
            return await _context.Reviews.Where(r => r.Reviewer.Id == reviewerId).ToListAsync();
        }

        public async Task<bool> UpdateReviewerAsync(Reviewer reviewer)
        {
            _context.Reviewers.Update(reviewer);
            return await SaveAsync();
        }

        public async Task<bool> CreateReviewerAsync(Reviewer reviewer)
        {
            _context.Reviewers.Add(reviewer);
            return await SaveAsync();
        }

        public async Task<bool> DeleteReviewerAsync(Reviewer reviewer)
        {
            _context.Reviewers.Remove(reviewer);
            return await SaveAsync();
        }

        public async Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0;
        }
    }
}
