using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SCPView_WinUI.Data;
using SCPView_WinUI.Data.Model;
using SCPView_WinUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPView_WinUI.ViewModels
{
    public partial class ItemContentPageViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private SCPItem scpItem;

        [ObservableProperty]
        private ObservableGroupedCollection<string,CollapsibleContent> collapsibleContentCollection = new ObservableGroupedCollection<string, CollapsibleContent>();

        private INavigationService _navigationService;

        public ItemContentPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            Messenger.Register<ItemContentPageViewModel, ParameterMessage, string>(this, nameof(ItemContentPageViewModel).Replace("ViewModel", ""), (r, m) =>
            {
                if(m.Value is SCPItemList item)
                {
                    scpItem = new SCPItem();
                    ScpItem.Name = item.HrefName;
                    GetContent(item.Href);
                }
            });
        }

        private async void GetContent(string contentUrl)
        {
            try
            {
                var contentData = await SCPService.GetItemContent(contentUrl);
                if (contentData != null)
                {
                    ScpItem = contentData;
                }

                if(ScpItem.CollapsibleContents != null)
                {
                    foreach (var item in ScpItem.CollapsibleContents)
                    {
                        CollapsibleContentCollection.AddItem(item.Name,item);
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

    }
}
