using ReviewService.Application.DTOs;
using ReviewService.Domain.Entities;
using ReviewService.Domain.Interfaces;

namespace ReviewService.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<ReviewDTO> CreateReviewAsync(CreateReviewDTO reviewDto, Guid userId)
        {
            var review = new Review
            {
                Id = Guid.NewGuid(),
                ProductId = reviewDto.ProductId,
                UserId = userId,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            var createdReview = await _reviewRepository.CreateAsync(review);
            return MapToDTO(createdReview);
        }

        public async Task<ReviewDTO> UpdateReviewAsync(Guid id, CreateReviewDTO reviewDto, Guid userId)
        {
            var existingReview = await _reviewRepository.GetByIdAsync(id);
            if (existingReview == null || existingReview.UserId != userId)
            {
                throw new Exception("Review not found or unauthorized");
            }

            existingReview.Rating = reviewDto.Rating;
            existingReview.Comment = reviewDto.Comment;
            existingReview.UpdatedAt = DateTime.UtcNow;

            var updatedReview = await _reviewRepository.UpdateAsync(existingReview);
            return MapToDTO(updatedReview);
        }

        public async Task DeleteReviewAsync(Guid id, Guid userId)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null || review.UserId != userId)
            {
                throw new Exception("Review not found or unauthorized");
            }

            await _reviewRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ReviewDTO>> GetReviewsByProductIdAsync(Guid productId)
        {
            var reviews = await _reviewRepository.GetByProductIdAsync(productId);
            return reviews.Select(MapToDTO);
        }

        public async Task<double> GetAverageRatingByProductIdAsync(Guid productId)
        {
            return await _reviewRepository.GetAverageRatingAsync(productId);
        }

        public async Task<ReviewDTO> GetReviewByIdAsync(Guid id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
            {
                throw new Exception("Review not found");
            }
            return MapToDTO(review);
        }

        private ReviewDTO MapToDTO(Review review)
        {
            return new ReviewDTO
            {
                Id = review.Id,
                ProductId = review.ProductId,
                UserId = review.UserId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt
            };
        }
    }
} 