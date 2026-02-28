using BookReview.Dto;
using BookReview.Interfaces;
using BookReview.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookReview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        public AuthorsController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        public IActionResult GetAuthors()
        {
            var authors = _authorRepository.GetAllAuthors().Select(a => new AuthorDto
            {
                Id = a.Id,
                Name = a.Name,
                Bio = a.Bio,
                CountryName = a.Country.Name
            });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(authors);
        }

        [HttpGet("{authorId}")]
        [ProducesResponseType(200, Type = typeof(AuthorDto))]
        [ProducesResponseType(400)]
        public IActionResult GetAuthor(int authorId)
        {
            if (!_authorRepository.AuthorExists(authorId))
                return NotFound("Author not found");
            var author = _authorRepository.GetAuthorById(authorId);
            var authorDto = new AuthorDto
            {
                Id = author.Id,
                Name = author.Name,
                Bio = author.Bio,
                CountryName = author.Country.Name
            };
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(authorDto);
        }

        [HttpGet("book/{bookId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetAuthorsOfBook(int bookId)
        {
            var authors = _authorRepository.GetAuthorsOfBook(bookId).Select(a => new AuthorDto
            {
                Id = a.Id,
                Name = a.Name,
                Bio = a.Bio,
                CountryName = a.Country.Name
            });
            if (authors == null)
                return NotFound("Book not found");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(authors);
        }

        [HttpGet("{authorId}/books")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetBooksByAuthor(int authorId)
        {
            if (!_authorRepository.AuthorExists(authorId))
                return NotFound("Author not found");
            var books = _authorRepository.GetBooksByAuthor(authorId).Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                ReleaseDate = b.ReleaseDate
            });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(books);
        }

        [HttpPut("{authorId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult UpdateAuthor(int authorId, [FromBody] AuthorDto updateAuthor)
        {
            if (updateAuthor == null || authorId != updateAuthor.Id)
                return BadRequest("Invalid data");
            if (!_authorRepository.AuthorExists(authorId))
                return BadRequest("Author not found");
            var authorToUpdate = _authorRepository.GetAuthorById(authorId);
            authorToUpdate.Name = updateAuthor.Name;
            authorToUpdate.Bio = updateAuthor.Bio;
            authorToUpdate.Country.Name = updateAuthor.CountryName;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            _authorRepository.UpdateAuthor(authorToUpdate);
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateAuthor([FromBody] AuthorDto newAuthor)
        {
            if (newAuthor == null)
                return BadRequest("Invalid data");
            var existingAuthor = _authorRepository.GetAllAuthors().FirstOrDefault(c => c.Name.Trim().ToUpper() == newAuthor.Name.Trim().ToUpper());
            if (existingAuthor != null)
            {
                ModelState.AddModelError("", "Author already exists");
                return StatusCode(422, ModelState);
            }
            var authorToCreate = new Author
            {
                Name = newAuthor.Name,
                Bio = newAuthor.Bio,
                Country = new Country
                {
                    Name = newAuthor.CountryName
                }
            };
            if (!_authorRepository.CreateAuthor(authorToCreate))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created");
        }

        [HttpDelete("{authorId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult DeleteAuthor(int authorId)
        {
            if (!_authorRepository.AuthorExists(authorId))
                return BadRequest("Author not found");
            var authorToDelete = _authorRepository.GetAuthorById(authorId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!_authorRepository.DeleteAuthor(authorToDelete))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
