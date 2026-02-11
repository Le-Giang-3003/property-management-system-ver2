using PropertyManagementSystemVer2.BLL.DTOs.User;
using PropertyManagementSystemVer2.DAL.Entities;

namespace PropertyManagementSystemVer2.BLL.Mapping
{
    public class UserMapper
    {
        public static UserInfoDto ToDto(User user)
        {
            return new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                AvatarUrl = user.AvatarUrl,
                Role = user.Role.ToString(),
                IsTenant = user.IsTenant,
                IsLandlord = user.IsLandlord,
                IsEmailVerified = user.IsEmailVerified,
                IsPhoneVerified = user.IsPhoneVerified,
                IsIdentityVerified = user.IsIdentityVerified
            };
        }
    }
}
