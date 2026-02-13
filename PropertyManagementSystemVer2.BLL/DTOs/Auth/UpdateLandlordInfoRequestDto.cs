using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystemVer2.BLL.DTOs.Auth
{
    public class UpdateLandlordInfoRequestDto
    {
        [Required, MaxLength(20)]
        public string IdentityNumber { get; set; } = string.Empty;

        [Required, MaxLength(30)]
        public string BankAccountNumber { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string BankName { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string BankAccountHolder { get; set; } = string.Empty;
    }
}
