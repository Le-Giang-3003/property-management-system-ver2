using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.BLL.DTOs.Auth
{
    public class LandlordStatusDto
    {
        public string Status { get; set; } = "None"; //"None" | "Pending" | "Approved" | "Rejected"
        public bool IsLandlord { get; set; }
        public string? RejectionReason { get; set; }

        // Thông tin đã submit (để prefill form khi resubmit)
        public string? IdentityNumber { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountHolder { get; set; }
    }
}
