using BookReview.Data;
using BookReview.Interfaces;
using BookReview.Models;
using Microsoft.EntityFrameworkCore;

namespace BookReview.Repository
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly DataContext _context;
        public AuthorRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> AuthorExistsAsync(int authorId)
        {
            return await _context.Authors.AnyAsync(a => a.Id == authorId);
        }

        public async Task<ICollection<Author>> GetAllAuthorsAsync()
        {
            return await _context.Authors.Include(a => a.Country).OrderBy(a => a.Id).ToListAsync();
        }

        public async Task<Author?> GetAuthorByIdAsync(int authorId)
        {
            return await _context.Authors.Include(a => a.Country).Where(a => a.Id == authorId).FirstOrDefaultAsync();
        }

        public async Task<ICollection<Author>> GetAuthorsOfBookAsync(int bookId)
        {
            return await _context.Authors.Where(a => a.BookAuthors.Any(ba => ba.BookId == bookId)).Include(a => a.Country).ToListAsync();
        }

        public async Task<ICollection<Book>> GetBooksByAuthorAsync(int authorId)
        {
            return await _context.BookAuthors.Where(ba => ba.AuthorId == authorId).Select(ba => ba.Book).ToListAsync();
        }

        public async Task<bool> UpdateAuthorAsync(Author author)
        {
            var countryName = author.Country.Name.Trim();
            var existingCountry = await _context.Countries.FirstOrDefaultAsync(c => c.Name.ToUpper() == countryName.ToUpper());
            if (existingCountry == null)
                return false;
            author.Country = existingCountry;
            _context.Authors.Update(author);
            return await SaveAsync();
        }

        public async Task<bool> CreateAuthorAsync(Author author)
        {
            var countryName = author.Country.Name.Trim();
            var existingCountry = await _context.Countries.FirstOrDefaultAsync(c => c.Name.ToUpper() == countryName.ToUpper());
            if (existingCountry == null)
                return false;
            author.Country = existingCountry;
            _context.Authors.Add(author);
            return await SaveAsync();
        }

        public async Task<bool> DeleteAuthorAsync(Author author)
        {
            _context.Authors.Remove(author);
            return await SaveAsync();
        }

        public async Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0;
        }
    }
}
