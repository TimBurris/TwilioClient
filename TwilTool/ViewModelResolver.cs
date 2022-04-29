using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace TwilTool
{
    public class ViewModelResolver : Abstractions.IViewModelResolver
    {
        public TViewModel Resolve<TViewModel>()
        {
            return Ninjector.Container.Get<TViewModel>();
        }
    }
}
