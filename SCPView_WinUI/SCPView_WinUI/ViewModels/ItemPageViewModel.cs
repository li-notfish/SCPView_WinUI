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
using System.Threading;
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
        private CancellationTokenSource? _cts;
        private string? _loadedHref;

        public ItemPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Messenger.Register<ItemPageViewModel, ParameterMessage, string>(this, nameof(ItemPageViewModel).Replace("ViewModel", ""), (r, m) =>
            {
                if (m.Value is not null)
                {
                    var newSeries = m.Value as SCPSeries;
                    if (newSeries?.Href == r._loadedHref && r.ScpItemList.Count > 0)
                        return;
                    r.Series = newSeries;
                    r._loadedHref = newSeries?.Href;
                    GetSeries();
                }
            });
        }

        private async void GetSeries()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                ScpItemList.Clear();
                var data = await SCPService.GetItemList(Series.Href);
                if (token.IsCancellationRequested || data is null) return;

                foreach (var item in data)
                {
                    if (token.IsCancellationRequested) return;
                    ScpItemList.AddItem(item.Key, item.Value);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        [RelayCommand]
        public void GoBackPage()
        {
            _cts?.Cancel();
            _navigationService.GoBack();
        }

        [RelayCommand]
        public void GoToContent(object parameter)
        {
            _navigationService.NavigateTo(nameof(ItemContentPage), parameter: parameter);
        }
    }
}
