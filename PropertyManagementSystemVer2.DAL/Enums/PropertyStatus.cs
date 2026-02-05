using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Enums
{
    public enum PropertyStatus
    {
        Draft = 1,          // Nháp, chưa gửi duyệt
        Pending = 2,        // Chờ Admin duyệt
        Approved = 3,       // Đã được duyệt, sẵn sàng cho thuê
        Rejected = 4,       // Bị từ chối bởi Admin
        Rented = 5,         // Đang được thuê
        Unavailable = 6     // Tạm ngưng cho thuê
    }
}
