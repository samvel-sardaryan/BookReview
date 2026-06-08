using BookReview.Data;
using BookReview.Interfaces;
using BookReview.Models;
using Microsoft.EntityFrameworkCore;

namespace BookReview.Repository
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DataContext _context;
        public CountryRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<ICollection<Country>> GetCountriesAsync()
        {
            return await _context.Countries.OrderBy(c => c.Id).ToListAsync();
        }
        public async Task<Country?> GetCountryAsync(int countryId)
        {
            return await _context.Countries.Where(c => c.Id == countryId).FirstOrDefaultAsync();
        }
        public async Task<Country?> GetCountryByAuthorAsync(int authorId)
        {
            var author = await _context.Authors.Include(a => a.Country).Where(a => a.Id == authorId).FirstOrDefaultAsync();
            return author?.Country;
        }
        public async Task<bool> CountryExistsAsync(int countryId)
        {
            return await _context.Countries.AnyAsync(c => c.Id == countryId);
        }
        public async Task<ICollection<Author>> GetAuthorsFromCountryAsync(int countryId)
        {
            return await _context.Authors.Where(a => a.Country.Id == countryId).ToListAsync();
        }

        public async Task<bool> UpdateCountryAsync(Country country)
        {
            _context.Countries.Update(country);
            return await SaveAsync();
        }

        public async Task<bool> CreateCountryAsync(Country country)
        {
            _context.Countries.Add(country);
            return await SaveAsync();
        }

        public async Task<bool> DeleteCountryAsync(Country country)
        {
            _context.Countries.Remove(country);
            return await SaveAsync();
        }

        public async Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0;
        }
    }
}
