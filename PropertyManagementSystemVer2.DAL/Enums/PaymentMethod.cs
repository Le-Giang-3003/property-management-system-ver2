using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Enums
{
    public enum PaymentMethod
    {
        Cash = 1,           // Tiền mặt
        BankTransfer = 2,   // Chuyển khoản ngân hàng
        CreditCard = 3,     // Thẻ tín dụng
        EWallet = 4,        // Ví điện tử (MoMo, ZaloPay, etc.)
        Check = 5           // Séc
    }
}
