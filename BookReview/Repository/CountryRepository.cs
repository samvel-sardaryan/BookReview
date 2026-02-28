using BookReview.Data;
using BookReview.Interfaces;
using BookReview.Models;

namespace BookReview.Repository
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DataContext _context;
        public CountryRepository(DataContext context)
        {
            _context = context;
        }
        public ICollection<Country> GetCountries()
        {
            return _context.Countries.OrderBy(c => c.Id).ToList();
        }
        public Country GetCountry(int countryId)
        {
            return _context.Countries.Where(c => c.Id == countryId).FirstOrDefault();
        }
        public Country GetCountryByAuthor(int authorId)
        {
            return _context.Authors.Where(a => a.Id == authorId).FirstOrDefault().Country;
            //return _context.Countries.Where(c => c.Name == author.Country.Name).FirstOrDefault();
        }
        public bool CountryExists(int countryId)
        {
            return _context.Countries.Any(c => c.Id == countryId);
        }
        public ICollection<Author> GetAuthorsFromCountry(int countryId)
        {
            return _context.Authors.Where(a => a.Country.Id == countryId).ToList();
        }

        public bool UpdateCountry(Country country)
        {
            _context.Countries.Update(country);
            return Save();
        }

        public bool CreateCountry(Country country)
        {
            _context.Countries.Add(country);
            return Save();
        }

        public bool DeleteCountry(Country country)
        {
            _context.Countries.Remove(country);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
