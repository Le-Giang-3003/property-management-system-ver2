using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Enums
{
    public enum BookingStatus
    {
        Pending = 1,        // Chờ Landlord xác nhận
        Confirmed = 2,      // Đã xác nhận
        Cancelled = 3,      // Đã hủy
        Completed = 4,      // Đã hoàn thành
        NoShow = 5          // Tenant không đến
    }
}
