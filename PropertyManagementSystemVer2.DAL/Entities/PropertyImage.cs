using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Entities
{
    public class PropertyImage
    {
        public int Id { get; set; }

        public int PropertyId { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public string? Caption { get; set; }

        public bool IsPrimary { get; set; } // Ảnh đại diện

        public int DisplayOrder { get; set; } // Thứ tự hiển thị

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Property Property { get; set; } = null!;
    }
}
