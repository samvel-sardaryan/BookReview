using BookReview.Dto;
using BookReview.Interfaces;
using BookReview.Models;
using BookReview.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BookReview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetBooks()
        {
            var books = _bookRepository.GetAllBooks().Select(b => new BookDto
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
        [ProducesResponseType(200, Type = typeof(BookDto))]
        [ProducesResponseType(400)]
        public IActionResult GetBook(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
                return NotFound("Book not found");
            var book = _bookRepository.GetBook(bookId);
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
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetBookRating(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
                return NotFound("Book not found");
            var rating = _bookRepository.GetBookRating(bookId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(rating);
        }

        [HttpPut("{bookId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult UpdateBook(int bookId, [FromBody] BookDto updateBook)
        {
            if (updateBook == null || bookId != updateBook.Id)
                return BadRequest("Invalid data");
            if (!_bookRepository.BookExists(bookId))
                return BadRequest("Book not found");
            var bookToUpdate = _bookRepository.GetBook(bookId);
            bookToUpdate.Title = updateBook.Title;
            bookToUpdate.ReleaseDate = updateBook.ReleaseDate;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            _bookRepository.UpdateBook(bookToUpdate);
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateBook([FromBody] BookDto newBook)
        {
            if (newBook == null)
                return BadRequest("Invalid data");
            var existingBook = _bookRepository.GetAllBooks().FirstOrDefault(c => c.Title.Trim().ToUpper() == newBook.Title.Trim().ToUpper());
            if (existingBook != null)
            {
                ModelState.AddModelError("", "Book already exists");
                return StatusCode(422, ModelState);
            }
            var bookToCreate = new Book
            {
                Title = newBook.Title,
                ReleaseDate = newBook.ReleaseDate
            };
            if (!_bookRepository.CreateBook(bookToCreate))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created");
        }

        [HttpDelete("{bookId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult DeleteBook(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
                return BadRequest("Book not found");
            var bookToDelete = _bookRepository.GetBook(bookId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!_bookRepository.DeleteBook(bookToDelete))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
