using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using System.Security.Cryptography;

namespace PropertyManagementSystemVer2.BLL.Security
{
    public class OtpGenerator : IOtpGenerator
    {
        public string Generate(int length = 6)
        {
            var min = (int)Math.Pow(10, length - 1);   // 100000
            var max = (int)Math.Pow(10, length);        // 1000000
            var number = RandomNumberGenerator.GetInt32(min, max);
            return number.ToString();
        }
    }
}
