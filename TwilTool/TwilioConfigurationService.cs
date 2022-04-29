using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwilTool.Services.Sms;

namespace TwilTool
{
    class TwilioConfigurationService : TwilTool.Services.Sms.Abstractions.ITwilioConfigurationService
    {
        public TwilioSettings GetSettings()
        {
            return new TwilioSettings()
            {
                AccountSid = Properties.Settings.Default.TwilioSettings_AccountSid,
                AuthToken = Properties.Settings.Default.TwilioSettings_AuthToken,
                FromNumber = Properties.Settings.Default.TwilioSettings_FromNumber,
            };
        }
    }
}
