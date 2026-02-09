namespace PropertyManagementSystemVer2.BLL.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int ReviewerId { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
        public string? ReviewerAvatar { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public int? CleanlinessRating { get; set; }
        public int? LocationRating { get; set; }
        public int? ValueRating { get; set; }
        public int? LandlordResponsivenessRating { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreatePropertyReviewDto
    {
        public int PropertyId { get; set; }
        public int LeaseId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public int? CleanlinessRating { get; set; }
        public int? LocationRating { get; set; }
        public int? ValueRating { get; set; }
        public int? LandlordResponsivenessRating { get; set; }
    }

    public class CreateTenantReviewDto
    {
        public int TenantId { get; set; }
        public int LeaseId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public int? PaymentPunctualityRating { get; set; }
        public int? PropertyCareRating { get; set; }
        public int? CommunicationRating { get; set; }
    }

    public class ReviewSummaryDto
    {
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } = new();
    }
}
