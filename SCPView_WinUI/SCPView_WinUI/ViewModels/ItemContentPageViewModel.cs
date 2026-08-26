using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
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
    public partial class ItemContentPageViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private SCPItem scpItem;

        [ObservableProperty]
        private ObservableGroupedCollection<string, CollapsibleContent> collapsibleContentCollection = new ObservableGroupedCollection<string, CollapsibleContent>();

        [ObservableProperty]
        private Visibility processBarVisibility = Visibility.Collapsed;

        private INavigationService _navigationService;
        private CancellationTokenSource? _cts;

        public ItemContentPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            Messenger.Register<ItemContentPageViewModel, ParameterMessage, string>(this, nameof(ItemContentPageViewModel).Replace("ViewModel", ""), (r, m) =>
            {
                if (m.Value is SCPItemList item)
                {
                    scpItem = new SCPItem();
                    ScpItem.Name = item.HrefName;
                    CollapsibleContentCollection.Clear();
                    GetContent(item.Href);
                }
            });
        }

        private async void GetContent(string contentUrl)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                ProcessBarVisibility = Visibility.Visible;
                var contentData = await SCPService.GetItemContent(contentUrl);
                if (token.IsCancellationRequested) return;

                if (contentData != null)
                {
                    var strName = ScpItem.Name;
                    ScpItem = contentData;
                    ScpItem.Name = strName;
                }

                if (ScpItem.CollapsibleContents != null && !token.IsCancellationRequested)
                {
                    foreach (var item in ScpItem.CollapsibleContents)
                    {
                        if (token.IsCancellationRequested) return;
                        CollapsibleContentCollection.AddItem(item.Name, item);
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"[ItemContentPage] EXCEPTION: {e}");
                Console.WriteLine($"[ItemContentPage] EXCEPTION: {e}");
            }
            finally
            {
                ProcessBarVisibility = Visibility.Collapsed;
            }
        }

        [RelayCommand]
        private void GoToHubContent(SCPItemList item)
        {
            _navigationService.NavigateTo(nameof(ItemContentPage), item);
        }

        [RelayCommand]
        public void GoBackPage()
        {
            _cts?.Cancel();
            _navigationService.GoBack();
        }

    }
}
