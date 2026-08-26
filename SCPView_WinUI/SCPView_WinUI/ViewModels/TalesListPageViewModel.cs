using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using SCPView_WinUI.Data;
using SCPView_WinUI.Data.Model;
using SCPView_WinUI.Pages;
using SCPView_WinUI.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace SCPView_WinUI.ViewModels
{
    public partial class TalesListPageViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private SCPSeries series;

        [ObservableProperty]
        private string seriesTitle = string.Empty;

        [ObservableProperty]
        private ObservableCollection<SCPItemList> items = new ObservableCollection<SCPItemList>();

        [ObservableProperty]
        private Visibility processBarVisibility = Visibility.Collapsed;

        private INavigationService _navigationService;
        private CancellationTokenSource? _cts;

        public TalesListPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            Messenger.Register<TalesListPageViewModel, ParameterMessage, string>(
                this, nameof(TalesListPageViewModel).Replace("ViewModel", ""), (r, m) =>
            {
                if (m.Value is SCPSeries series)
                {
                    Items.Clear();
                    Series = series;
                    SeriesTitle = series.SeriesName;
                    LoadItems(series.Href);
                }
            });
        }

        private async void LoadItems(string href)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                ProcessBarVisibility = Visibility.Visible;
                var data = await SCPService.GetItemList(href);
                if (token.IsCancellationRequested) return;

                foreach (var group in data)
                {
                    foreach (var item in group.Value)
                        Items.Add(item);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                Console.WriteLine($"[TalesListPage] EXCEPTION: {e}");
            }
            finally
            {
                ProcessBarVisibility = Visibility.Collapsed;
            }
        }

        [RelayCommand]
        public void GoToContent(object parameter)
        {
            if (parameter is SCPItemList item)
            {
                _navigationService.NavigateTo(nameof(ItemContentPage), item);
            }
        }

        [RelayCommand]
        public void GoBackPage()
        {
            _cts?.Cancel();
            _navigationService.GoBack();
        }
    }
}
