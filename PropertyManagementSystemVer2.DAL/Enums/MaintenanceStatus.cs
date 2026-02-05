using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Enums
{
    public enum MaintenanceStatus
    {
        Open = 1,           // Mới tạo, chưa xử lý
        InProgress = 2,     // Đang xử lý
        Resolved = 3,       // Đã giải quyết
        Cancelled = 4       // Đã hủy
    }
}
