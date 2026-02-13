using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.BLL.DTOs.Auth
{
    public class SendPhoneOtpRequestDto
    {
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
