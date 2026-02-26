using BookReview.Dto;
using BookReview.Entities;

namespace BookReview.Interfaces
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<string?> LoginAsync(UserDto request);
    }
}
