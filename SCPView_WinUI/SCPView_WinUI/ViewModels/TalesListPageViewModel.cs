using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SCPView_WinUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPView_WinUI.ViewModels
{
    public partial class TalesListPageViewModel : ObservableRecipient
    {
        private INavigationService _navigationService;

        public TalesListPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        [RelayCommand]
        public void GoBackPage()
        {
            _navigationService.GoBack();
        }
    }
}
