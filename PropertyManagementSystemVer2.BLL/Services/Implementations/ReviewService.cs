using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.BLL.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        // In-memory reviews store (sẽ thay bằng DB entity sau)
        private static readonly List<(ReviewDto Review, string Type, int TargetId, int LeaseId)> _reviews = new();
        private static int _nextId = 1;

        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // BR55: Tenant đánh giá Property
        // 1. Chỉ đánh giá property đã/đang thuê (có Lease)
        // 2. Rating 1-5 sao + text review
        // 3. Subcategories: cleanliness, location, value, landlord responsiveness
        // 4. 1 review/tenant/lease. Edit trong 7 ngày
        public async Task<ServiceResultDto<ReviewDto>> CreatePropertyReviewAsync(int tenantId, CreatePropertyReviewDto dto)
        {
            // BR55.1: Kiểm tra Lease tồn tại
            var lease = await _unitOfWork.Leases.GetByIdAsync(dto.LeaseId);
            if (lease == null || lease.TenantId != tenantId)
                return ServiceResultDto<ReviewDto>.Failure("Bạn không có hợp đồng thuê cho property này.");

            // BR55.4: 1 review/tenant/lease
            if (_reviews.Any(r => r.Type == "Property" && r.TargetId == dto.PropertyId && r.LeaseId == dto.LeaseId))
                return ServiceResultDto<ReviewDto>.Failure("Bạn đã đánh giá property này cho hợp đồng này.");

            if (dto.Rating < 1 || dto.Rating > 5)
                return ServiceResultDto<ReviewDto>.Failure("Rating phải từ 1 đến 5.");

            var user = await _unitOfWork.Users.GetByIdAsync(tenantId);
            var review = new ReviewDto
            {
                Id = _nextId++,
                ReviewerId = tenantId,
                ReviewerName = user?.FullName ?? string.Empty,
                ReviewerAvatar = user?.AvatarUrl,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CleanlinessRating = dto.CleanlinessRating,
                LocationRating = dto.LocationRating,
                ValueRating = dto.ValueRating,
                LandlordResponsivenessRating = dto.LandlordResponsivenessRating,
                CreatedAt = DateTime.UtcNow
            };

            _reviews.Add((review, "Property", dto.PropertyId, dto.LeaseId));
            return await Task.FromResult(ServiceResultDto<ReviewDto>.Success(review, "Đã đánh giá thành công."));
        }

        // BR56: Landlord đánh giá Tenant
        // 1. Đánh giá Tenant sau khi Lease kết thúc
        // 2. Rating 1-5: payment punctuality, property care, communication
        // 3. 1 review/landlord/lease
        public async Task<ServiceResultDto<ReviewDto>> CreateTenantReviewAsync(int landlordId, CreateTenantReviewDto dto)
        {
            var lease = await _unitOfWork.Leases.GetByIdAsync(dto.LeaseId);
            if (lease == null || lease.LandlordId != landlordId)
                return ServiceResultDto<ReviewDto>.Failure("Bạn không có quyền đánh giá tenant này.");

            // BR56.3: 1 review/landlord/lease
            if (_reviews.Any(r => r.Type == "Tenant" && r.TargetId == dto.TenantId && r.LeaseId == dto.LeaseId))
                return ServiceResultDto<ReviewDto>.Failure("Bạn đã đánh giá tenant này cho hợp đồng này.");

            var user = await _unitOfWork.Users.GetByIdAsync(landlordId);
            var review = new ReviewDto
            {
                Id = _nextId++,
                ReviewerId = landlordId,
                ReviewerName = user?.FullName ?? string.Empty,
                ReviewerAvatar = user?.AvatarUrl,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _reviews.Add((review, "Tenant", dto.TenantId, dto.LeaseId));
            return await Task.FromResult(ServiceResultDto<ReviewDto>.Success(review, "Đã đánh giá thành công."));
        }

        // BR57: Xem Reviews
        // 1. Property reviews public
        // 2. Tenant reviews chỉ visible cho Landlords khi Tenant apply
        // 3. Average rating hiển thị trên listing
        public async Task<ServiceResultDto<List<ReviewDto>>> GetPropertyReviewsAsync(int propertyId)
        {
            var reviews = _reviews.Where(r => r.Type == "Property" && r.TargetId == propertyId).Select(r => r.Review).ToList();
            return await Task.FromResult(ServiceResultDto<List<ReviewDto>>.Success(reviews));
        }

        public async Task<ServiceResultDto<List<ReviewDto>>> GetTenantReviewsAsync(int tenantId)
        {
            var reviews = _reviews.Where(r => r.Type == "Tenant" && r.TargetId == tenantId).Select(r => r.Review).ToList();
            return await Task.FromResult(ServiceResultDto<List<ReviewDto>>.Success(reviews));
        }

        // BR57.3: Average rating
        public async Task<ServiceResultDto<ReviewSummaryDto>> GetPropertyReviewSummaryAsync(int propertyId)
        {
            var reviews = _reviews.Where(r => r.Type == "Property" && r.TargetId == propertyId).Select(r => r.Review).ToList();

            var summary = new ReviewSummaryDto
            {
                AverageRating = reviews.Any() ? Math.Round(reviews.Average(r => r.Rating), 1) : 0,
                TotalReviews = reviews.Count,
                RatingDistribution = Enumerable.Range(1, 5).ToDictionary(i => i, i => reviews.Count(r => r.Rating == i))
            };

            return await Task.FromResult(ServiceResultDto<ReviewSummaryDto>.Success(summary));
        }

        // BR58: Report Review
        // 1. User report review không phù hợp
        // 2. Admin review và quyết định ẩn/xóa
        public async Task<ServiceResultDto> ReportReviewAsync(int userId, int reviewId, string reason)
        {
            // TODO: Lưu report vào DB khi có entity
            return await Task.FromResult(ServiceResultDto.Success("Đã gửi báo cáo review."));
        }
    }
}
