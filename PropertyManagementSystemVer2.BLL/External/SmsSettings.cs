using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.BLL.External
{
    public class SmsSettings
    {
        public string Provider { get; set; } = "Twilio"; // "Twilio" hoặc "SpeedSMS" hoặc "Console"

        // === Twilio ===
        public string TwilioAccountSid { get; set; } = string.Empty;
        public string TwilioAuthToken { get; set; } = string.Empty;
        public string TwilioPhoneNumber { get; set; } = string.Empty; // +1234567890

        // === SpeedSMS (VN) ===
        public string SpeedSmsToken { get; set; } = string.Empty;
        public string SpeedSmsSender { get; set; } = "PropertyMS";
    }
}
