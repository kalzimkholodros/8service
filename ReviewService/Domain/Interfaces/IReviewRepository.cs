using ReviewService.Domain.Entities;

namespace ReviewService.Domain.Interfaces
{
    public interface IReviewRepository
    {
        Task<Review> GetByIdAsync(Guid id);
        Task<IEnumerable<Review>> GetByProductIdAsync(Guid productId);
        Task<double> GetAverageRatingAsync(Guid productId);
        Task<IEnumerable<Review>> GetByUserIdAsync(Guid userId);
        Task<Review> CreateAsync(Review review);
        Task<Review> UpdateAsync(Review review);
        Task DeleteAsync(Guid id);
    }
} 