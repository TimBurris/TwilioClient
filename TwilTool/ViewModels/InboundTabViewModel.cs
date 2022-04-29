using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilTool.ViewModels
{
    public class InboundTabViewModel : ViewModelBase
    {
        private readonly Services.Sms.Abstractions.ISmsService _smsService;
        private readonly NLog.ILogger _logger;

        public InboundTabViewModel() : base(null) { }//designer only
        public InboundTabViewModel(Abstractions.INavigator navigator,
            Services.Sms.Abstractions.ISmsService smsService,
            NLog.ILogger logger)
            : base(navigator)
        {
            this._smsService = smsService;
            this._logger = logger;
            this.FilterToday = true;
        }


        public bool FilterToday
        {
            get { return GetField<bool>(); }
            set
            {
                SetField(value);
                if (value) this.FilterDate = DateTime.Today;
            }
        }

        public bool FilterYesterday
        {
            get { return GetField<bool>(); }
            set
            {
                SetField(value);
                if (value) this.FilterDate = DateTime.Today.AddDays(-1);
            }
        }

        public bool FilterOther
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        public DateTime FilterDate
        {
            get { return GetField<DateTime>(); }
            set { SetField(value); }
        }


        public bool HasError
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        public string ErrorMessage
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public bool WasSuccessful
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        public string SuccessMessage
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public ObservableCollection<Services.Sms.IncomingMessage> Messages { get; } = new ObservableCollection<Services.Sms.IncomingMessage>();

        #region GetMessages Command

        public NinjaMvvm.Wpf.RelayCommand GetMessagesCommand => new NinjaMvvm.Wpf.RelayCommand((param) => this.GetMessages());

        public async Task GetMessages()
        {
            try
            {
                this.HasError = false;
                this.ErrorMessage = null;
                this.WasSuccessful = false;
                this.SuccessMessage = null;
                this.Messages.Clear();
                this.IsBusy = true;

                var results = await _smsService.GetMessagesReceived(this.FilterDate);

                if (results.WasSuccessful)
                {
                    this.SuccessMessage = $"Received {results.ReceivedMessages.Count()} messages for {this.FilterDate.ToShortDateString()}";
                    this.WasSuccessful = true;

                    foreach (var message in results.ReceivedMessages.OrderByDescending(m => m.DateMessageRecieved))
                    {
                        this.Messages.Add(message);
                    }
                }
                else
                {
                    this.HasError = true;
                    this.ErrorMessage = results.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting messages");
                this.HasError = true;
                this.ErrorMessage = ex.Message;
            }
            finally
            {

                this.IsBusy = false;
            }
        }

        #endregion

    }
}
