namespace PropertyManagementSystemVer2.DAL.Enums
{
    public enum LandlordApprovalStatus
    {
        None = 0,           // Chưa đăng ký Landlord
        Pending = 1,        // Đã submit, chờ Admin duyệt
        Approved = 2,       // Admin đã duyệt → IsLandlord = true
        Rejected = 3        // Admin từ chối → Member sửa và submit lại
    }
}
