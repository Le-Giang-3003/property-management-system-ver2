using PropertyManagementSystemVer2.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Entities
{
    public class SystemConfiguration
    {
        public int Id { get; set; }

        public string Key { get; set; } = string.Empty; // Unique key

        public string Value { get; set; } = string.Empty;

        public ConfigurationType Type { get; set; }

        public string? Description { get; set; }

        public string? Unit { get; set; } // Đơn vị (%, MB, VND, etc.)

        public bool IsActive { get; set; } = true;

        public int? UpdatedBy { get; set; } // Admin ID

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
