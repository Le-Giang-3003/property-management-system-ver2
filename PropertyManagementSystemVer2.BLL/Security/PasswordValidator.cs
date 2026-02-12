using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using System.Text.RegularExpressions;

namespace PropertyManagementSystemVer2.BLL.Security
{
    public class PasswordValidator : IPasswordValidator
    {
        public List<string> Validate(string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add("Mật khẩu không được để trống");
                return errors;
            }

            if (password.Length < 8)
                errors.Add("Tối thiểu 8 ký tự");

            if (!Regex.IsMatch(password, @"[A-Z]"))
                errors.Add("Phải có ít nhất 1 chữ hoa (A-Z)");

            if (!Regex.IsMatch(password, @"[a-z]"))
                errors.Add("Phải có ít nhất 1 chữ thường (a-z)");

            if (!Regex.IsMatch(password, @"[0-9]"))
                errors.Add("Phải có ít nhất 1 số (0-9)");

            if (!Regex.IsMatch(password, @"[\W_]"))
                errors.Add("Phải có ít nhất 1 ký tự đặc biệt (!@#$%...)");

            return errors;
        }
        public bool IsStrong(string password)
        {
            return Validate(password).Count == 0;
        }
    }
}
