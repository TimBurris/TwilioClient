using System;
using System.Collections.Generic;
using System.Text;

namespace TwilTool.Services.Sms
{
    public class GetMessageStatusExectutionResult : ExecutionResult
    {
        public string RawResponse { get; set; }
        public MessageStatus Status { get; set; }

        public static GetMessageStatusExectutionResult SuccessResult(MessageStatus status, string rawResponse)
        {
            return new GetMessageStatusExectutionResult() { WasSuccessful = true, Status = status, RawResponse = rawResponse };
        }

        public static GetMessageStatusExectutionResult FailedResult(string message, string rawResponse)
        {
            return new GetMessageStatusExectutionResult() { Status = MessageStatus.Unknown, Message = message, RawResponse = rawResponse };
        }
    }
}
