using System;
using System.Collections.Generic;
using System.Text;

namespace TwilTool.Services.Sms
{
    public class IncomingMessage
    {
        public string MessageIdentifier { get; set; }
        public string MessageBody { get; set; }
        public string FromNumber { get; set; }
        public DateTime DateMessageRecieved { get; set; }
    }
}
