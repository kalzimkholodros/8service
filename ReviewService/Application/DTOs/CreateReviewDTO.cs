namespace ReviewService.Application.DTOs
{
    public class CreateReviewDTO
    {
        public Guid ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
} 