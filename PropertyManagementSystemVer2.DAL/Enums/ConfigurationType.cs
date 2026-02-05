using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Enums
{
    public enum ConfigurationType
    {
        ServiceFee = 1,         // Phí dịch vụ (%)
        MaxUploadSize = 2,      // Kích thước upload tối đa
        DefaultCurrency = 3,    // Đơn vị tiền tệ mặc định
        BookingDuration = 4,    // Thời gian mỗi lịch hẹn
        ApplicationExpiry = 5,  // Thời gian hết hạn đơn xin thuê
        Other = 99             // Khác
    }
}
