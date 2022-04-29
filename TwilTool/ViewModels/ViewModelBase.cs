using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilTool.ViewModels
{
    public class ViewModelBase : NinjaMvvm.Wpf.WpfViewModelBase
    {
        public ViewModelBase(Abstractions.INavigator navigator)
        {
            this.Navigator = navigator;
        }

        protected Abstractions.INavigator Navigator { get; set; }

    }
}
