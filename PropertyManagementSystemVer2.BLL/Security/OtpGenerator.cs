using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using System.Security.Cryptography;

namespace PropertyManagementSystemVer2.BLL.Security
{
    public class OtpGenerator : IOtpGenerator
    {
        public string Generate()
        {
            var number = RandomNumberGenerator.GetInt32(100000, 999999);
            return number.ToString();
        }
    }
}
