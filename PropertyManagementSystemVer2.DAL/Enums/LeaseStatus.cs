using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Enums
{
    public enum LeaseStatus
    {
        Active = 1,         // Đang hiệu lực
        Expired = 2,        // Hết hạn
        Terminated = 3,     // Chấm dứt trước hạn
        Pending = 4         // Chờ bắt đầu
    }
}
