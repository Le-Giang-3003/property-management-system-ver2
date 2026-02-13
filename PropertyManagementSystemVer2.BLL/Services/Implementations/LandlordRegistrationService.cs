using PropertyManagementSystemVer2.BLL.DTOs.Auth;
using PropertyManagementSystemVer2.BLL.Helpers;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.BLL.Services.Implementations
{
    /// <summary>
    /// This is LandlordRegistrationService
    /// </summary>
    /// <seealso cref="PropertyManagementSystemVer2.BLL.Services.Interfaces.ILandlordRegistrationService" />
    public class LandlordRegistrationService : ILandlordRegistrationService
    {
        /// <summary>
        /// The unit of work
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;
        /// <summary>
        /// The email service
        /// </summary>
        private readonly IEmailService _emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LandlordRegistrationService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="emailService">The email service.</param>
        public LandlordRegistrationService(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        /// <summary>
        /// Submits the asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async Task<AuthResultDto> SubmitAsync(int userId, SubmitLandlordRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return AuthResultDto.Fail("Người dùng không tồn tại.");
            if (user.IsLandlord) return AuthResultDto.Fail("Bạn đã là Landlord.");
            if (user.LandlordStatus == LandlordApprovalStatus.Pending)
                return AuthResultDto.Fail("Đơn đang chờ duyệt. Vui lòng chờ Admin xử lý.");

            var errors = LandlordRegistrationHelper.Validate(request);
            if (errors.Count > 0) return AuthResultDto.Fail(string.Join("; ", errors));

            user.IdentityNumber = request.IdentityNumber.Trim();
            user.BankAccountNumber = request.BankAccountNumber.Trim();
            user.BankName = request.BankName.Trim();
            user.BankAccountHolder = request.BankAccountHolder.Trim();
            user.LandlordStatus = LandlordApprovalStatus.Pending;
            user.LandlordRejectionReason = null;
            user.LandlordSubmittedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return AuthResultDto.Ok("Đơn đăng ký Landlord đã được gửi. Vui lòng chờ Admin duyệt.");
        }

        /// <summary>
        /// Resubmits the asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async Task<AuthResultDto> ResubmitAsync(int userId, SubmitLandlordRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return AuthResultDto.Fail("Người dùng không tồn tại.");
            if (user.IsLandlord) return AuthResultDto.Fail("Bạn đã là Landlord.");
            if (user.LandlordStatus != LandlordApprovalStatus.Rejected)
                return AuthResultDto.Fail("Chỉ có thể gửi lại đơn đã bị từ chối.");

            var errors = Validate(request);
            if (errors.Count > 0) return AuthResultDto.Fail(string.Join("; ", errors));

            user.IdentityNumber = request.IdentityNumber.Trim();
            user.BankAccountNumber = request.BankAccountNumber.Trim();
            user.BankName = request.BankName.Trim();
            user.BankAccountHolder = request.BankAccountHolder.Trim();
            user.LandlordStatus = LandlordApprovalStatus.Pending;
            user.LandlordRejectionReason = null;
            user.LandlordSubmittedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return AuthResultDto.Ok("Đơn đã được gửi lại. Vui lòng chờ Admin duyệt.");
        }

        /// <summary>
        /// Gets my status asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public async Task<AuthResultDto> GetMyStatusAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return AuthResultDto.Fail("Người dùng không tồn tại.");

            return AuthResultDto.Ok(data: new LandlordStatusDto
            {
                Status = user.LandlordStatus.ToString(),
                IsLandlord = user.IsLandlord,
                RejectionReason = user.LandlordRejectionReason,
                IdentityNumber = user.IdentityNumber,
                BankAccountNumber = user.BankAccountNumber,
                BankName = user.BankName,
                BankAccountHolder = user.BankAccountHolder
            });
        }

        /// <summary>
        /// Gets the pending list asynchronous.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        public async Task<AuthResultDto> GetPendingListAsync(int page = 1, int pageSize = 20)
        {
            var total = await _unitOfWork.Users.CountPendingLandlordsAsync();
            var users = await _unitOfWork.Users.GetPendingLandlordsAsync(page, pageSize);

            var items = users.Select(u => new LandlordStatusDto
            {
                Status = u.LandlordStatus.ToString(),
                IsLandlord = u.IsLandlord,
                IdentityNumber = u.IdentityNumber,
                BankAccountNumber = u.BankAccountNumber,
                BankName = u.BankName,
                BankAccountHolder = u.BankAccountHolder
            }).ToList();

            return AuthResultDto.Ok(data: new { items, total, page, pageSize });
        }

        /// <summary>
        /// Reviews the asynchronous.
        /// </summary>
        /// <param name="adminId">The admin identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async Task<AuthResultDto> ReviewAsync(int adminId, ReviewLandlordRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user == null) return AuthResultDto.Fail("Người dùng không tồn tại.");

            if (user.LandlordStatus != LandlordApprovalStatus.Pending)
                return AuthResultDto.Fail($"Trạng thái hiện tại là '{user.LandlordStatus}', không thể duyệt.");

            if (!request.IsApproved && string.IsNullOrWhiteSpace(request.RejectionReason))
                return AuthResultDto.Fail("Vui lòng nhập lý do từ chối.");

            user.LandlordReviewedBy = adminId;
            user.LandlordReviewedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            if (request.IsApproved)
            {
                user.LandlordStatus = LandlordApprovalStatus.Approved;
                user.IsLandlord = true;
                user.IsIdentityVerified = true;
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                try { await _emailService.SendEmailAsync(user.Email, "Đơn đăng ký Landlord đã được duyệt - PropertyMS", BuildApprovalEmail(user.FullName)); } catch { }

                return AuthResultDto.Ok($"Đã duyệt. {user.FullName} giờ là Landlord.");
            }
            else
            {
                user.LandlordStatus = LandlordApprovalStatus.Rejected;
                user.LandlordRejectionReason = request.RejectionReason;
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                try { await _emailService.SendEmailAsync(user.Email, "Đơn đăng ký Landlord bị từ chối - PropertyMS", BuildRejectionEmail(user.FullName, request.RejectionReason!)); } catch { }

                return AuthResultDto.Ok("Đã từ chối. Member sẽ nhận email thông báo.");
            }
        }
    }
}
