using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Entities
{
    public class Revenue
    {
        public int Id { get; set; }

        public int PropertyId { get; set; }

        // Doanh thu theo tháng
        public int Month { get; set; } // 1-12

        public int Year { get; set; }

        // Các khoản thu
        public decimal TotalRentCollected { get; set; } // Tổng tiền thuê đã thu

        public decimal TotalDeposit { get; set; } // Tổng tiền cọc

        public decimal TotalServiceFees { get; set; } // Tổng phí dịch vụ

        public decimal TotalUtilities { get; set; } // Tổng tiền điện nước

        public decimal TotalLateFees { get; set; } // Tổng phí trả chậm

        public decimal TotalOtherIncome { get; set; } // Thu nhập khác

        // Các khoản chi
        public decimal TotalMaintenanceCost { get; set; } // Chi phí bảo trì

        public decimal TotalRefunds { get; set; } // Hoàn trả

        public decimal TotalOtherExpenses { get; set; } // Chi phí khác

        // Tổng kết
        public decimal GrossRevenue { get; set; } // Tổng doanh thu

        public decimal NetRevenue { get; set; } // Doanh thu ròng (sau chi phí)

        public decimal OccupancyRate { get; set; } // Tỷ lệ cho thuê (%)

        public int DaysOccupied { get; set; } // Số ngày đã cho thuê trong tháng

        public DateTime CalculatedAt { get; set; } // Thời điểm tính toán

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Property Property { get; set; } = null!;
    }
}
