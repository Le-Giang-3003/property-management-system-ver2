using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.BLL.DTOs.Auth
{
    public class AuthResultDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }

        public static AuthResultDto Ok(string? message = null, object? data = null)
            => new() { Success = true, Message = message, Data = data };

        public static AuthResultDto Fail(string message)
            => new() { Success = false, Message = message };
    }
}
