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
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IBookRepository _bookRepository;
        public ReviewsController(IReviewRepository reviewRepository, IBookRepository bookRepository)
        {
            _reviewRepository = reviewRepository;
            _bookRepository = bookRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public async Task<IActionResult> GetReviews()
        {
            var reviews = (await _reviewRepository.GetReviewsAsync()).Select(r => new ReviewDto
            {
                Id = r.Id,
                Title = r.Title,
                Text = r.Text,
                Rating = r.Rating,
                BookId = r.Book.Id,
                ReviewerId = r.Reviewer.Id
            });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(reviews);
        }

        [HttpGet("{reviewId}")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(ReviewDto))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetReview(int reviewId)
        {
            var review = await _reviewRepository.GetReviewAsync(reviewId);
            if (review == null)
                return NotFound("Review not found");
            var reviewDto = new ReviewDto
            {
                Id = review.Id,
                Title = review.Title,
                Text = review.Text,
                Rating = review.Rating,
                BookId = review.Book.Id,
                ReviewerId = review.Reviewer.Id
            };
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(reviewDto);
        }

        [HttpGet("book/{bookId}")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetReviewsOfBook(int bookId)
        {
            if (!await _bookRepository.BookExistsAsync(bookId))
                return NotFound("Book not found");
            var reviews = (await _reviewRepository.GetReviewsOfBookAsync(bookId)).Select(r => new ReviewDto
            {
                Id = r.Id,
                Title = r.Title,
                Text = r.Text,
                Rating = r.Rating,
                BookId = r.Book.Id,
                ReviewerId = r.Reviewer.Id
            });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(reviews);
        }

        [HttpPut("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] ReviewDto updateReview)
        {
            if (updateReview == null || reviewId != updateReview.Id)
                return BadRequest("Invalid data");
            var reviewToUpdate = await _reviewRepository.GetReviewAsync(reviewId);
            if (reviewToUpdate == null)
                return BadRequest("Review not found");
            reviewToUpdate.Title = updateReview.Title;
            reviewToUpdate.Text = updateReview.Text;
            reviewToUpdate.Rating = updateReview.Rating;
            reviewToUpdate.Book = new Book { Id = updateReview.BookId };
            reviewToUpdate.Reviewer = new Reviewer { Id = updateReview.ReviewerId };
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!await _reviewRepository.UpdateReviewAsync(reviewToUpdate))
                return BadRequest("Book or reviewer not found");
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateReview([FromBody] ReviewDto newReview)
        {
            if (newReview == null)
                return BadRequest("Invalid data");
            var reviewToCreate = new Review
            {
                Title = newReview.Title,
                Text = newReview.Text,
                Rating = newReview.Rating,
                Book = new Book { Id = newReview.BookId },
                Reviewer = new Reviewer { Id = newReview.ReviewerId }
            };
            if (!await _reviewRepository.CreateReviewAsync(reviewToCreate))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created");
        }

        [HttpDelete("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var reviewToDelete = await _reviewRepository.GetReviewAsync(reviewId);
            if (reviewToDelete == null)
                return BadRequest("Review not found");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!await _reviewRepository.DeleteReviewAsync(reviewToDelete))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
