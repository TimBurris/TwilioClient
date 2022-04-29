using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilTool.ViewModels
{
    public class OutboundTabViewModel : ViewModelBase
    {
        private readonly Services.PhoneNumberParsing.Abstractions.IPhoneNumberParser _phoneNumberParser;
        private readonly Services.Sms.Abstractions.ISmsService _smsService;
        private readonly ILogger _logger;

        public OutboundTabViewModel() : base(null) { }//designer only

        public OutboundTabViewModel(Abstractions.INavigator navigator,
            Services.PhoneNumberParsing.Abstractions.IPhoneNumberParser phoneNumberParser,
            Services.Sms.Abstractions.ISmsService smsService,
            NLog.ILogger logger)
            : base(navigator)
        {
            this._phoneNumberParser = phoneNumberParser;
            this._smsService = smsService;
            _logger = logger;
        }

        public bool HasSuccesses
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        public bool HasErrors
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        public bool HasWarnings
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        public string WarningMessages
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }
        public string ErrorMessages
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public string SuccessMessages
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public string MessageText
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }
        public string PhoneNumbers
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }


        public bool IsSendingMessages
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        public string SendProgress
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        #region SendSms Command

        public NinjaMvvm.Wpf.RelayCommand SendSmsCommand => new NinjaMvvm.Wpf.RelayCommand((param) => this.SendSms());

        public async Task SendSms()
        {
            this.HasErrors = false;
            this.HasSuccesses = false;
            this.HasWarnings = false;

            if (string.IsNullOrEmpty(this.PhoneNumbers))
            {
                this.Navigator.ShowDialog<MessageBoxViewModel>(initAction: (vm) =>
               {
                   vm.Init(title: "No Numbers",
                       message: "Please enter one or more phone numbers",
                       buttons: MessageBoxViewModel.MessageBoxButton.OK,
                       icon: MessageBoxViewModel.MessageBoxImage.Exclamation);
               });

                return;
            }

            if (string.IsNullOrEmpty(this.MessageText))
            {
                this.Navigator.ShowDialog<MessageBoxViewModel>(initAction: (vm) =>
                {
                    vm.Init(title: "No Message",
                        message: $"You must enter a message to send",
                        buttons: MessageBoxViewModel.MessageBoxButton.OK,
                        icon: MessageBoxViewModel.MessageBoxImage.Exclamation);
                });

                return;
            }

            var results = _phoneNumberParser.ParseMultipFromText(this.PhoneNumbers)
                            .GroupBy(r => r.IsValidNumber)
                            .ToDictionary((r) => r.Key);

            if (results.ContainsKey(false))
            {
                var parseFailures = results[false].ToList();
                var messageBoxResult = this.Navigator.ShowDialog<MessageBoxViewModel>(initAction: (vm) =>
                {
                    vm.Init(title: "Invalid Numbers",
                        message: $"{parseFailures.Count()} numbers in the list were invalid.  Do you still want to send to the valid numbers?",
                        buttons: MessageBoxViewModel.MessageBoxButton.YesNo,
                        icon: MessageBoxViewModel.MessageBoxImage.Exclamation);

                    vm.CanShowMoreDetails = true;
                    vm.MoreDetailsCaption = "Show invalid numbers";
                    vm.MoreDetailsMessage = string.Join(", ", parseFailures.Select(r => r.Result));
                });

                if (messageBoxResult.ViewResult != MessageBoxViewModel.MessageBoxResult.Yes)
                    return;
            }

            if (!results.ContainsKey(true))
            {
                this.Navigator.ShowDialog<MessageBoxViewModel>(initAction: (vm) =>
                {
                    vm.Init(title: "No Valid Numbers",
                        message: "No valid phone numbers to send to",
                        buttons: MessageBoxViewModel.MessageBoxButton.OK,
                        icon: MessageBoxViewModel.MessageBoxImage.Exclamation);
                });

                return;
            }

            try
            {
                this.IsSendingMessages = true;
                List<string> failures = new List<string>();
                List<string> successes = new List<string>();
                List<string> warnings = new List<string>();

                var numbers = results[true].Select(n => n.Result).ToList(); //results.Select(n => n.Result).ToList();
                int count = numbers.Count();

                numbers = numbers.Distinct().ToList();
                if (numbers.Count != count)
                {
                    int numDupes = count - numbers.Count;
                    warnings.Add($"{numDupes} number(s) were duplicated in the list but were only sent one message");

                    //now set to the number of distinct items
                    count = numbers.Count;
                }
                int i = 1;

                foreach (var phoneNumber in numbers)
                {
                    this.SendProgress = $"Sending message {i} of {count}. ({phoneNumber})";

                    try
                    {
                        var smsresult = await _smsService.SendSms(phoneNumber, this.MessageText);

                        if (smsresult.WasSuccessful)
                        {

                            _logger.Info(message: $"{phoneNumber} successfully queued");
                            successes.Add(phoneNumber + smsresult.RawResponse);
                        }
                        else
                        {
                            _logger.Info(message: $"{phoneNumber} failed: {smsresult.Message}");
                            failures.Add(smsresult.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Info(message: $"{phoneNumber} failed: {ex.Message}");
                        failures.Add(ex.Message);
                    }

                    i++;
                }


                if (failures.Any())
                {
                    var sb = new StringBuilder();
                    foreach (var reason in failures.GroupBy(x => x))
                    {
                        sb.AppendLine($"{reason.Count()} failed with {reason.Key}");
                    }

                    this.ErrorMessages = sb.ToString();
                    this.HasErrors = true;
                }

                if (successes.Any())
                {
                    this.SuccessMessages = $"{successes.Count} messages were successfully queued";
                    this.HasSuccesses = true;
                }

                if (warnings.Any())
                {
                    var sb = new StringBuilder();
                    foreach (var reason in warnings)
                    {
                        sb.AppendLine(reason);
                    }

                    this.WarningMessages = sb.ToString();
                    this.HasWarnings = true;
                }
            }
            finally
            {
                this.IsSendingMessages = false;
            }

            _logger.Info(message: this.ErrorMessages);
            _logger.Info(message: this.WarningMessages);
            _logger.Info(message: this.SuccessMessages);
        }

        #endregion

    }
}
