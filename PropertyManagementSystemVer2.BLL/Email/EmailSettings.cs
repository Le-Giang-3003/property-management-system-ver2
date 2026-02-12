using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.BLL.Email
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public string SmtpUser { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
        public string SenderName { get; set; } = "PropertyMS";
        public string SenderEmail { get; set; } = string.Empty;
        public bool UseSsl { get; set; } = true;
        public string BaseUrl { get; set; } = "https://localhost:7001"; // URL gốc của web app
    }
}
