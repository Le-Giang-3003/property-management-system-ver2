using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Enums
{
    public enum ApplicationStatus
    {
        Pending = 1,        // Chờ Landlord xét duyệt
        Approved = 2,       // Đã chấp nhận
        Rejected = 3,       // Bị từ chối
        Cancelled = 4       // Tenant hủy đơn
    }
}
