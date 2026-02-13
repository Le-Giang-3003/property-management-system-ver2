using PropertyManagementSystemVer2.BLL.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.BLL.Helpers
{
    public class LandlordRegistrationHelper
    {
        public static List<string> Validate(SubmitLandlordRequestDto req)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(req.IdentityNumber))
                errors.Add("Số CMND/CCCD không được để trống");
            else if (req.IdentityNumber.Trim().Length is < 9 or > 12)
                errors.Add("Số CMND/CCCD phải từ 9-12 ký tự");

            if (string.IsNullOrWhiteSpace(req.BankAccountNumber))
                errors.Add("Số tài khoản không được để trống");

            if (string.IsNullOrWhiteSpace(req.BankName))
                errors.Add("Tên ngân hàng không được để trống");

            if (string.IsNullOrWhiteSpace(req.BankAccountHolder))
                errors.Add("Tên chủ tài khoản không được để trống");

            return errors;
        }

        public static string BuildApprovalEmail(string name) => $@"
            <div style='font-family:Segoe UI,Arial;max-width:500px;margin:30px auto;background:#fff;border-radius:10px;box-shadow:0 2px 10px rgba(0,0,0,0.08);overflow:hidden'>
              <div style='background:#198754;padding:25px;text-align:center'><h2 style='color:#fff;margin:0'>✅ Đơn đã được duyệt!</h2></div>
              <div style='padding:25px'>
                <p>Xin chào <strong>{name}</strong>,</p>
                <p>Chúc mừng! Bạn đã được cấp quyền <strong>Landlord</strong>. Bạn có thể bắt đầu đăng tin cho thuê bất động sản.</p>
              </div>
              <div style='padding:15px;background:#f8f9fa;text-align:center;font-size:12px;color:#999'>&copy; {DateTime.Now.Year} PropertyMS</div>
            </div>";

        public static string BuildRejectionEmail(string name, string reason) => $@"
            <div style='font-family:Segoe UI,Arial;max-width:500px;margin:30px auto;background:#fff;border-radius:10px;box-shadow:0 2px 10px rgba(0,0,0,0.08);overflow:hidden'>
              <div style='background:#dc3545;padding:25px;text-align:center'><h2 style='color:#fff;margin:0'>❌ Đơn bị từ chối</h2></div>
              <div style='padding:25px'>
                <p>Xin chào <strong>{name}</strong>,</p>
                <p>Đơn đăng ký Landlord của bạn bị từ chối:</p>
                <div style='background:#fff3cd;border:1px solid #ffc107;border-radius:6px;padding:12px;margin:10px 0'>
                  <strong>Lý do:</strong> {reason}
                </div>
                <p>Bạn có thể chỉnh sửa thông tin và gửi lại.</p>
              </div>
              <div style='padding:15px;background:#f8f9fa;text-align:center;font-size:12px;color:#999'>&copy; {DateTime.Now.Year} PropertyMS</div>
            </div>";
    }
}
