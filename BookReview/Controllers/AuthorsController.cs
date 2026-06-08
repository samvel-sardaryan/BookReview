using BookReview.Dto;
using BookReview.Interfaces;
using BookReview.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BookReview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        public AuthorsController(IAuthorRepository authorRepository, IBookRepository bookRepository)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        public async Task<IActionResult> GetAuthors()
        {
            var authors = (await _authorRepository.GetAllAuthorsAsync()).Select(a => new AuthorDto
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
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(AuthorDto))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAuthor(int authorId)
        {
            var author = await _authorRepository.GetAuthorByIdAsync(authorId);
            if (author == null)
                return NotFound("Author not found");
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
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAuthorsOfBook(int bookId)
        {
            if (!await _bookRepository.BookExistsAsync(bookId))
                return NotFound("Book not found");
            var authors = (await _authorRepository.GetAuthorsOfBookAsync(bookId)).Select(a => new AuthorDto
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

        [HttpGet("{authorId}/books")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetBooksByAuthor(int authorId)
        {
            if (!await _authorRepository.AuthorExistsAsync(authorId))
                return NotFound("Author not found");
            var books = (await _authorRepository.GetBooksByAuthorAsync(authorId)).Select(b => new BookDto
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
        public async Task<IActionResult> UpdateAuthor(int authorId, [FromBody] AuthorDto updateAuthor)
        {
            if (updateAuthor == null || authorId != updateAuthor.Id)
                return BadRequest("Invalid data");
            var authorToUpdate = await _authorRepository.GetAuthorByIdAsync(authorId);
            if (authorToUpdate == null)
                return BadRequest("Author not found");
            authorToUpdate.Name = updateAuthor.Name;
            authorToUpdate.Bio = updateAuthor.Bio;
            authorToUpdate.Country = new Country { Name = updateAuthor.CountryName };
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!await _authorRepository.UpdateAuthorAsync(authorToUpdate))
                return BadRequest("Country not found");
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateAuthor([FromBody] AuthorDto newAuthor)
        {
            if (newAuthor == null)
                return BadRequest("Invalid data");
            var existingAuthor = (await _authorRepository.GetAllAuthorsAsync()).FirstOrDefault(c => c.Name.Trim().ToUpper() == newAuthor.Name.Trim().ToUpper());
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
            if (!await _authorRepository.CreateAuthorAsync(authorToCreate))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created");
        }

        [HttpDelete("{authorId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteAuthor(int authorId)
        {
            var authorToDelete = await _authorRepository.GetAuthorByIdAsync(authorId);
            if (authorToDelete == null)
                return BadRequest("Author not found");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!await _authorRepository.DeleteAuthorAsync(authorToDelete))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
