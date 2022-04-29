using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwilTool.Services.Sms;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Text;

namespace TwilTool.Services.Sms
{
    public class TwilioService : Abstractions.ISmsService
    {
        private Abstractions.ITwilioConfigurationService _configurationService;

        private static MessageResource.StatusEnum[] _pendingStatuses = new MessageResource.StatusEnum[] {
            MessageResource.StatusEnum.Queued,
            MessageResource.StatusEnum.Sending,
            MessageResource.StatusEnum.Accepted,
        };
        private static MessageResource.StatusEnum[] _sentStatuses = new MessageResource.StatusEnum[] {
            MessageResource.StatusEnum.Sent,
            MessageResource.StatusEnum.Delivered,
        };

        private static MessageResource.StatusEnum[] _errorStatuses = new MessageResource.StatusEnum[] {
            MessageResource.StatusEnum.Failed,
            MessageResource.StatusEnum.Undelivered,
        };

        public TwilioService(Abstractions.ITwilioConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }
        public Task<string> SmsConfigMessage()
        {
            var settings = _configurationService.GetSettings();

            return Task.FromResult($"AccountSid: {settings?.AccountSid}\r\nAuthToken: {settings?.AuthToken}\r\nFrom PhoneNumber: {settings?.FromNumber}");
        }

        public async Task<BoolWithMessage> IsSmsAvailable()
        {
            var settings = _configurationService.GetSettings();

            if (settings == null)
                return BoolWithMessage.FalseResult("No Twilio settings found");

            var sb = new StringBuilder();

            if (string.IsNullOrEmpty(settings.AccountSid))
                sb.AppendLine("Twilio Account Sid cannot be empty");
            if (string.IsNullOrEmpty(settings.AuthToken))
                sb.AppendLine("Twilio Auth Token cannot be empty");
            if (string.IsNullOrEmpty(settings.FromNumber))
                sb.AppendLine("Twilio From Number cannot be empty");

            var message = sb.ToString();

            if (string.IsNullOrEmpty(message))
                return BoolWithMessage.TrueResult();
            else
                return BoolWithMessage.FalseResult(message);
        }

        public async Task<SendMessageExectutionResult> SendSms(string phoneNumber, string message)
        {
            var settings = _configurationService.GetSettings();

            TwilioClient.Init(settings.AccountSid, settings.AuthToken);
            try
            {
                var messageResult = await MessageResource.CreateAsync(
                      to: new Twilio.Types.PhoneNumber(phoneNumber),
                      from: new Twilio.Types.PhoneNumber(settings.FromNumber),
                      body: message,
                      forceDelivery: true);

                var raw = Newtonsoft.Json.JsonConvert.SerializeObject(message, formatting: Newtonsoft.Json.Formatting.Indented);

                if (String.IsNullOrEmpty(messageResult.Sid))
                    return SendMessageExectutionResult.FailedResult(message: "Failed to send message", rawResponse: raw);
                else
                    return SendMessageExectutionResult.SuccessResult(messageIdentifier: messageResult.Sid, rawResponse: raw);
            }
            catch (Twilio.Exceptions.RestException ex)
            {
                return SendMessageExectutionResult.FailedResult("Failed to send message", ex.Message);

            }
        }

        public async Task<GetMessageStatusExectutionResult> GetMessageStatus(string messageIdentifier)
        {
            var settings = _configurationService.GetSettings();

            TwilioClient.Init(settings.AccountSid, settings.AuthToken);

            try
            {
                var message = await MessageResource.FetchAsync(messageIdentifier);
                var raw = Newtonsoft.Json.JsonConvert.SerializeObject(message, formatting: Newtonsoft.Json.Formatting.Indented);

                if (_pendingStatuses.Contains(message.Status))
                    return GetMessageStatusExectutionResult.SuccessResult(MessageStatus.Pending, raw);
                else if (_sentStatuses.Contains(message.Status))
                    return GetMessageStatusExectutionResult.SuccessResult(MessageStatus.Sent, raw);
                else if (_errorStatuses.Contains(message.Status))
                    return GetMessageStatusExectutionResult.SuccessResult(MessageStatus.Failed, raw);
                else
                    return GetMessageStatusExectutionResult.SuccessResult(MessageStatus.Unknown, raw);
            }
            catch (Twilio.Exceptions.RestException ex)
            {
                return GetMessageStatusExectutionResult.FailedResult("Failed to get message", ex.Message);

            }
        }

        public async Task<GetMessageReceivedExectutionResult> GetMessagesReceived(DateTime dateSent)
        {
            var settings = _configurationService.GetSettings();

            TwilioClient.Init(settings.AccountSid, settings.AuthToken);

            try
            {
                var result = new GetMessageReceivedExectutionResult();
                var messages = await MessageResource.ReadAsync(to: new Twilio.Types.PhoneNumber(settings.FromNumber), dateSent: dateSent);

                //whether successful or not, whichever message we DID get need to be returned
                result.ReceivedMessages = messages.Select(m => new IncomingMessage()
                {
                    MessageIdentifier = m.Sid,
                    MessageBody = m.Body,
                    FromNumber = m.From.ToString(),
                    DateMessageRecieved = m.DateSent.GetValueOrDefault()
                }).ToList();

                result.WasSuccessful = true;
                return result;
            }
            catch (Twilio.Exceptions.RestException ex)
            {
                return GetMessageReceivedExectutionResult.FailedResult("Failed to get messages", ex.Message);

            }
        }
    }
}
