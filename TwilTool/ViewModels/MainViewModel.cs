using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TwilTool.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly Abstractions.IViewModelResolver _viewModelResolver;
        private readonly Services.Sms.Abstractions.ISmsService _smsService;

        public MainViewModel() : base(null) { }//designer only

        public MainViewModel(Abstractions.INavigator navigator,
            Abstractions.IViewModelResolver viewModelResolver,
            Services.Sms.Abstractions.ISmsService smsService)
            : base(navigator)
        {
            this._viewModelResolver = viewModelResolver;
            this._smsService = smsService;
        }

        protected async override Task<bool> OnReloadDataAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _smsService.IsSmsAvailable();
                this.HasTwilioAccess = result.Result;
                this.Message = result.Message;
                this.ConfigurationMessage = await _smsService.SmsConfigMessage();
            }
            catch (Exception ex)
            {
                this.HasTwilioAccess = false;
                this.Message = $"An unexpected error has occurect initializing.  The error is '{ex.Message}'";
                throw;
            }
            return await base.OnReloadDataAsync(cancellationToken);
        }


        public string ConfigurationMessage
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public string Message
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public bool HasTwilioAccess
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        public InboundTabViewModel InboundVM => _viewModelResolver.Resolve<InboundTabViewModel>();
        public OutboundTabViewModel OutboundVM => _viewModelResolver.Resolve<OutboundTabViewModel>();
    }
}
