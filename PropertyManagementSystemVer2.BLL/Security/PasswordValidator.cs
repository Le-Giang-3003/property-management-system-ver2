using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using System.Text.RegularExpressions;

namespace PropertyManagementSystemVer2.BLL.Security
{
    public class PasswordValidator : IPasswordValidator
    {
        public bool IsStrong(string password)
        {
            if (password.Length < 8) return false;
            if (!Regex.IsMatch(password, @"[A-Z]")) return false;
            if (!Regex.IsMatch(password, @"[a-z]")) return false;
            if (!Regex.IsMatch(password, @"[0-9]")) return false;
            if (!Regex.IsMatch(password, @"[\W_]")) return false;
            return true;
        }
    }
}
