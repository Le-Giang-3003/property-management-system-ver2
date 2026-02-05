using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Enums
{
    public enum UserRole
    {
        Admin = 1,      // Quản trị viên hệ thống
        Member = 2      // Thành viên (có thể là Landlord hoặc Tenant)
    }
}
