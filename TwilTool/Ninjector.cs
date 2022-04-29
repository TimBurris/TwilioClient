using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilTool
{
    class Ninjector : Ninject.Modules.NinjectModule
    {
        public static IKernel Container { get; set; }

        public override void Load()
        {
            Bind<NLog.ILogger>().ToMethod(p => NLog.LogManager.GetCurrentClassLogger(
                    p.Request?.Target?.Member?.DeclaringType ?? typeof(App)));

            Bind<Abstractions.INavigator>().To<Navigator>().InSingletonScope();
            Bind<Abstractions.IViewModelResolver>().To<ViewModelResolver>();


            Bind<Services.Sms.Abstractions.ISmsService>().To<Services.Sms.TwilioService>();
            Bind<Services.Sms.Abstractions.ITwilioConfigurationService>().To<TwilioConfigurationService>();

            Bind<Services.PhoneNumberParsing.Abstractions.IPhoneNumberParser>().To<Services.PhoneNumberParsing.PhoneNumberParser>();

        }
    }
}
