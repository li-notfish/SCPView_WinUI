using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    public partial class ContestListPageViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private ObservableCollection<SCPContestItem> contestItems = new ObservableCollection<SCPContestItem>();

        [ObservableProperty]
        private string contestTitle = string.Empty;

        [ObservableProperty]
        private string contestDescription = string.Empty;

        [ObservableProperty]
        private Visibility processBarVisibility = Visibility.Collapsed;

        private INavigationService _navigationService;
        private CancellationTokenSource? _cts;

        public ContestListPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            Messenger.Register<ContestListPageViewModel, ParameterMessage, string>(
                this, nameof(ContestListPageViewModel).Replace("ViewModel", ""), (r, m) =>
            {
                if (m.Value is string contestUrl)
                {
                    ContestItems.Clear();
                    ContestTitle = "征文竞赛";
                    ContestDescription = string.Empty;
                    LoadContestList(contestUrl);
                }
            });
        }

        private async void LoadContestList(string contestUrl)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                ProcessBarVisibility = Visibility.Visible;
                var data = await SCPService.GetContestList(contestUrl);
                if (token.IsCancellationRequested) return;

                if (!string.IsNullOrEmpty(data.Title))
                    ContestTitle = data.Title;

                if (!string.IsNullOrEmpty(data.Description))
                    ContestDescription = data.Description;

                foreach (var item in data.Items)
                    ContestItems.Add(item);
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                Console.WriteLine($"[ContestListPage] EXCEPTION: {e}");
            }
            finally
            {
                ProcessBarVisibility = Visibility.Collapsed;
            }
        }

        [RelayCommand]
        private void GoToContent(object parameter)
        {
            if (parameter is SCPContestItem item)
            {
                var scpItemList = new SCPItemList
                {
                    Href = item.Href,
                    HrefName = item.Title,
                    Name = item.Title
                };
                _navigationService.NavigateTo(nameof(ItemContentPage), scpItemList);
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
