using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Enums
{
    public enum PaymentStatus
    {
        Pending = 1,        // Chờ thanh toán
        Completed = 2,      // Đã thanh toán thành công
        Failed = 3,         // Thanh toán thất bại
        Refunded = 4,       // Đã hoàn tiền
        Overdue = 5,        // Quá hạn thanh toán
        OverduePaid = 6     // Quá hạn/Đã thanh toán
    }
}
