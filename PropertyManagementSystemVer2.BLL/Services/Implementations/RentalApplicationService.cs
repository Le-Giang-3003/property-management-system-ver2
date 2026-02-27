using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.BLL.Services.Implementations
{
    public class RentalApplicationService : IRentalApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RentalApplicationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // BR19: Nộp đơn xin thuê
        // 1. Nộp đơn kèm: thông tin cá nhân, nghề nghiệp, thu nhập, số người ở, ngày muốn vào, thời hạn thuê
        // 2. Attach documents (CMND, giấy tờ thu nhập) - xử lý ở tầng Web
        // 3. Max 1 application active/tenant/property
        // 4. Không nộp đơn nếu property không Available
        public async Task<ServiceResultDto<RentalApplicationDto>> SubmitApplicationAsync(int tenantId, CreateRentalApplicationDto dto)
        {
            // BR19.4: Kiểm tra property Available
            var property = await _unitOfWork.Properties.GetByIdAsync(dto.PropertyId);
            if (property == null)
                return ServiceResultDto<RentalApplicationDto>.Failure("Không tìm thấy property.");

            if (property.Status != PropertyStatus.Available)
                return ServiceResultDto<RentalApplicationDto>.Failure("Property không khả dụng để nộp đơn.");

            // BR19.3: Max 1 application active/tenant/property
            if (await _unitOfWork.RentalApplications.HasActiveApplicationAsync(tenantId, dto.PropertyId))
                return ServiceResultDto<RentalApplicationDto>.Failure("Bạn đã có đơn xin thuê đang chờ xử lý cho property này.");

            var application = new RentalApplication
            {
                PropertyId = dto.PropertyId,
                TenantId = tenantId,
                Status = ApplicationStatus.Pending,
                MoveInDate = dto.MoveInDate,
                LeaseDurationMonths = dto.LeaseDurationMonths,
                NumberOfOccupants = dto.NumberOfOccupants,
                Message = dto.Message,
                Occupation = dto.Occupation,
                MonthlyIncome = dto.MonthlyIncome,
                EmployerName = dto.EmployerName,
                EmployerContact = dto.EmployerContact,
                ReferenceName = dto.ReferenceName,
                ReferenceContact = dto.ReferenceContact,
                ReferenceRelationship = dto.ReferenceRelationship,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.RentalApplications.AddAsync(application);
            await _unitOfWork.SaveChangesAsync();

            var result = await _unitOfWork.RentalApplications.GetByIdWithDetailsAsync(application.Id);
            return ServiceResultDto<RentalApplicationDto>.Success(MapToDto(result!));
        }

        // BR20: Xem danh sách Applications
        // 1. Landlord xem danh sách applications theo property
        // 2. Sort theo ngày nộp
        // 3. Xem chi tiết từng đơn kèm attached documents
        public async Task<ServiceResultDto<RentalApplicationDto>> GetByIdAsync(int applicationId)
        {
            var app = await _unitOfWork.RentalApplications.GetByIdWithDetailsAsync(applicationId);
            if (app == null)
                return ServiceResultDto<RentalApplicationDto>.Failure("Không tìm thấy đơn xin thuê.");

            return ServiceResultDto<RentalApplicationDto>.Success(MapToDto(app));
        }

        // BR20.1: Landlord xem theo property
        public async Task<ServiceResultDto<List<RentalApplicationDto>>> GetByPropertyIdAsync(int propertyId, ApplicationStatus? status = null)
        {
            var apps = await _unitOfWork.RentalApplications.GetByPropertyIdAsync(propertyId, status);
            return ServiceResultDto<List<RentalApplicationDto>>.Success(apps.Select(MapToDto).ToList());
        }

        public async Task<ServiceResultDto<List<RentalApplicationDto>>> GetByTenantIdAsync(int tenantId, ApplicationStatus? status = null)
        {
            var apps = await _unitOfWork.RentalApplications.GetByTenantIdAsync(tenantId, status);
            return ServiceResultDto<List<RentalApplicationDto>>.Success(apps.Select(MapToDto).ToList());
        }

        public async Task<ServiceResultDto<List<RentalApplicationDto>>> GetByLandlordIdAsync(int landlordId, ApplicationStatus? status = null)
        {
            var apps = await _unitOfWork.RentalApplications.GetByLandlordIdAsync(landlordId, status);
            return ServiceResultDto<List<RentalApplicationDto>>.Success(apps.Select(MapToDto).ToList());
        }

        // BR21: Landlord duyệt/từ chối đơn xin thuê
        // 1. Approve 1 đơn → auto-reject các đơn khác cho cùng property (configurable)
        // 2. Reject phải nêu lý do
        // 3. Notify Tenant cả 2 trường hợp
        // 4. Approve → chuyển sang flow tạo Lease
        public async Task<ServiceResultDto> ApproveRejectApplicationAsync(int landlordId, ApproveRejectApplicationDto dto)
        {
            var application = await _unitOfWork.RentalApplications.GetByIdWithDetailsAsync(dto.ApplicationId);
            if (application == null)
                return ServiceResultDto.Failure("Không tìm thấy đơn xin thuê.");

            if (application.Property.LandlordId != landlordId)
                return ServiceResultDto.Failure("Bạn không có quyền xử lý đơn này.");

            if (application.Status != ApplicationStatus.Pending)
                return ServiceResultDto.Failure("Đơn xin thuê không ở trạng thái chờ xử lý.");

            if (dto.IsApproved)
            {
                // BR21.1: Approve đơn này
                application.Status = ApplicationStatus.Approved;
                application.ReviewedAt = DateTime.UtcNow;
                _unitOfWork.RentalApplications.Update(application);

                // BR21.1: Auto-reject các đơn khác cho cùng property
                var otherApps = await _unitOfWork.RentalApplications.GetByPropertyIdAsync(application.PropertyId, ApplicationStatus.Pending);
                foreach (var otherApp in otherApps.Where(a => a.Id != application.Id))
                {
                    otherApp.Status = ApplicationStatus.Rejected;
                    otherApp.RejectionReason = "Đã có đơn khác được chấp nhận cho property này.";
                    otherApp.ReviewedAt = DateTime.UtcNow;
                    _unitOfWork.RentalApplications.Update(otherApp);
                }
            }
            else
            {
                // BR21.2: Reject phải nêu lý do
                if (string.IsNullOrWhiteSpace(dto.RejectionReason))
                    return ServiceResultDto.Failure("Phải nêu lý do từ chối.");

                application.Status = ApplicationStatus.Rejected;
                application.RejectionReason = dto.RejectionReason;
                application.ReviewedAt = DateTime.UtcNow;
                _unitOfWork.RentalApplications.Update(application);
            }

            await _unitOfWork.SaveChangesAsync();

            // BR21.3: TODO - Notify Tenant
            return ServiceResultDto.Success(dto.IsApproved ? "Đã chấp nhận đơn xin thuê." : "Đã từ chối đơn xin thuê.");
        }

        // BR22: Rút đơn xin thuê
        // 1. Tenant rút đơn bất cứ lúc nào khi đơn còn Pending
        // 2. Đơn đã Approved/Rejected không rút được
        public async Task<ServiceResultDto> WithdrawApplicationAsync(int tenantId, int applicationId)
        {
            var application = await _unitOfWork.RentalApplications.GetByIdAsync(applicationId);
            if (application == null)
                return ServiceResultDto.Failure("Không tìm thấy đơn xin thuê.");

            if (application.TenantId != tenantId)
                return ServiceResultDto.Failure("Bạn không có quyền rút đơn này.");

            // BR22.2: Chỉ rút khi Pending
            if (application.Status != ApplicationStatus.Pending)
                return ServiceResultDto.Failure("Chỉ có thể rút đơn khi đơn đang ở trạng thái chờ xử lý.");

            application.Status = ApplicationStatus.Cancelled;
            application.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.RentalApplications.Update(application);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success("Đã rút đơn xin thuê.");
        }

        private static RentalApplicationDto MapToDto(RentalApplication a)
        {
            return new RentalApplicationDto
            {
                Id = a.Id,
                PropertyId = a.PropertyId,
                PropertyTitle = a.Property?.Title ?? string.Empty,
                PropertyThumbnail = a.Property?.Images?.FirstOrDefault(i => i.IsPrimary)?.ImageUrl,
                TenantId = a.TenantId,
                TenantName = a.Tenant?.FullName ?? string.Empty,
                TenantEmail = a.Tenant?.Email ?? string.Empty,
                TenantPhone = a.Tenant?.PhoneNumber ?? string.Empty,
                Status = a.Status,
                MoveInDate = a.MoveInDate,
                LeaseDurationMonths = a.LeaseDurationMonths,
                NumberOfOccupants = a.NumberOfOccupants,
                Message = a.Message,
                Occupation = a.Occupation,
                MonthlyIncome = a.MonthlyIncome,
                EmployerName = a.EmployerName,
                RejectionReason = a.RejectionReason,
                ReviewedAt = a.ReviewedAt,
                CreatedAt = a.CreatedAt
            };
        }
    }
}
