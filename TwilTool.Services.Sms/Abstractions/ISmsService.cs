using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TwilTool.Services.Sms.Abstractions
{
    public interface ISmsService
    {
        Task<string> SmsConfigMessage();
        Task<BoolWithMessage> IsSmsAvailable();
        Task<SendMessageExectutionResult> SendSms(string phoneNumber, string message);
        Task<GetMessageStatusExectutionResult> GetMessageStatus(string messageIdentifier);
        Task<GetMessageReceivedExectutionResult> GetMessagesReceived(DateTime dateSent);
    }
}
