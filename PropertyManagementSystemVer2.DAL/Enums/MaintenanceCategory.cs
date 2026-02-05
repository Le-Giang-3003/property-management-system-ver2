using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Enums
{
    public enum MaintenanceCategory
    {
        Plumbing = 1,       // Hệ thống nước
        Electrical = 2,     // Hệ thống điện
        HVAC = 3,           // Điều hòa, thông gió
        Appliance = 4,      // Thiết bị gia dụng
        Structural = 5,     // Kết cấu
        Painting = 6,       // Sơn, tường
        Cleaning = 7,       // Vệ sinh
        Security = 8,       // An ninh
        Other = 9           // Khác
    }
}
