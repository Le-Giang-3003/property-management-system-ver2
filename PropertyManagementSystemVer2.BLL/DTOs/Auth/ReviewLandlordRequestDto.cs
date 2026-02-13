using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.BLL.DTOs.Auth
{
    public class ReviewLandlordRequestDto
    {

        public int UserId { get; set; }
        public bool IsApproved { get; set; }
        public string? RejectionReason { get; set; }  // Bắt buộc khi reject
    }
}
