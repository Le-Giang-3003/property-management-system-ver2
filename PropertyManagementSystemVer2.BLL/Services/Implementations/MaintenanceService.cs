using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.BLL.Services.Implementations
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MaintenanceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // BR34: Tạo yêu cầu bảo trì
        // 1. Tạo request: tiêu đề, mô tả, mức độ ưu tiên (Low/Medium/High/Emergency)
        // 2. Category: Plumbing/Electrical/Appliance/Structural/Other
        // 3. Attach ảnh/video (max 5 files, 10MB/file) - validate ở tầng Web
        // 4. Chỉ tạo được khi có Lease active
        public async Task<ServiceResultDto<MaintenanceRequestDto>> CreateRequestAsync(int tenantId, CreateMaintenanceRequestDto dto)
        {
            // BR34.4: Kiểm tra Lease active
            var lease = await _unitOfWork.Leases.GetByIdAsync(dto.LeaseId);
            if (lease == null || lease.TenantId != tenantId || lease.Status != LeaseStatus.Active)
                return ServiceResultDto<MaintenanceRequestDto>.Failure("Bạn không có hợp đồng thuê active để tạo yêu cầu bảo trì.");

            var request = new MaintenanceRequest
            {
                PropertyId = dto.PropertyId,
                LeaseId = dto.LeaseId,
                RequestedBy = tenantId,
                Status = MaintenanceStatus.Open,
                Priority = dto.Priority,
                Category = dto.Category,
                Title = dto.Title,
                Description = dto.Description,
                ImageUrls = dto.ImageUrls,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.MaintenanceRequests.AddAsync(request);
            await _unitOfWork.SaveChangesAsync();

            var result = await _unitOfWork.MaintenanceRequests.GetByIdWithDetailsAsync(request.Id);
            return ServiceResultDto<MaintenanceRequestDto>.Success(MapToDto(result!));
        }

        // BR35: Landlord review request
        // 1. Landlord nhận notification, review request
        // 2. Có thể thay đổi priority
        // 3. Set estimated completion date
        // 4. Emergency → notify ngay (push + SMS)
        // 5. Sau khi review xong → assign cho Technician
        public async Task<ServiceResultDto> ReviewRequestAsync(int landlordId, UpdateMaintenanceRequestDto dto)
        {
            var request = await _unitOfWork.MaintenanceRequests.GetByIdWithDetailsAsync(dto.RequestId);
            if (request == null)
                return ServiceResultDto.Failure("Không tìm thấy yêu cầu bảo trì.");

            if (request.Property.LandlordId != landlordId)
                return ServiceResultDto.Failure("Bạn không có quyền review yêu cầu này.");

            // BR35.2: Thay đổi priority
            if (dto.Priority.HasValue) request.Priority = dto.Priority.Value;

            request.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.MaintenanceRequests.Update(request);
            await _unitOfWork.SaveChangesAsync();

            // BR35.4: TODO - Emergency notify
            return ServiceResultDto.Success("Đã review yêu cầu bảo trì.");
        }

        public async Task<ServiceResultDto> LandlordApproveAsync(int landlordId, int requestId, string technicianName, string technicianPhone)
        {
            var request = await _unitOfWork.MaintenanceRequests.GetByIdWithDetailsAsync(requestId);
            if (request == null || request.Property.LandlordId != landlordId) return ServiceResultDto.Failure("Không tìm thấy yêu cầu.");
            if (request.Status != MaintenanceStatus.Open) return ServiceResultDto.Failure("Chỉ phê duyệt yêu cầu đang mở.");

            request.Status = MaintenanceStatus.InProgress;
            request.TechnicianName = technicianName;
            request.TechnicianPhone = technicianPhone;
            request.ScheduledDate = DateTime.UtcNow.AddDays(1); // default: ngày mai
            request.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.MaintenanceRequests.Update(request);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResultDto.Success("Đã phê duyệt yêu cầu.");
        }

        public async Task<ServiceResultDto> LandlordRejectAsync(int landlordId, int requestId, string reason)
        {
            var request = await _unitOfWork.MaintenanceRequests.GetByIdWithDetailsAsync(requestId);
            if (request == null || request.Property.LandlordId != landlordId) return ServiceResultDto.Failure("Không tìm thấy yêu cầu.");
            if (request.Status != MaintenanceStatus.Open) return ServiceResultDto.Failure("Chỉ được từ chối yêu cầu đang mở.");

            request.Status = MaintenanceStatus.Cancelled;
            request.RejectionReason = reason;
            request.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.MaintenanceRequests.Update(request);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResultDto.Success("Đã từ chối yêu cầu bảo trì.");
        }

        public async Task<ServiceResultDto> LandlordCompleteAsync(int landlordId, int requestId, string resolution)
        {
            var request = await _unitOfWork.MaintenanceRequests.GetByIdWithDetailsAsync(requestId);
            if (request == null || request.Property.LandlordId != landlordId) return ServiceResultDto.Failure("Không tìm thấy yêu cầu.");
            if (request.Status != MaintenanceStatus.InProgress) return ServiceResultDto.Failure("Yêu cầu không ở trạng thái đang xử lý.");

            request.Status = MaintenanceStatus.Resolved;
            request.Resolution = resolution; // Bắt lấy value do landlord nhập
            request.ResolvedAt = DateTime.UtcNow;
            request.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.MaintenanceRequests.Update(request);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResultDto.Success("Đã hoàn thành yêu cầu bảo trì.");
        }



        // BR37: Xác nhận hoàn thành
        // 1. Landlord verify kết quả công việc của Technician
        // 2. Tenant confirm request đã resolved
        // 3. Nếu không hài lòng → reopen (max 2 lần)
        // 4. Auto-close sau 7 ngày nếu không confirm
        public async Task<ServiceResultDto> ConfirmCompletionAsync(int userId, int requestId, bool isResolved)
        {
            var request = await _unitOfWork.MaintenanceRequests.GetByIdWithDetailsAsync(requestId);
            if (request == null)
                return ServiceResultDto.Failure("Không tìm thấy yêu cầu.");

            if (request.Status != MaintenanceStatus.Resolved)
                return ServiceResultDto.Failure("Yêu cầu chưa được đánh dấu hoàn thành.");

            if (!isResolved)
            {
                // BR37.3: Reopen
                request.Status = MaintenanceStatus.InProgress;
                request.ResolvedAt = null;
            }

            request.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.MaintenanceRequests.Update(request);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success(isResolved ? "Đã xác nhận hoàn thành." : "Đã yêu cầu xử lý lại.");
        }



        // BR38: Lịch sử bảo trì
        // 1. Xem theo property
        // 2. Filter theo status/priority/category
        // 3. Report: số request/tháng, avg resolution time, category breakdown
        public async Task<ServiceResultDto<MaintenanceRequestDto>> GetByIdAsync(int requestId)
        {
            var request = await _unitOfWork.MaintenanceRequests.GetByIdWithDetailsAsync(requestId);
            if (request == null)
                return ServiceResultDto<MaintenanceRequestDto>.Failure("Không tìm thấy yêu cầu.");
            return ServiceResultDto<MaintenanceRequestDto>.Success(MapToDto(request));
        }

        public async Task<ServiceResultDto<List<MaintenanceRequestDto>>> GetByPropertyIdAsync(int propertyId, MaintenanceStatus? status = null)
        {
            var requests = await _unitOfWork.MaintenanceRequests.GetByPropertyIdAsync(propertyId, status);
            return ServiceResultDto<List<MaintenanceRequestDto>>.Success(requests.Select(MapToDto).ToList());
        }

        public async Task<ServiceResultDto<List<MaintenanceRequestDto>>> GetByTenantIdAsync(int tenantId, MaintenanceStatus? status = null)
        {
            var requests = await _unitOfWork.MaintenanceRequests.GetByTenantIdAsync(tenantId, status);
            return ServiceResultDto<List<MaintenanceRequestDto>>.Success(requests.Select(MapToDto).ToList());
        }

        public async Task<ServiceResultDto<List<MaintenanceRequestDto>>> GetByLandlordIdAsync(int landlordId, MaintenanceStatus? status = null)
        {
            var requests = await _unitOfWork.MaintenanceRequests.GetByLandlordIdAsync(landlordId, status);
            return ServiceResultDto<List<MaintenanceRequestDto>>.Success(requests.Select(MapToDto).ToList());
        }

        public async Task<ServiceResultDto<List<MaintenanceRequestDto>>> GetAllRequestsAsync(MaintenanceStatus? status = null)
        {
            var requests = await _unitOfWork.MaintenanceRequests.GetAllWithDetailsAsync(status);
            return ServiceResultDto<List<MaintenanceRequestDto>>.Success(requests.Select(MapToDto).ToList());
        }

        // BR38.3: Maintenance Summary
        public async Task<ServiceResultDto<MaintenanceSummaryDto>> GetSummaryByPropertyAsync(int propertyId)
        {
            var requests = await _unitOfWork.MaintenanceRequests.GetByPropertyIdAsync(propertyId);
            var requestList = requests.ToList();

            var resolved = requestList.Where(r => r.Status == MaintenanceStatus.Resolved && r.ResolvedAt.HasValue).ToList();
            var avgDays = resolved.Any()
                ? resolved.Average(r => (r.ResolvedAt!.Value - r.CreatedAt).TotalDays)
                : 0;

            var summary = new MaintenanceSummaryDto
            {
                TotalRequests = requestList.Count,
                OpenCount = requestList.Count(r => r.Status == MaintenanceStatus.Open),
                InProgressCount = requestList.Count(r => r.Status == MaintenanceStatus.InProgress),
                ResolvedCount = requestList.Count(r => r.Status == MaintenanceStatus.Resolved),
                AverageResolutionDays = Math.Round(avgDays, 1)
            };

            return ServiceResultDto<MaintenanceSummaryDto>.Success(summary);
        }

        private static MaintenanceRequestDto MapToDto(MaintenanceRequest m)
        {
            return new MaintenanceRequestDto
            {
                Id             = m.Id,
                PropertyId     = m.PropertyId,
                PropertyTitle  = m.Property?.Title ?? string.Empty,
                LeaseId        = m.LeaseId,
                RequestedBy    = m.RequestedBy,
                RequesterName  = m.Requester?.FullName ?? string.Empty,
                RequesterPhone = m.Requester?.PhoneNumber ?? string.Empty,
                Status         = m.Status,
                Priority       = m.Priority,
                Category       = m.Category,
                Title          = m.Title,
                Description    = m.Description,
                ImageUrls      = m.ImageUrls,
                Resolution     = m.Resolution,
                TechnicianName = m.TechnicianName,
                TechnicianPhone = m.TechnicianPhone,
                RejectionReason = m.RejectionReason,
                ScheduledDate  = m.ScheduledDate,
                ResolvedAt     = m.ResolvedAt,
                CreatedAt      = m.CreatedAt
            };
        }
    }
}
