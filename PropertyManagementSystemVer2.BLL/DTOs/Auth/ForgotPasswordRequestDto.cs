using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.BLL.DTOs.Auth
{
    public class ForgotPasswordRequestDto
    {
        [Required]
        public string Identifier { get; set; } = string.Empty;
    }
}
