using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Enums
{
    public enum PropertyStatus
    {
        Pending = 2,        // Chờ Admin duyệt
        Approved = 3,       // Đã được duyệt, nhưng chưa hiển thị cho thuê (Landlord cần đăng)
        Rejected = 4,       // Bị từ chối bởi Admin
        Rented = 5,         // Đang được thuê
        Unavailable = 6,    // Tạm ngưng cho thuê (đã bị ẩn bởi Landlord)
        Available = 7,      // Đã được Landlord đăng công khai lên trang cho thuê
        Blocked = 8         // Bị khóa bởi Admin do vi phạm
    }
}
