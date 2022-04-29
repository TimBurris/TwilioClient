using System;
using System.Collections.Generic;
using System.Text;

namespace TwilTool.Services.Sms.Abstractions
{
    public interface ITwilioConfigurationService
    {
        TwilioSettings GetSettings();
    }
}
