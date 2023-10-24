using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using SCPView_WinUI.Pages;
using SCPView_WinUI.Services;
using SCPView_WinUI.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SCPView_WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public Frame RootFrame { get; } = new Frame();
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();

            ConfigureServices();

            RootFrame.NavigationFailed += OnNavigationFailed;
            RootFrame.Navigate(typeof(MainPage), args.Arguments);

            m_window.Content = RootFrame;
            m_window.Activate();
        }

        private Window m_window;

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private static IServiceProvider ConfigureServices()
        {
            var nav =  ConfigNavigationService();

            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                .AddSingleton<INavigationService>(nav)
                .AddSingleton<MainPageViewModel>()
                .AddSingleton<ItemPageViewModel>()
                .AddSingleton<ItemContentPageViewModel>()
                .AddSingleton<TalesListPageViewModel>()
                .BuildServiceProvider()
                );
            var service = new ServiceCollection();
            
            return service.BuildServiceProvider();
        }

        private static NavigationService ConfigNavigationService()
        {
            var navigate = new NavigationService();
            navigate.Configure(nameof(MainPage),typeof(MainPage));
            navigate.Configure(nameof(ItemPage), typeof(ItemPage));
            navigate.Configure(nameof(ItemContentPage),typeof(ItemContentPage));
            navigate.Configure(nameof(TalesListPage), typeof(TalesListPage));
            return navigate;
        }
    }
}
