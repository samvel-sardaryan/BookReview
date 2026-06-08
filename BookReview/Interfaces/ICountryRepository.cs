using BookReview.Models;

namespace BookReview.Interfaces
{
    public interface ICountryRepository
    {
        Task<ICollection<Country>> GetCountriesAsync();
        Task<Country?> GetCountryAsync(int countryId);
        Task<Country?> GetCountryByAuthorAsync(int authorId);
        Task<bool> CountryExistsAsync(int countryId);
        Task<ICollection<Author>> GetAuthorsFromCountryAsync(int countryId);
        Task<bool> UpdateCountryAsync(Country country);
        Task<bool> CreateCountryAsync(Country country);
        Task<bool> DeleteCountryAsync(Country country);
        Task<bool> SaveAsync();
    }
}
