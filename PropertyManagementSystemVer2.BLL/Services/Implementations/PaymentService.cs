using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.BLL.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // BR28: Tự động tạo Payment record hàng tháng
        // - Tạo vào ngày 1 hàng tháng, sau khi StartDate và trước EndDate
        // - Hạn chót mặc định ngày 5 của tháng billing
        public async Task<ServiceResultDto> GenerateMonthlyPaymentsAsync(int leaseId)
        {
            var lease = await _unitOfWork.Leases.GetByIdWithDetailsAsync(leaseId);
            if (lease == null)
                return ServiceResultDto.Failure("Không tìm thấy hợp đồng.");

            if (lease.Status != LeaseStatus.Active)
                return ServiceResultDto.Failure("Hợp đồng không active.");

            var now = DateTime.UtcNow.Date;
            var billingMonth = now.Month;
            var billingYear = now.Year;

            // Kiểm tra hợp đồng đã bắt đầu chưa (ngày hiện tại >= ngày bắt đầu)
            // StartDate = 2026-02-28 → hóa đơn đầu tiên tạo từ 2026-03-01 trở đi
            if (now < lease.StartDate.Date)
                return ServiceResultDto.Failure("Hợp đồng chưa bắt đầu.");

            // Kiểm tra hợp đồng chưa hết hạn (EndDate là ngày cuối cùng tenant thuê)
            if (now > lease.EndDate.Date)
                return ServiceResultDto.Failure("Hợp đồng đã hết hạn, không tạo thêm hóa đơn.");

            // Kiểm tra đã tạo payment cho tháng này chưa
            var existingPayments = await _unitOfWork.Payments.GetByLeaseIdAsync(leaseId);
            var hasPaymentForMonth = existingPayments.Any(p =>
                p.BillingMonth == billingMonth && p.BillingYear == billingYear && p.PaymentType == PaymentType.Rent);

            if (hasPaymentForMonth)
                return ServiceResultDto.Failure("Đã tạo payment cho tháng này.");

            // Hạn chót: ngày 5 của tháng billing (hoặc PaymentDueDay nếu muốn configurable)
            var dueDay = lease.PaymentDueDay > 0 ? lease.PaymentDueDay : 5;
            // Đảm bảo dueDay không vượt quá số ngày trong tháng
            var daysInMonth = DateTime.DaysInMonth(billingYear, billingMonth);
            dueDay = Math.Min(dueDay, daysInMonth);
            var dueDate = new DateTime(billingYear, billingMonth, dueDay);

            // Tạo payment tiền thuê
            var rentPayment = new Payment
            {
                LeaseId = leaseId,
                PaymentType = PaymentType.Rent,
                Status = PaymentStatus.Pending,
                Amount = lease.MonthlyRent,
                Currency = lease.Currency,
                DueDate = dueDate,
                BillingMonth = billingMonth,
                BillingYear = billingYear,
                Description = $"Ti\u1ec1n thu\u00ea th\u00e1ng {billingMonth}/{billingYear}",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Payments.AddAsync(rentPayment);

            // Payment đầu tiên bao gồm tiền cọc
            var isFirstPayment = !existingPayments.Any(p => p.PaymentType == PaymentType.Deposit);
            if (isFirstPayment && lease.DepositAmount > 0)
            {
                var depositPayment = new Payment
                {
                    LeaseId = leaseId,
                    PaymentType = PaymentType.Deposit,
                    Status = PaymentStatus.Pending,
                    Amount = lease.DepositAmount,
                    Currency = lease.Currency,
                    DueDate = dueDate,
                    BillingMonth = billingMonth,
                    BillingYear = billingYear,
                    Description = "Ti\u1ec1n \u0111\u1eb7t c\u1ecdc",
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Payments.AddAsync(depositPayment);
            }

            await _unitOfWork.SaveChangesAsync();
            return ServiceResultDto.Success($"\u0110\u00e3 t\u1ea1o payment th\u00e1ng {billingMonth}/{billingYear}.");
        }

        // BR29: Tenant thanh toán
        // 1. Thanh toán qua: chuyển khoản, ví điện tử, thẻ
        // 2. Upload biên lai nếu chuyển khoản manual
        // 3. Landlord confirm nhận tiền (manual) hoặc auto-confirm (gateway)
        public async Task<ServiceResultDto> MakePaymentAsync(int tenantId, MakePaymentDto dto)
        {
            var payment = await _unitOfWork.Payments.GetByIdWithDetailsAsync(dto.PaymentId);
            if (payment == null)
                return ServiceResultDto.Failure("Không tìm thấy khoản thanh toán.");

            if (payment.Lease.TenantId != tenantId)
                return ServiceResultDto.Failure("Bạn không phải người thuê của hợp đồng này.");

            if (payment.Status != PaymentStatus.Pending && payment.Status != PaymentStatus.Overdue)
                return ServiceResultDto.Failure("Khoản thanh toán không ở trạng thái chờ thanh toán.");

            // BR29.1: Cập nhật thông tin thanh toán
            payment.PaymentMethod = dto.PaymentMethod;
            payment.TransactionId = dto.TransactionId;
            payment.PaymentProof = dto.PaymentProof;
            payment.Notes = dto.Notes;
            payment.PaidDate = DateTime.UtcNow;

            // BR29.3: Nếu có gateway → auto-confirm, manual → chờ Landlord confirm
            if (dto.PaymentMethod == DAL.Enums.PaymentMethod.CreditCard || dto.PaymentMethod == DAL.Enums.PaymentMethod.EWallet)
            {
                payment.Status = PaymentStatus.Completed;
            }
            // Nếu chuyển khoản manual → giữ Pending, chờ Landlord confirm

            payment.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success("Đã ghi nhận thanh toán.");
        }

        // BR30: Landlord xác nhận Payment
        // 1. Landlord verify payment (manual flow)
        // 2. Nếu không confirm trong 48h → escalate Admin
        // 3. Auto-confirm với payment gateway
        public async Task<ServiceResultDto> ConfirmPaymentAsync(int userId, ConfirmPaymentDto dto)
        {
            var payment = await _unitOfWork.Payments.GetByIdWithDetailsAsync(dto.PaymentId);
            if (payment == null)
                return ServiceResultDto.Failure("Không tìm thấy khoản thanh toán.");

            // Kiểm tra quyền: Landlord hoặc Admin
            if (payment.Lease.LandlordId != userId)
                return ServiceResultDto.Failure("Bạn không có quyền xác nhận khoản thanh toán này.");

            if (dto.IsConfirmed)
            {
                payment.Status = PaymentStatus.Completed;
                payment.PaidDate ??= DateTime.UtcNow;
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
            }

            payment.Notes = (payment.Notes ?? "") + $"\n[{(dto.IsConfirmed ? "Xác nhận" : "Từ chối")}] {dto.Notes}";
            payment.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success(dto.IsConfirmed ? "Đã xác nhận thanh toán." : "Đã từ chối thanh toán.");
        }

        // BR31: Xử lý trả chậm và quá hạn
        // Phí quá hạn (1 lần): LateFeeAmount = MonthlyRent × (LateFeePercentage / 100)
        // Tổng phải trả = MonthlyRent + LateFeeAmount
        public async Task<ServiceResultDto> ProcessOverduePaymentsAsync()
        {
            var overduePayments = await _unitOfWork.Payments.GetOverduePaymentsAsync();
            var processedCount = 0;

            foreach (var payment in overduePayments)
            {
                var daysOverdue = (int)(DateTime.UtcNow - payment.DueDate).TotalDays;
                if (daysOverdue <= 0) continue;

                // Tính phí quá hạn 1 lần khi lần đầu quá hạn
                if (payment.LateFeeAmount == null)
                {
                    var lease = await _unitOfWork.Leases.GetByIdAsync(payment.LeaseId);
                    if (lease?.LateFeePercentage is > 0)
                    {
                        // Tổng phí = MonthlyRent × (LateFeePercentage / 100)
                        // LateFeeAmount chỉ là phần phí thêm, tổng cần trả = Amount + LateFeeAmount
                        payment.LateFeeAmount = Math.Round(lease.MonthlyRent * (lease.LateFeePercentage.Value / 100m), 0);
                    }
                }

                if (payment.Status != PaymentStatus.Overdue)
                {
                    payment.Status = PaymentStatus.Overdue;
                    processedCount++;
                }

                payment.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Payments.Update(payment);
            }

            await _unitOfWork.SaveChangesAsync();
            return ServiceResultDto.Success($"Đã xử lý {processedCount} khoản thanh toán quá hạn.");
        }


        // BR33: Hoàn tiền
        // 1. Landlord hoặc Admin initiate refund
        // 2. Ghi rõ lý do, số tiền
        // 3. Refund về phương thức thanh toán gốc hoặc credit balance
        // 4. Cần Admin approve nếu > threshold
        public async Task<ServiceResultDto> RefundPaymentAsync(int userId, RefundDto dto)
        {
            var originalPayment = await _unitOfWork.Payments.GetByIdWithDetailsAsync(dto.PaymentId);
            if (originalPayment == null)
                return ServiceResultDto.Failure("Không tìm thấy khoản thanh toán.");

            if (originalPayment.Status != PaymentStatus.Completed)
                return ServiceResultDto.Failure("Chỉ có thể hoàn tiền khoản đã thanh toán.");

            if (dto.Amount > originalPayment.Amount)
                return ServiceResultDto.Failure("Số tiền hoàn không được lớn hơn số tiền đã thanh toán.");

            // Cập nhật payment gốc
            originalPayment.Status = PaymentStatus.Refunded;
            originalPayment.Notes = (originalPayment.Notes ?? "") + $"\n[Hoàn tiền] {dto.Reason} - Số tiền: {dto.Amount}";
            originalPayment.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Payments.Update(originalPayment);

            await _unitOfWork.SaveChangesAsync();
            return ServiceResultDto.Success("Đã tạo yêu cầu hoàn tiền.");
        }

        // Admin: Xem tất cả payments trong hệ thống
        public async Task<ServiceResultDto<List<PaymentDto>>> GetAllPaymentsAsync(PaymentStatus? status = null)
        {
            var query = _unitOfWork.Payments.Query();
            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);
            var list = query.OrderByDescending(p => p.CreatedAt).ToList();
            // Fetch details for each (avoid lazy loading issues)
            var result = new List<PaymentDto>();
            foreach (var p in list)
            {
                var detailed = await _unitOfWork.Payments.GetByIdWithDetailsAsync(p.Id);
                if (detailed != null) result.Add(MapToPaymentDto(detailed));
            }
            return ServiceResultDto<List<PaymentDto>>.Success(result);
        }

        // BR32: Xem lịch sử Payment
        // 1. Xem lịch sử theo Lease
        // 2. Filter theo status (Pending/Paid/Overdue/Failed)
        public async Task<ServiceResultDto<PaymentDto>> GetByIdAsync(int paymentId)
        {
            var payment = await _unitOfWork.Payments.GetByIdWithDetailsAsync(paymentId);
            if (payment == null)
                return ServiceResultDto<PaymentDto>.Failure("Không tìm thấy khoản thanh toán.");

            return ServiceResultDto<PaymentDto>.Success(MapToPaymentDto(payment));
        }

        public async Task<ServiceResultDto<List<PaymentDto>>> GetByLeaseIdAsync(int leaseId, PaymentStatus? status = null)
        {
            var payments = await _unitOfWork.Payments.GetByLeaseIdAsync(leaseId, status);
            return ServiceResultDto<List<PaymentDto>>.Success(payments.Select(MapToPaymentDto).ToList());
        }

        public async Task<ServiceResultDto<List<PaymentDto>>> GetByTenantIdAsync(int tenantId, PaymentStatus? status = null)
        {
            var payments = await _unitOfWork.Payments.GetByTenantIdAsync(tenantId, status);
            return ServiceResultDto<List<PaymentDto>>.Success(payments.Select(MapToPaymentDto).ToList());
        }

        public async Task<ServiceResultDto<List<PaymentDto>>> GetByLandlordIdAsync(int landlordId, PaymentStatus? status = null)
        {
            var payments = await _unitOfWork.Payments.GetByLandlordIdAsync(landlordId, status);
            return ServiceResultDto<List<PaymentDto>>.Success(payments.Select(MapToPaymentDto).ToList());
        }

        // BR32.4: Tổng hợp payment theo lease
        public async Task<ServiceResultDto<PaymentSummaryDto>> GetPaymentSummaryByLeaseAsync(int leaseId)
        {
            var payments = await _unitOfWork.Payments.GetByLeaseIdAsync(leaseId);
            var paymentList = payments.ToList();

            var summary = new PaymentSummaryDto
            {
                TotalPaid = paymentList.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount),
                TotalPending = paymentList.Where(p => p.Status == PaymentStatus.Pending).Sum(p => p.Amount),
                TotalOverdue = paymentList.Where(p => p.Status == PaymentStatus.Overdue).Sum(p => p.Amount),
                PaidCount = paymentList.Count(p => p.Status == PaymentStatus.Completed),
                PendingCount = paymentList.Count(p => p.Status == PaymentStatus.Pending),
                OverdueCount = paymentList.Count(p => p.Status == PaymentStatus.Overdue)
            };

            return ServiceResultDto<PaymentSummaryDto>.Success(summary);
        }

        private static PaymentDto MapToPaymentDto(Payment p)
        {
            return new PaymentDto
            {
                Id = p.Id,
                LeaseId = p.LeaseId,
                LeaseNumber = p.Lease?.LeaseNumber ?? string.Empty,
                PropertyTitle = p.Lease?.Property?.Title ?? string.Empty,
                TenantName = p.Lease?.Tenant?.FullName ?? string.Empty,
                LandlordName = p.Lease?.Landlord?.FullName ?? string.Empty,
                PaymentType = p.PaymentType,
                Status = p.Status,
                PaymentMethod = p.PaymentMethod,
                Amount = p.Amount,
                Currency = p.Currency,
                DueDate = p.DueDate,
                PaidDate = p.PaidDate,
                BillingMonth = p.BillingMonth,
                BillingYear = p.BillingYear,
                LateFeeAmount = p.LateFeeAmount,
                Description = p.Description,
                TransactionId = p.TransactionId,
                PaymentProof = p.PaymentProof,
                Notes = p.Notes,
                CreatedAt = p.CreatedAt,
                LandlordBankAccount = p.Lease?.Landlord?.BankAccountNumber,
                LandlordBankName = p.Lease?.Landlord?.BankName,
                LandlordBankAccountHolder = p.Lease?.Landlord?.BankAccountHolder
            };
        }
    }
}
