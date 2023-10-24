using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SCPView_WinUI.Data;
using SCPView_WinUI.Data.Model;
using SCPView_WinUI.Pages;
using SCPView_WinUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPView_WinUI.ViewModels
{
    public partial class ItemPageViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private SCPSeries series;

        [ObservableProperty]
        private ObservableGroupedCollection<string, List<SCPItemList>> scpItemList = new ObservableGroupedCollection<string, List<SCPItemList>>();

        private INavigationService _navigationService;

        public ItemPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Messenger.Register<ItemPageViewModel, ParameterMessage, string>(this, nameof(ItemPageViewModel).Replace("ViewModel", ""), (r, m) =>
            {
                if (m.Value is not null)
                {
                    r.Series = m.Value as SCPSeries;
                    ScpItemList.Clear();
                    GetSeries();
                }
            });
        }

        private async void GetSeries()
        {
            try
            {
                var data = await SCPService.GetItemList(Series.Href);
                if(data is not null)
                {
                    foreach (var item in data)
                    {
                        ScpItemList.AddItem(item.Key,item.Value);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            
        }

        [RelayCommand]
        public void GoBackPage()
        {
            _navigationService.GoBack();
        }

        [RelayCommand]
        public void GoToContent(object parameter)
        {
            _navigationService.NavigateTo(nameof(ItemContentPage),parameter: parameter);
        }
    }
}
