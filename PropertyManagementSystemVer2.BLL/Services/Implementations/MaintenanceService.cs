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
            // BR35.3: Set estimated completion date
            if (dto.ScheduledDate.HasValue) request.ScheduledDate = dto.ScheduledDate.Value;
            if (dto.EstimatedCost.HasValue) request.EstimatedCost = dto.EstimatedCost.Value;

            request.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.MaintenanceRequests.Update(request);
            await _unitOfWork.SaveChangesAsync();

            // BR35.4: TODO - Emergency notify
            return ServiceResultDto.Success("Đã review yêu cầu bảo trì.");
        }

        // BR39: Assign cho Technician
        // 1. Landlord chọn Technician từ danh sách để assign
        // 2. Gửi notification cho Technician ngay khi assign
        // 3. Có thể reassign sang Technician khác nếu chưa bắt đầu
        // 4. Emergency request → notify Technician qua push + SMS
        public async Task<ServiceResultDto> AssignTechnicianAsync(int landlordId, AssignTechnicianDto dto)
        {
            var request = await _unitOfWork.MaintenanceRequests.GetByIdWithDetailsAsync(dto.RequestId);
            if (request == null)
                return ServiceResultDto.Failure("Không tìm thấy yêu cầu bảo trì.");

            if (request.Property.LandlordId != landlordId)
                return ServiceResultDto.Failure("Bạn không có quyền assign technician.");

            // BR39.3: Chỉ reassign khi chưa bắt đầu (Open status)
            if (request.Status != MaintenanceStatus.Open && request.AssignedTo != null)
                return ServiceResultDto.Failure("Không thể reassign khi technician đã bắt đầu xử lý.");

            request.AssignedTo = dto.TechnicianId;
            request.AssignedAt = DateTime.UtcNow;
            request.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.MaintenanceRequests.Update(request);
            await _unitOfWork.SaveChangesAsync();

            // BR39.2: TODO - Notify Technician
            return ServiceResultDto.Success("Đã assign technician cho yêu cầu.");
        }

        // BR41: Technician accept/decline assignment
        // 1. Technician accept hoặc decline request được assign
        // 2. Decline phải nêu lý do
        // 3. Khi decline → notify Landlord để reassign
        // 4. Accept → status chuyển InProgress, notify Landlord + Tenant
        public async Task<ServiceResultDto> AcceptDeclineAssignmentAsync(int technicianId, int requestId, bool accept, string? reason = null)
        {
            var request = await _unitOfWork.MaintenanceRequests.GetByIdAsync(requestId);
            if (request == null || request.AssignedTo != technicianId)
                return ServiceResultDto.Failure("Không tìm thấy yêu cầu hoặc bạn không được assign.");

            if (accept)
            {
                // BR41.4: Accept → InProgress
                request.Status = MaintenanceStatus.InProgress;
            }
            else
            {
                // BR41.2: Decline phải nêu lý do
                if (string.IsNullOrWhiteSpace(reason))
                    return ServiceResultDto.Failure("Phải nêu lý do từ chối.");

                request.AssignedTo = null;
                request.AssignedAt = null;
                request.Resolution = $"[Từ chối] {reason}";
                // BR41.3: TODO - Notify Landlord
            }

            request.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.MaintenanceRequests.Update(request);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success(accept ? "Đã nhận xử lý yêu cầu." : "Đã từ chối yêu cầu.");
        }

        // BR42: Technician cập nhật tiến độ
        // 1. Technician cập nhật tiến độ công việc
        // 2. Upload ảnh trước/sau sửa chữa (max 10 ảnh/lần update)
        // 3. Ghi note mỗi lần cập nhật
        // 4. Notify Landlord + Tenant mỗi lần update
        public async Task<ServiceResultDto> UpdateProgressAsync(int technicianId, int requestId, string notes, string? imageUrls = null)
        {
            var request = await _unitOfWork.MaintenanceRequests.GetByIdAsync(requestId);
            if (request == null || request.AssignedTo != technicianId)
                return ServiceResultDto.Failure("Không tìm thấy yêu cầu hoặc bạn không được assign.");

            if (request.Status != MaintenanceStatus.InProgress)
                return ServiceResultDto.Failure("Yêu cầu không ở trạng thái đang xử lý.");

            // BR42.3: Ghi note
            request.Resolution = (request.Resolution ?? "") + $"\n[{DateTime.UtcNow:dd/MM/yyyy HH:mm}] {notes}";

            // BR42.2: Cập nhật ảnh
            if (!string.IsNullOrWhiteSpace(imageUrls))
                request.ImageUrls = (request.ImageUrls ?? "") + "," + imageUrls;

            request.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.MaintenanceRequests.Update(request);
            await _unitOfWork.SaveChangesAsync();

            // BR42.4: TODO - Notify Landlord + Tenant
            return ServiceResultDto.Success("Đã cập nhật tiến độ.");
        }

        // BR43: Technician hoàn thành công việc
        // 1. Technician đánh dấu hoàn thành công việc
        // 2. Upload ảnh kết quả sau sửa chữa (bắt buộc)
        // 3. Ghi tóm tắt công việc đã thực hiện
        // 4. Ghi actual cost (chi phí thực tế)
        // 5. Notify Landlord + Tenant để confirm
        public async Task<ServiceResultDto> CompleteRequestAsync(int technicianId, CompleteMaintenanceDto dto)
        {
            var request = await _unitOfWork.MaintenanceRequests.GetByIdAsync(dto.RequestId);
            if (request == null || request.AssignedTo != technicianId)
                return ServiceResultDto.Failure("Không tìm thấy yêu cầu hoặc bạn không được assign.");

            if (request.Status != MaintenanceStatus.InProgress)
                return ServiceResultDto.Failure("Yêu cầu không ở trạng thái đang xử lý.");

            // BR43.3: Ghi tóm tắt
            request.Resolution = (request.Resolution ?? "") + $"\n[HOÀN THÀNH - {DateTime.UtcNow:dd/MM/yyyy HH:mm}] {dto.Resolution}";
            // BR43.4: Actual cost
            request.ActualCost = dto.ActualCost;
            // BR43.2: Ảnh kết quả
            if (!string.IsNullOrWhiteSpace(dto.ImageUrls))
                request.ImageUrls = (request.ImageUrls ?? "") + "," + dto.ImageUrls;

            request.Status = MaintenanceStatus.Resolved;
            request.ResolvedAt = DateTime.UtcNow;
            request.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.MaintenanceRequests.Update(request);
            await _unitOfWork.SaveChangesAsync();

            // BR43.5: TODO - Notify Landlord + Tenant
            return ServiceResultDto.Success("Đã hoàn thành công việc sửa chữa.");
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
                request.Resolution = (request.Resolution ?? "") + $"\n[REOPEN - {DateTime.UtcNow:dd/MM/yyyy}] Chưa hài lòng với kết quả";
            }

            request.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.MaintenanceRequests.Update(request);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success(isResolved ? "Đã xác nhận hoàn thành." : "Đã yêu cầu xử lý lại.");
        }

        // BR37.5: Tenant đánh giá Technician
        public async Task<ServiceResultDto> RateMaintenanceAsync(int tenantId, RateMaintenanceDto dto)
        {
            var request = await _unitOfWork.MaintenanceRequests.GetByIdAsync(dto.RequestId);
            if (request == null || request.RequestedBy != tenantId)
                return ServiceResultDto.Failure("Không tìm thấy yêu cầu hoặc bạn không phải người yêu cầu.");

            if (request.Status != MaintenanceStatus.Resolved)
                return ServiceResultDto.Failure("Chỉ đánh giá khi yêu cầu đã hoàn thành.");

            if (dto.Rating < 1 || dto.Rating > 5)
                return ServiceResultDto.Failure("Đánh giá phải từ 1 đến 5 sao.");

            request.Rating = dto.Rating;
            request.Feedback = dto.Feedback;
            request.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.MaintenanceRequests.Update(request);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success("Đã đánh giá.");
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

        // BR40: Technician xem request được assign
        // 1. Technician xem danh sách request được assign cho mình
        // 2. Filter theo status (Assigned/InProgress/Completed), priority, category
        // 3. Xem chi tiết request: mô tả, ảnh, thông tin property, liên hệ Tenant/Landlord
        public async Task<ServiceResultDto<List<MaintenanceRequestDto>>> GetByAssignedToAsync(int technicianId, MaintenanceStatus? status = null)
        {
            var requests = await _unitOfWork.MaintenanceRequests.GetByAssignedToAsync(technicianId, status);
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
                Id = m.Id,
                PropertyId = m.PropertyId,
                PropertyTitle = m.Property?.Title ?? string.Empty,
                LeaseId = m.LeaseId,
                RequestedBy = m.RequestedBy,
                RequesterName = m.Requester?.FullName ?? string.Empty,
                Status = m.Status,
                Priority = m.Priority,
                Category = m.Category,
                Title = m.Title,
                Description = m.Description,
                ImageUrls = m.ImageUrls,
                AssignedTo = m.AssignedTo,
                AssignedAt = m.AssignedAt,
                EstimatedCost = m.EstimatedCost,
                ActualCost = m.ActualCost,
                Resolution = m.Resolution,
                ResolvedAt = m.ResolvedAt,
                ScheduledDate = m.ScheduledDate,
                Rating = m.Rating,
                Feedback = m.Feedback,
                CreatedAt = m.CreatedAt
            };
        }
    }
}
