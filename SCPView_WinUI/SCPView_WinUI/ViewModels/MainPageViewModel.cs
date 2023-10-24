using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using SCPView_WinUI.Data;
using SCPView_WinUI.Data.Model;
using SCPView_WinUI.Pages;
using SCPView_WinUI.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPView_WinUI.ViewModels
{
    public partial class MainPageViewModel : ObservableRecipient
    {
        private INavigationService _navigationService;

        [ObservableProperty]
        private Visibility processBarVisibility = Visibility.Visible;

        [ObservableProperty]
        private bool processBarError = false;

        [ObservableProperty]
        private ObservableCollection<SCPMenuItem> menuItems = new ObservableCollection<SCPMenuItem>();

        [ObservableProperty]
        private SCPBanner banner = new SCPBanner();

        [ObservableProperty]
        private BitmapImage bannerImage = new BitmapImage();

        public MainPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            LoadData();
        }


        private async void LoadData()
        {
            try
            {
                ProcessBarVisibility = Visibility.Visible;
                var data = await SCPService.GetSeriesList();
                var bannerData = await SCPService.GetBanner();
                if (data != null)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        MenuItems.Add(data[i]);
                    }
                }
                if(bannerData != null)
                {
                    BannerImage.UriSource = new Uri(bannerData.BannerImagePath);
                }
                ProcessBarVisibility = Visibility.Collapsed;
            }
            catch (Exception e)
            {
                ProcessBarError = true;
                Console.WriteLine(e.Message);
            }
            
            
        }

        [RelayCommand]
        public void ReferData()
        {
            Task.Run(async () =>
            {
                ProcessBarVisibility = Visibility.Visible;
                ProcessBarError = false;
                LoadData();
            });
        }

        [RelayCommand]
        public void ToItemListPage(object parameter)
        {
            try
            {
                if(((SCPSeries)parameter).Href.Contains("tales-edition"))
                {
                    _navigationService.NavigateTo(nameof(TalesListPage),parameter);
                }
                else
                {
                    _navigationService.NavigateTo(nameof(ItemPage), parameter);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " \r\n" + e.TargetSite);
            }
            
        }
    }
}
