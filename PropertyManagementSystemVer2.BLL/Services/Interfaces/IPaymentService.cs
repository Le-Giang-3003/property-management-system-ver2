using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<ServiceResultDto<List<PaymentDto>>> GetAllPaymentsAsync(DAL.Enums.PaymentStatus? status = null);
        Task<ServiceResultDto<PaymentDto>> GetByIdAsync(int paymentId);
        Task<ServiceResultDto<List<PaymentDto>>> GetByLeaseIdAsync(int leaseId, PaymentStatus? status = null);
        Task<ServiceResultDto<List<PaymentDto>>> GetByTenantIdAsync(int tenantId, PaymentStatus? status = null);
        Task<ServiceResultDto<List<PaymentDto>>> GetByLandlordIdAsync(int landlordId, PaymentStatus? status = null);
        Task<ServiceResultDto> GenerateMonthlyPaymentsAsync(int leaseId);
        Task<ServiceResultDto> MakePaymentAsync(int tenantId, MakePaymentDto dto);
        Task<ServiceResultDto> ConfirmPaymentAsync(int userId, ConfirmPaymentDto dto);
        Task<ServiceResultDto> ProcessOverduePaymentsAsync();
        Task<ServiceResultDto> RefundPaymentAsync(int userId, RefundDto dto);
        Task<ServiceResultDto<PaymentSummaryDto>> GetPaymentSummaryByLeaseAsync(int leaseId);
    }
}
