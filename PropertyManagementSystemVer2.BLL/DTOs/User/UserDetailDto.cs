namespace PropertyManagementSystemVer2.BLL.DTOs.User
{
    public class UserDetailDto : UserListDto
    {
        public string? Address { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsPhoneVerified { get; set; }
        public string? IdentityNumber { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountHolder { get; set; }
        public int OwnedPropertiesCount { get; set; }
        public int ActiveLeasesCount { get; set; }
        public int ApplicationsCount { get; set; }
    }
}
