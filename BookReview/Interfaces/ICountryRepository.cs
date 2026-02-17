using BookReview.Models;

namespace BookReview.Interfaces
{
    public interface ICountryRepository
    {
        ICollection<Country> GetCountries();
        Country GetCountry(int countryId);
        Country GetCountryByAuthor(int authorId);
        bool CountryExists(int countryId);
        ICollection<Author> GetAuthorsFromCountry(int countryId);
        bool UpdateCountry(Country country);
        bool CreateCountry(Country country);
        bool DeleteCountry(Country country);
        bool Save();
    }
}
