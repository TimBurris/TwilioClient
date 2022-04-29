using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Ninject;

namespace TwilTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.SetupIoC();
            this.SetupErrorHandling();
        }

        private void SetupIoC()
        {

            var kernel = new Ninject.StandardKernel(new Ninject.Modules.INinjectModule[] { new Ninjector() });

            Ninjector.Container = kernel;
        }
        private void SetupErrorHandling()
        {

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            System.Windows.Threading.Dispatcher.CurrentDispatcher.UnhandledException += CurrentDispatcher_UnhandledException;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var logger = Ninjector.Container.Get<NLog.ILogger>();
            logger.Error(e.Exception, "unhandled error caught by global handler");
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var logger = Ninjector.Container.Get<NLog.ILogger>();
            logger.Error(e.Exception, "unhandled error caught by global handler");
            e.Handled = true;
        }

        private static void CurrentDispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var logger = Ninjector.Container.Get<NLog.ILogger>();
            logger.Error(e.Exception, "unhandled error caught by global handler");
            e.Handled = true;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                var logger = Ninjector.Container.Get<NLog.ILogger>();
                logger.Error(ex, "unhandled error caught by global handler");

            }
        }
    }
}
