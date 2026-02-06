using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Enums
{
    public enum PresenceStatus
    {
        Online = 1,             // Đang online
        Away = 2,               // Vắng mặt (idle > 5 phút)
        Busy = 3,               // Bận
        Offline = 4             // Offline
    }
}
