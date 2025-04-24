using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewService.Application.DTOs;
using ReviewService.Application.Services;

namespace ReviewService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetReviewsByProductId(Guid productId)
        {
            var reviews = await _reviewService.GetReviewsByProductIdAsync(productId);
            return Ok(reviews);
        }

        [HttpGet("product/{productId}/avg")]
        public async Task<ActionResult<double>> GetAverageRating(Guid productId)
        {
            var averageRating = await _reviewService.GetAverageRatingByProductIdAsync(productId);
            return Ok(averageRating);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ReviewDTO>> CreateReview([FromBody] CreateReviewDTO reviewDto)
        {
            var userId = Guid.Parse(User.FindFirst("sub")?.Value);
            var review = await _reviewService.CreateReviewAsync(reviewDto, userId);
            return CreatedAtAction(nameof(GetReviewById), new { id = review.Id }, review);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ReviewDTO>> UpdateReview(Guid id, [FromBody] CreateReviewDTO reviewDto)
        {
            var userId = Guid.Parse(User.FindFirst("sub")?.Value);
            var review = await _reviewService.UpdateReviewAsync(id, reviewDto, userId);
            return Ok(review);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst("sub")?.Value);
            await _reviewService.DeleteReviewAsync(id, userId);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDTO>> GetReviewById(Guid id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            return Ok(review);
        }
    }
} 