using ReviewService.Application.DTOs;

namespace ReviewService.Application.Services
{
    public interface IReviewService
    {
        Task<ReviewDTO> CreateReviewAsync(CreateReviewDTO reviewDto, Guid userId);
        Task<ReviewDTO> UpdateReviewAsync(Guid id, CreateReviewDTO reviewDto, Guid userId);
        Task DeleteReviewAsync(Guid id, Guid userId);
        Task<IEnumerable<ReviewDTO>> GetReviewsByProductIdAsync(Guid productId);
        Task<double> GetAverageRatingByProductIdAsync(Guid productId);
        Task<ReviewDTO> GetReviewByIdAsync(Guid id);
    }
} 