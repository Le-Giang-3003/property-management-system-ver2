using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.DAL.Repositories.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<Payment?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Payment>> GetByLeaseIdAsync(int leaseId, PaymentStatus? status = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Payment>> GetByTenantIdAsync(int tenantId, PaymentStatus? status = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Payment>> GetByLandlordIdAsync(int landlordId, PaymentStatus? status = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Payment>> GetOverduePaymentsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Payment>> GetPaymentsDueSoonAsync(int daysBeforeDue, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalPaidByLeaseAsync(int leaseId, CancellationToken cancellationToken = default);
    }
}
