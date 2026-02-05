using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Enums
{
    public enum PaymentType
    {
        Rent = 1,           // Tiền thuê nhà hàng tháng
        Deposit = 2,        // Tiền đặt cọc
        ServiceFee = 3,     // Phí dịch vụ
        Utility = 4,        // Tiền điện, nước, internet
        Maintenance = 5,    // Chi phí bảo trì
        LateFee = 6         // Phí trả chậm
    }
}
