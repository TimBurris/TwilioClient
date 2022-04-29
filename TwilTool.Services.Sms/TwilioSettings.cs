using System;
using System.Collections.Generic;
using System.Text;

namespace TwilTool.Services.Sms
{
    public class TwilioSettings
    {

        public string FromNumber { get; set; }

        public string AccountSid { get; set; }

        public string AuthToken { get; set; }
    }
}
