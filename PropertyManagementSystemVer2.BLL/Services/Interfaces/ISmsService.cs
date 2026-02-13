using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface ISmsService
    {
        Task SendOtpAsync(string phoneNumber, string otpCode);
        Task SendSmsAsync(string phoneNumber, string message);
    }
}
