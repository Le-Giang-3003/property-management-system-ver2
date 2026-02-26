using PropertyManagementSystemVer2.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Entities
{
    public class Property
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public PropertyType PropertyType { get; set; }

        public PropertyStatus Status { get; set; } = PropertyStatus.Pending;

        // Địa chỉ
        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string District { get; set; } = string.Empty;

        public string? Ward { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        // Thông tin chi tiết
        public decimal Area { get; set; } // Diện tích (m2)

        public int Bedrooms { get; set; }

        public int Bathrooms { get; set; }

        public int? Floors { get; set; }

        // Giá thuê
        public decimal MonthlyRent { get; set; }

        public decimal DepositAmount { get; set; } // Tiền đặt cọc

        public string Currency { get; set; } = "VND";

        // Tiện ích (có thể lưu dạng JSON hoặc bảng riêng)
        public string? Amenities { get; set; } // JSON: ["Parking", "Elevator", "Swimming Pool"]

        // Quy định
        public bool AllowPets { get; set; }

        public bool AllowSmoking { get; set; }

        public int? MaxOccupants { get; set; }

        // Thông tin Landlord
        public int LandlordId { get; set; }

        // Lý do reject (nếu Admin từ chối)
        public string? RejectionReason { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public int? ApprovedBy { get; set; } // Admin ID

        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public User Landlord { get; set; } = null!;

        public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();

        public ICollection<RentalApplication> RentalApplications { get; set; } = new List<RentalApplication>();

        public ICollection<Lease> Leases { get; set; } = new List<Lease>();

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();

        public Revenue? Revenue { get; set; }
    }
}
