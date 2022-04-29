﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilTool.Abstractions
{
    public interface INavigator
    {
        /// <summary>
        /// opens a new view
        /// </summary>
        /// <param name="viewModel"></param>
        TViewModel ShowDialog<TViewModel>() where TViewModel : ViewModels.ViewModelBase;

        /// <summary>
        /// opens a new view
        /// </summary>
        TViewModel ShowDialog<TViewModel>(Action<TViewModel> initAction) where TViewModel : ViewModels.ViewModelBase;

        /// <summary>
        /// closes a view
        /// </summary>
        /// <param name="viewModel"></param>
        void CloseDialog(ViewModels.ViewModelBase viewModel);

        /// <summary>
        /// navigates to a viewmodel
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <returns></returns>
        TViewModel NavigateTo<TViewModel>() where TViewModel : ViewModels.ViewModelBase;

        /// <summary>
        /// navigates to a viewmodel
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="initAction"></param>
        /// <returns></returns>
        TViewModel NavigateTo<TViewModel>(Action<TViewModel> initAction) where TViewModel : ViewModels.ViewModelBase;
    }
}
