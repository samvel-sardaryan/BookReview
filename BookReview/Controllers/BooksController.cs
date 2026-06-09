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
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public async Task<IActionResult> GetBooks()
        {
            var books = (await _bookRepository.GetAllBooksAsync()).Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                ReleaseDate = b.ReleaseDate
            });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(books);
        }

        [HttpGet("{bookId}")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetBook(int bookId)
        {
            var book = await _bookRepository.GetBookAsync(bookId);
            if (book == null)
                return NotFound("Book not found");
            var bookDto = new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                ReleaseDate = book.ReleaseDate
            };
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(bookDto);
        }

        [HttpGet("{bookId}/rating")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetBookRating(int bookId)
        {
            if (!await _bookRepository.BookExistsAsync(bookId))
                return NotFound("Book not found");
            var rating = await _bookRepository.GetBookRatingAsync(bookId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(rating);
        }

        [HttpPut("{bookId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateBook(int bookId, [FromBody] BookDto updateBook)
        {
            if (updateBook == null || bookId != updateBook.Id)
                return BadRequest("Invalid data");
            var bookToUpdate = await _bookRepository.GetBookAsync(bookId);
            if (bookToUpdate == null)
                return NotFound("Book not found");
            bookToUpdate.Title = updateBook.Title;
            bookToUpdate.ReleaseDate = updateBook.ReleaseDate;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!await _bookRepository.UpdateBookAsync(bookToUpdate))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(BookDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateBook([FromBody] BookDto newBook)
        {
            if (newBook == null)
                return BadRequest("Invalid data");
            var existingBook = (await _bookRepository.GetAllBooksAsync()).FirstOrDefault(c => c.Title.Trim().ToUpper() == newBook.Title.Trim().ToUpper());
            if (existingBook != null)
            {
                ModelState.AddModelError("", "Book already exists");
                return Conflict(ModelState);
            }
            var bookToCreate = new Book
            {
                Title = newBook.Title,
                ReleaseDate = newBook.ReleaseDate
            };
            if (!await _bookRepository.CreateBookAsync(bookToCreate))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            var bookDto = new BookDto
            {
                Id = bookToCreate.Id,
                Title = bookToCreate.Title,
                ReleaseDate = bookToCreate.ReleaseDate
            };
            return CreatedAtAction(nameof(GetBook), new { bookId = bookToCreate.Id }, bookDto);
        }

        [HttpDelete("{bookId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteBook(int bookId)
        {
            var bookToDelete = await _bookRepository.GetBookAsync(bookId);
            if (bookToDelete == null)
                return NotFound("Book not found");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!await _bookRepository.DeleteBookAsync(bookToDelete))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
