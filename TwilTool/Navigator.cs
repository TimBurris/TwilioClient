using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilTool
{


    //ideally INavigator would be implemented by MainWindow, but since we can use ninject to instantiate it (app.xaml is doing that) so we'll use this class which will be completely dependent on MainWindow
    public class Navigator : Abstractions.INavigator
    {
        private MainWindow _window;
        private NLog.ILogger _logger;
        private readonly Abstractions.IViewModelResolver _viewModelResolver;

        public Navigator(NLog.ILogger logger, Abstractions.IViewModelResolver viewModelResolver)
        {
            _window = (MainWindow)App.Current.MainWindow;
            _logger = logger;
            this._viewModelResolver = viewModelResolver;
        }

        private List<DialogView> _dialogViews = new List<DialogView>();

        public void CloseDialog(ViewModels.ViewModelBase viewModel)
        {
            var dialog = _dialogViews.SingleOrDefault(x => x.ViewModel == viewModel);

            if (dialog == null)
            {
                _logger.Warn($@"A ViewModel was requested to be closed but not dialog corresponding the ViewModel was in our list of open dialogs.
                            While not an error, probably means someone is either closing something that did never actually opened, closing something multiple times, or closed a dialog that was not opened by Navigator.
                            ViewModel was asked to be closed was: {viewModel?.GetType()?.FullName}");
            }
            else
                CloseDialogView(dialog);
        }

        public TViewModel NavigateTo<TViewModel>() where TViewModel : ViewModels.ViewModelBase
        {
            return this.NavigateTo<TViewModel>(initAction: null);
        }

        public TViewModel NavigateTo<TViewModel>(Action<TViewModel> initAction) where TViewModel : ViewModels.ViewModelBase
        {
            //TODO: in future if more added, simply choose what todo here
            throw new NotSupportedException();

            /*
            var vm = this.CreateViewModel(initAction);

            _window.mainContent.Content = vm;

            return vm;
            */
        }

        public TViewModel ShowDialog<TViewModel>() where TViewModel : ViewModels.ViewModelBase
        {
            return ShowDialog<TViewModel>(initAction: null);
        }

        public TViewModel ShowDialog<TViewModel>(Action<TViewModel> initAction) where TViewModel : ViewModels.ViewModelBase
        {
            var viewModel = this.CreateViewModel(initAction);

            var dialogWindow = new DialogWindow();
            _dialogViews.Add(new DialogView() { ViewModel = viewModel, Window = dialogWindow });

            dialogWindow.DataContext = viewModel;
            dialogWindow.Closed += DialogWindow_Closed;

            dialogWindow.Owner = _window;//TODO: maybe the owner should be the "current dialog"?
            dialogWindow.ShowDialog();

            return viewModel;
        }


        private TViewModel CreateViewModel<TViewModel>(Action<TViewModel> initAction) where TViewModel : ViewModels.ViewModelBase
        {
            var vm = _viewModelResolver.Resolve<TViewModel>();

            //if they gave us something to init with, init it!
            initAction?.Invoke(vm);

            return vm;
        }

        private void DialogWindow_Closed(object sender, EventArgs e)
        {
            var window = (DialogWindow)sender;
            var dialog = _dialogViews.SingleOrDefault(x => x.Window == window);

            if (dialog == null)
                _logger.Warn($@"A dialog was closed, but that dialog was not found in our list of open dialogs.  
                            The windows that closed was: {sender.GetType()?.FullName}");
            else
                CloseDialogView(dialog);
        }

        private void CloseDialogView(DialogView dialog)
        {
            dialog.Window.Close();
            dialog.Window.Closed -= DialogWindow_Closed;
            _dialogViews.Remove(dialog);
        }

        private class DialogView
        {
            public ViewModels.ViewModelBase ViewModel { get; set; }
            public DialogWindow Window { get; set; }
        }
    }
}
