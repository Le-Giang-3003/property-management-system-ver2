using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Enums
{
    public enum MessageType
    {
        Text = 1,               // Tin nhắn văn bản thông thường
        Image = 2,              // Hình ảnh
        Document = 3,           // Tài liệu (PDF, DOCX, etc.)
        PropertyLink = 4,       // Link đến Property
        BookingRequest = 5,     // Yêu cầu đặt lịch xem nhà
        LeaseDocument = 6,      // Tài liệu hợp đồng
        PaymentReceipt = 7,     // Biên lai thanh toán
        SystemMessage = 8,      // Tin nhắn hệ thống (auto-generated)
        Video = 9,              // Video
        Audio = 10              // Voice message
    }
}
