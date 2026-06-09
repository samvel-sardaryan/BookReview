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
    public class ReviewersController : ControllerBase
    {
        private readonly IReviewerRepository _reviewerRepository;
        public ReviewersController(IReviewerRepository reviewerRepository)
        {
            _reviewerRepository = reviewerRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewerDto>))]
        public async Task<IActionResult> GetReviewers()
        {
            var reviewers = (await _reviewerRepository.GetReviewersAsync()).Select(r => new ReviewerDto
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
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetReviewer(int reviewerId)
        {
            var reviewer = await _reviewerRepository.GetReviewerAsync(reviewerId);
            if (reviewer == null)
                return NotFound("Reviewer not found");
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
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetReviewsByReviewer(int reviewerId)
        {
            if (!await _reviewerRepository.ReviewerExistsAsync(reviewerId))
                return NotFound("Reviewer not found");
            var reviews = (await _reviewerRepository.GetReviewsByReviewerAsync(reviewerId)).Select(rv => new ReviewDto
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
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateReviewer(int reviewerId, [FromBody] ReviewerDto updateReviewer)
        {
            if (updateReviewer == null || reviewerId != updateReviewer.Id)
                return BadRequest("Invalid data");
            var reviewerToUpdate = await _reviewerRepository.GetReviewerAsync(reviewerId);
            if (reviewerToUpdate == null)
                return NotFound("Reviewer not found");
            reviewerToUpdate.FirstName = updateReviewer.FirstName;
            reviewerToUpdate.LastName = updateReviewer.LastName;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!await _reviewerRepository.UpdateReviewerAsync(reviewerToUpdate))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(ReviewerDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateReviewer([FromBody] ReviewerDto newReviewer)
        {
            if (newReviewer == null)
                return BadRequest("Invalid data");
            var existingReviewer = (await _reviewerRepository.GetReviewersAsync()).FirstOrDefault(c => c.FirstName.Trim().ToUpper() == newReviewer.FirstName.Trim().ToUpper() && c.LastName.Trim().ToUpper() == newReviewer.LastName.Trim().ToUpper());
            if (existingReviewer != null)
            {
                ModelState.AddModelError("", "Reviewer already exists");
                return Conflict(ModelState);
            }
            var reviewerToCreate = new Reviewer
            {
                FirstName = newReviewer.FirstName,
                LastName = newReviewer.LastName
            };
            if (!await _reviewerRepository.CreateReviewerAsync(reviewerToCreate))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            var reviewerDto = new ReviewerDto
            {
                Id = reviewerToCreate.Id,
                FirstName = reviewerToCreate.FirstName,
                LastName = reviewerToCreate.LastName
            };
            return CreatedAtAction(nameof(GetReviewer), new { reviewerId = reviewerToCreate.Id }, reviewerDto);
        }

        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteReviewer(int reviewerId)
        {
            var reviewerToDelete = await _reviewerRepository.GetReviewerAsync(reviewerId);
            if (reviewerToDelete == null)
                return NotFound("Reviewer not found");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!await _reviewerRepository.DeleteReviewerAsync(reviewerToDelete))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
