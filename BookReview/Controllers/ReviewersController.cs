using BookReview.Dto;
using BookReview.Interfaces;
using BookReview.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookReview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewersController : ControllerBase
    {
        private readonly IReviewerRepository _reviewerRepository;
        public ReviewersController(IReviewerRepository reviewerRepository)
        {
            _reviewerRepository = reviewerRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewerDto>))]
        public IActionResult GetReviewers()
        {
            var reviewers = _reviewerRepository.GetReviewers().Select(r => new ReviewerDto
            {
                Id = r.Id,
                FirstName = r.FirstName,
                LastName = r.LastName
            });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(reviewers);
        }

        [HttpGet("{reviewerId}")]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound("Reviewer not found");
            var reviewer = _reviewerRepository.GetReviewer(reviewerId);
            var reviewerDto = new ReviewerDto
            {
                Id = reviewer.Id,
                FirstName = reviewer.FirstName,
                LastName = reviewer.LastName
            };
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(reviewerDto);
        }

        [HttpGet("{reviewerId}/reviews")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsByReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound("Reviewer not found");
            var reviews = _reviewerRepository.GetReviewsByReviewer(reviewerId).Select(rv => new ReviewDto
            {
                Id = rv.Id,
                Title = rv.Title,
                Text = rv.Text,
                Rating = rv.Rating
            });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(reviews);
        }

        [HttpPut("{reviewerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult UpdateReviewer(int reviewerId, [FromBody] ReviewerDto updateReviewer)
        {
            if (updateReviewer == null || reviewerId != updateReviewer.Id)
                return BadRequest("Invalid data");
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return BadRequest("Reviewer not found");
            var reviewerToUpdate = _reviewerRepository.GetReviewer(reviewerId);
            reviewerToUpdate.FirstName = updateReviewer.FirstName;
            reviewerToUpdate.LastName = updateReviewer.LastName;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            _reviewerRepository.UpdateReviewer(reviewerToUpdate);
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReviewer([FromBody] ReviewerDto newReviewer)
        {
            if (newReviewer == null)
                return BadRequest("Invalid data");
            var existingReviewer = _reviewerRepository.GetReviewers().FirstOrDefault(c => c.FirstName.Trim().ToUpper() == newReviewer.FirstName.Trim().ToUpper() && c.LastName.Trim().ToUpper() == newReviewer.LastName.Trim().ToUpper());
            if (existingReviewer != null)
            {
                ModelState.AddModelError("", "Reviewer already exists");
                return StatusCode(422, ModelState);
            }
            var reviewerToCreate = new Reviewer
            {
                FirstName = newReviewer.FirstName,
                LastName = newReviewer.LastName
            };
            if (!_reviewerRepository.CreateReviewer(reviewerToCreate))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created");
        }

        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult DeleteReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return BadRequest("Reviewer not found");
            var reviewerToDelete = _reviewerRepository.GetReviewer(reviewerId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!_reviewerRepository.DeleteReviewer(reviewerToDelete))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
