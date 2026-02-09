using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.BLL.Services.Implementations
{
    public class LeaseService : ILeaseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LeaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // BR23: Tạo hợp đồng thuê
        // 1. Tạo sau khi application approved
        // 2. Bao gồm: ngày bắt đầu/kết thúc, giá thuê/tháng, tiền cọc, điều khoản, payment due date (mặc định ngày 5)
        // 3. Lease period tối thiểu 1 tháng
        // 4. Auto-generate mã hợp đồng unique
        public async Task<ServiceResultDto<LeaseDto>> CreateLeaseAsync(int landlordId, CreateLeaseDto dto)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(dto.PropertyId);
            if (property == null || property.LandlordId != landlordId)
                return ServiceResultDto<LeaseDto>.Failure("Không có quyền tạo hợp đồng cho property này.");

            // BR23.3: Lease period tối thiểu 1 tháng
            if ((dto.EndDate - dto.StartDate).TotalDays < 30)
                return ServiceResultDto<LeaseDto>.Failure("Thời hạn hợp đồng tối thiểu 1 tháng.");

            // Kiểm tra không có lease active cho property
            var existingLease = await _unitOfWork.Leases.GetActiveLeaseByPropertyAsync(dto.PropertyId);
            if (existingLease != null)
                return ServiceResultDto<LeaseDto>.Failure("Property đã có hợp đồng thuê đang active.");

            // BR23.4: Auto-generate mã hợp đồng
            var leaseNumber = await _unitOfWork.Leases.GenerateLeaseNumberAsync();

            var lease = new Lease
            {
                PropertyId = dto.PropertyId,
                LandlordId = landlordId,
                TenantId = dto.TenantId,
                RentalApplicationId = dto.RentalApplicationId,
                Status = LeaseStatus.Pending,
                LeaseNumber = leaseNumber,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                MonthlyRent = dto.MonthlyRent,
                DepositAmount = dto.DepositAmount,
                PaymentDueDay = dto.PaymentDueDay,
                LateFeePercentage = dto.LateFeePercentage,
                Terms = dto.Terms,
                SpecialConditions = dto.SpecialConditions,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Leases.AddAsync(lease);
            await _unitOfWork.SaveChangesAsync();

            var result = await _unitOfWork.Leases.GetByIdWithDetailsAsync(lease.Id);
            return ServiceResultDto<LeaseDto>.Success(MapToLeaseDto(result!));
        }

        // BR24: Ký hợp đồng điện tử
        // 1. Cả Landlord và Tenant phải confirm/sign
        // 2. Lease chỉ Active khi cả 2 bên đã ký
        // 3. Timeout ký: 7 ngày, quá hạn auto-cancel
        // 4. Khi active → Property status chuyển 'Rented'
        public async Task<ServiceResultDto> SignLeaseAsync(int userId, int leaseId)
        {
            var lease = await _unitOfWork.Leases.GetByIdWithDetailsAsync(leaseId);
            if (lease == null)
                return ServiceResultDto.Failure("Không tìm thấy hợp đồng.");

            if (lease.Status != LeaseStatus.Pending)
                return ServiceResultDto.Failure("Hợp đồng không ở trạng thái chờ ký.");

            // BR24.3: Kiểm tra timeout 7 ngày
            if ((DateTime.UtcNow - lease.CreatedAt).TotalDays > 7)
            {
                lease.Status = LeaseStatus.Terminated;
                lease.TerminationReason = "Quá hạn ký hợp đồng (7 ngày).";
                lease.TerminatedAt = DateTime.UtcNow;
                _unitOfWork.Leases.Update(lease);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResultDto.Failure("Hợp đồng đã quá hạn ký (7 ngày).");
            }

            // BR24.1: Landlord hoặc Tenant ký
            if (userId == lease.LandlordId)
            {
                lease.LandlordSigned = true;
                lease.LandlordSignedAt = DateTime.UtcNow;
            }
            else if (userId == lease.TenantId)
            {
                lease.TenantSigned = true;
                lease.TenantSignedAt = DateTime.UtcNow;
            }
            else
            {
                return ServiceResultDto.Failure("Bạn không phải bên ký hợp đồng này.");
            }

            // BR24.2: Cả 2 bên ký → Active
            if (lease.LandlordSigned && lease.TenantSigned)
            {
                lease.Status = LeaseStatus.Active;

                // BR24.4: Property chuyển Rented
                var property = await _unitOfWork.Properties.GetByIdAsync(lease.PropertyId);
                if (property != null)
                {
                    property.Status = PropertyStatus.Rented;
                    property.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Properties.Update(property);
                }
            }

            lease.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Leases.Update(lease);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success(lease.Status == LeaseStatus.Active
                ? "Hợp đồng đã được kích hoạt."
                : "Đã ký hợp đồng. Chờ bên còn lại ký.");
        }

        // BR25: Gia hạn hợp đồng
        // 1. Trước khi hết hạn 30 ngày, hệ thống notify cả 2 bên
        // 2. Landlord đề xuất gia hạn (có thể thay đổi giá)
        // 3. Tenant accept/reject
        // 4. Nếu reject → Lease kết thúc đúng hạn
        public async Task<ServiceResultDto> RenewLeaseAsync(int landlordId, RenewLeaseDto dto)
        {
            var lease = await _unitOfWork.Leases.GetByIdAsync(dto.LeaseId);
            if (lease == null)
                return ServiceResultDto.Failure("Không tìm thấy hợp đồng.");

            if (lease.LandlordId != landlordId)
                return ServiceResultDto.Failure("Bạn không có quyền gia hạn hợp đồng này.");

            if (lease.Status != LeaseStatus.Active)
                return ServiceResultDto.Failure("Chỉ có thể gia hạn hợp đồng đang active.");

            // Cập nhật thông tin gia hạn (Landlord đề xuất)
            lease.EndDate = dto.NewEndDate;
            if (dto.NewMonthlyRent.HasValue)
                lease.MonthlyRent = dto.NewMonthlyRent.Value;

            // Reset chữ ký Tenant để Tenant phải accept lại
            lease.TenantSigned = false;
            lease.TenantSignedAt = null;
            lease.SpecialConditions = (lease.SpecialConditions ?? "") + $"\n[Gia hạn] Đề xuất ngày {DateTime.UtcNow:dd/MM/yyyy}";
            lease.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Leases.Update(lease);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success("Đã gửi đề xuất gia hạn cho Tenant.");
        }

        // BR25.3: Tenant accept/reject renewal
        public async Task<ServiceResultDto> AcceptRejectRenewalAsync(int tenantId, int leaseId, bool accept)
        {
            var lease = await _unitOfWork.Leases.GetByIdAsync(leaseId);
            if (lease == null || lease.TenantId != tenantId)
                return ServiceResultDto.Failure("Không tìm thấy hợp đồng.");

            if (accept)
            {
                lease.TenantSigned = true;
                lease.TenantSignedAt = DateTime.UtcNow;
            }
            // BR25.4: Nếu reject → kết thúc đúng hạn (không thay đổi gì)

            lease.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Leases.Update(lease);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success(accept ? "Đã đồng ý gia hạn hợp đồng." : "Đã từ chối gia hạn. Hợp đồng sẽ kết thúc đúng hạn.");
        }

        // BR26: Chấm dứt hợp đồng sớm
        // 1. Một bên yêu cầu chấm dứt sớm
        // 2. Phải thông báo trước 30 ngày
        // 3. Tính phí phạt theo điều khoản
        // 4. Cả 2 bên phải confirm
        // 5. Xử lý hoàn cọc
        public async Task<ServiceResultDto> RequestEarlyTerminationAsync(int userId, EarlyTerminationDto dto)
        {
            var lease = await _unitOfWork.Leases.GetByIdAsync(dto.LeaseId);
            if (lease == null)
                return ServiceResultDto.Failure("Không tìm thấy hợp đồng.");

            if (lease.LandlordId != userId && lease.TenantId != userId)
                return ServiceResultDto.Failure("Bạn không phải bên trong hợp đồng này.");

            if (lease.Status != LeaseStatus.Active)
                return ServiceResultDto.Failure("Chỉ có thể chấm dứt hợp đồng đang active.");

            // BR26.2: Ghi nhận yêu cầu chấm dứt
            lease.TerminationReason = dto.Reason;
            // Reset signatures để cả 2 bên confirm
            lease.LandlordSigned = userId == lease.LandlordId;
            lease.TenantSigned = userId == lease.TenantId;
            lease.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Leases.Update(lease);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success("Đã gửi yêu cầu chấm dứt hợp đồng sớm. Chờ bên còn lại xác nhận.");
        }

        // BR26.4: Bên còn lại confirm chấm dứt
        public async Task<ServiceResultDto> ConfirmTerminationAsync(int userId, int leaseId)
        {
            var lease = await _unitOfWork.Leases.GetByIdWithDetailsAsync(leaseId);
            if (lease == null)
                return ServiceResultDto.Failure("Không tìm thấy hợp đồng.");

            if (lease.LandlordId != userId && lease.TenantId != userId)
                return ServiceResultDto.Failure("Bạn không phải bên trong hợp đồng này.");

            if (userId == lease.LandlordId) lease.LandlordSigned = true;
            if (userId == lease.TenantId) lease.TenantSigned = true;

            // Cả 2 bên đã confirm → chấm dứt
            if (lease.LandlordSigned && lease.TenantSigned)
            {
                lease.Status = LeaseStatus.Terminated;
                lease.TerminatedAt = DateTime.UtcNow;

                // Chuyển property về Available
                var property = await _unitOfWork.Properties.GetByIdAsync(lease.PropertyId);
                if (property != null)
                {
                    property.Status = PropertyStatus.Approved;
                    property.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Properties.Update(property);
                }
            }

            lease.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Leases.Update(lease);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success(lease.Status == LeaseStatus.Terminated
                ? "Hợp đồng đã được chấm dứt."
                : "Đã xác nhận. Chờ bên còn lại.");
        }

        // BR27: Xem hợp đồng
        // 1. Xem chi tiết hợp đồng, lịch sử payment, trạng thái
        public async Task<ServiceResultDto<LeaseDto>> GetByIdAsync(int leaseId)
        {
            var lease = await _unitOfWork.Leases.GetByIdWithDetailsAsync(leaseId);
            if (lease == null)
                return ServiceResultDto<LeaseDto>.Failure("Không tìm thấy hợp đồng.");

            return ServiceResultDto<LeaseDto>.Success(MapToLeaseDto(lease));
        }

        public async Task<ServiceResultDto<List<LeaseDto>>> GetByTenantIdAsync(int tenantId, LeaseStatus? status = null)
        {
            var leases = await _unitOfWork.Leases.GetByTenantIdAsync(tenantId, status);
            return ServiceResultDto<List<LeaseDto>>.Success(leases.Select(MapToLeaseDto).ToList());
        }

        public async Task<ServiceResultDto<List<LeaseDto>>> GetByLandlordIdAsync(int landlordId, LeaseStatus? status = null)
        {
            var leases = await _unitOfWork.Leases.GetByLandlordIdAsync(landlordId, status);
            return ServiceResultDto<List<LeaseDto>>.Success(leases.Select(MapToLeaseDto).ToList());
        }

        // BR25.1: Lấy danh sách lease sắp hết hạn
        public async Task<ServiceResultDto<List<LeaseDto>>> GetExpiringLeasesAsync(int daysBeforeExpiry = 30)
        {
            var leases = await _unitOfWork.Leases.GetExpiringLeasesAsync(daysBeforeExpiry);
            return ServiceResultDto<List<LeaseDto>>.Success(leases.Select(MapToLeaseDto).ToList());
        }

        private static LeaseDto MapToLeaseDto(Lease l)
        {
            return new LeaseDto
            {
                Id = l.Id,
                LeaseNumber = l.LeaseNumber,
                PropertyId = l.PropertyId,
                PropertyTitle = l.Property?.Title ?? string.Empty,
                PropertyAddress = l.Property?.Address ?? string.Empty,
                LandlordId = l.LandlordId,
                LandlordName = l.Landlord?.FullName ?? string.Empty,
                TenantId = l.TenantId,
                TenantName = l.Tenant?.FullName ?? string.Empty,
                Status = l.Status,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                MonthlyRent = l.MonthlyRent,
                DepositAmount = l.DepositAmount,
                Currency = l.Currency,
                PaymentDueDay = l.PaymentDueDay,
                LateFeePercentage = l.LateFeePercentage,
                Terms = l.Terms,
                SpecialConditions = l.SpecialConditions,
                LandlordSigned = l.LandlordSigned,
                LandlordSignedAt = l.LandlordSignedAt,
                TenantSigned = l.TenantSigned,
                TenantSignedAt = l.TenantSignedAt,
                TerminationReason = l.TerminationReason,
                CreatedAt = l.CreatedAt
            };
        }
    }
}
