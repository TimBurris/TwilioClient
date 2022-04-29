using System;
using System.Collections.Generic;
using System.Text;

namespace TwilTool.Services.Sms
{
    public class GetMessageReceivedExectutionResult : ExecutionResult
    {

        public IEnumerable<IncomingMessage> ReceivedMessages { get; set; }

        public static GetMessageReceivedExectutionResult SuccessResult(IEnumerable<IncomingMessage> receivedMessages)
        {
            return new GetMessageReceivedExectutionResult() { WasSuccessful = true, ReceivedMessages = receivedMessages };
        }

        public static GetMessageReceivedExectutionResult FailedResult(string message, string rawResponse)
        {
            return new GetMessageReceivedExectutionResult() { Message = message };
        }
    }
}
