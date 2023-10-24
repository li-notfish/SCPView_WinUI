using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPView_WinUI.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            
        }

        public MainPageViewModel MainPage => Ioc.Default.GetService<MainPageViewModel>();
        public ItemPageViewModel ItemPage => Ioc.Default.GetService<ItemPageViewModel>();
        public ItemContentPageViewModel ItemContentPage => Ioc.Default.GetService<ItemContentPageViewModel>();
        public TalesListPageViewModel TalesListPage => Ioc.Default.GetService<TalesListPageViewModel>();
    }

    public class ParameterMessage : ValueChangedMessage<object>
    {
        public ParameterMessage(object value) : base(value)
        {
            
        }
    }
}
