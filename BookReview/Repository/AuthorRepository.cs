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

        public bool AuthorExists(int authorId)
        {
            return _context.Authors.Any(a => a.Id == authorId);
        }

        public ICollection<Author> GetAllAuthors()
        {
            return _context.Authors.Include(a => a.Country).OrderBy(a => a.Id).ToList();
        }

        public Author GetAuthorById(int authorId)
        {
            return _context.Authors.Include(a => a.Country).Where(a => a.Id == authorId).FirstOrDefault();
        }

        public ICollection<Author> GetAuthorsOfBook(int bookId)
        {
            return _context.Authors.Where(a => a.BookAuthors.Any(ba => ba.BookId == bookId)).Include(a => a.Country).ToList();
        }

        public ICollection<Book> GetBooksByAuthor(int authorId)
        {
            return _context.BookAuthors.Where(ba => ba.AuthorId == authorId).Select(ba => ba.Book).ToList();
        }

        public bool UpdateAuthor(Author author)
        {
            var countryName = author.Country.Name.Trim();
            var existingCountry = _context.Countries.FirstOrDefault(c => c.Name.ToUpper() == countryName.ToUpper());
            if (existingCountry == null)
                return false;
            author.Country = existingCountry;
            _context.Authors.Update(author);
            return Save();
        }

        public bool CreateAuthor(Author author)
        {
            var countryName = author.Country.Name.Trim();
            var existingCountry = _context.Countries.FirstOrDefault(c => c.Name.ToUpper() == countryName.ToUpper());
            if (existingCountry == null)
                return false;
            author.Country = existingCountry;
            _context.Authors.Add(author);
            return Save();
        }

        public bool DeleteAuthor(Author author)
        {
            _context.Authors.Remove(author);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
