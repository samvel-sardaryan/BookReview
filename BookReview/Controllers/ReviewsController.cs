using BookReview.Dto;
using BookReview.Interfaces;
using BookReview.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookReview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        public ReviewsController(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviews()
        {
            var reviews = _reviewRepository.GetReviews().Select(r => new ReviewDto
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
        [ProducesResponseType(200, Type = typeof(ReviewDto))]
        [ProducesResponseType(400)]
        public IActionResult GetReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound("Review not found");
            var review = _reviewRepository.GetReview(reviewId);
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
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsOfBook(int bookId)
        {
            var reviews = _reviewRepository.GetReviewsOfBook(bookId).Select(r => new ReviewDto
            {
                Id = r.Id,
                Title = r.Title,
                Text = r.Text,
                Rating = r.Rating,
                BookId = r.Book.Id,
                ReviewerId = r.Reviewer.Id
            });
            if (reviews == null)
                return NotFound("No reviews found for the given book");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(reviews);
        }

        [HttpPut("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult UpdateReview(int reviewId, [FromBody] ReviewDto updateReview)
        {
            if (updateReview == null || reviewId != updateReview.Id)
                return BadRequest("Invalid data");
            if (!_reviewRepository.ReviewExists(reviewId))
                return BadRequest("Review not found");
            var reviewToUpdate = _reviewRepository.GetReview(reviewId);
            reviewToUpdate.Title = updateReview.Title;
            reviewToUpdate.Text = updateReview.Text;
            reviewToUpdate.Rating = updateReview.Rating;
            reviewToUpdate.Book = new Book { Id = updateReview.BookId };
            reviewToUpdate.Reviewer = new Reviewer { Id = updateReview.ReviewerId };
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            _reviewRepository.UpdateReview(reviewToUpdate);
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReview([FromBody] ReviewDto newReview)
        {
            if (newReview == null)
                return BadRequest("Invalid data");
            var existingReview = _reviewRepository.GetReviews().FirstOrDefault(c => c.Title.Trim().ToUpper() == newReview.Title.Trim().ToUpper());
            if (existingReview != null)
            {
                ModelState.AddModelError("", "Review already exists");
                return StatusCode(422, ModelState);
            }
            var reviewToCreate = new Review
            {
                Title = newReview.Title,
                Text = newReview.Text,
                Rating = newReview.Rating,
                Book = new Book { Id = newReview.BookId },
                Reviewer = new Reviewer { Id = newReview.ReviewerId }
            };
            if (!_reviewRepository.CreateReview(reviewToCreate))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created");
        }

        [HttpDelete("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult DeleteReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
                return BadRequest("Review not found");
            var reviewToDelete = _reviewRepository.GetReview(reviewId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!_reviewRepository.DeleteReview(reviewToDelete))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
