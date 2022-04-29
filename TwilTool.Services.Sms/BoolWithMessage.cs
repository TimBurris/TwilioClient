using System;
using System.Collections.Generic;
using System.Text;

namespace TwilTool.Services.Sms
{
    public class BoolWithMessage
    {

        public bool Result { get; set; }
        public string Message { get; set; }

        public static BoolWithMessage TrueResult()
        {
            return new BoolWithMessage() { Result = true };
        }

        public static BoolWithMessage FalseResult(string message)
        {
            return new BoolWithMessage() { Message = message };
        }
    }
}
