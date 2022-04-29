using System;
using System.Collections.Generic;
using System.Text;

namespace TwilTool.Services.Sms
{
    public class SendMessageExectutionResult : ExecutionResult
    {
        public string RawResponse { get; set; }
        public string MessageIdentifier { get; set; }


        public static SendMessageExectutionResult SuccessResult(string messageIdentifier, string rawResponse)
        {
            return new SendMessageExectutionResult() { WasSuccessful = true, MessageIdentifier = messageIdentifier, RawResponse = rawResponse };
        }

        public static SendMessageExectutionResult FailedResult(string message, string rawResponse)
        {
            return new SendMessageExectutionResult() { Message = message, RawResponse = rawResponse };
        }
    }
}
