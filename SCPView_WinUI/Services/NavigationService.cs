using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SCPView_WinUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPView_WinUI.Services
{
    public class NavigationService : INavigationService
    {
        public const string RootPageKey = "-- ROOT --";

        public const string UnknowPageKey = "-- UNKNOWN";

        private readonly Dictionary<string,Type> _pagesByKey = new Dictionary<string,Type>();

        private Frame _currentFrame;

        public Frame CurrentFrame
        {
            get => _currentFrame ?? (Application.Current as App).RootFrame;
            set => _currentFrame = value;
        }

        public NavigationService()
        {
            CurrentFrame.Navigated += OnNavigated;
        }

        public bool CanGoBack => CurrentFrame.CanGoBack;

        public bool CanGoForward => CurrentFrame.CanGoForward;

        public void GoForward()
        {
            if(CurrentFrame.CanGoForward)
            {
                CurrentFrame.GoForward();
            }
        }

        public string CurrentPageKey
        {
            get
            {
                lock(_pagesByKey)
                {
                    if(CurrentFrame.BackStackDepth == 0)
                    {
                        return RootPageKey;
                    }

                    if(CurrentFrame.Content == null)
                    {
                        return UnknowPageKey;
                    }

                    var currentType = CurrentFrame.Content.GetType();

                    if(_pagesByKey.All(p=>p.Value != currentType))
                    {
                        return UnknowPageKey;
                    }

                    var item = _pagesByKey.FirstOrDefault(i => i.Value == currentType);

                    return item.Key;
                }
            }
        }

        public string CurrentPage { get; }

        public void GoBack()
        {
            if(CurrentFrame.CanGoBack)
            {
                CurrentFrame.GoBack();
            }
        }

        public void NavigateTo(string pageKey)
        {
            NavigateTo(pageKey, null);
        }

        public virtual void NavigateTo(string pageKey, object parameter)
        {
            lock (_pagesByKey)
            {
                if(!_pagesByKey.ContainsKey(pageKey))
                {
                    throw new ArgumentException(
                        string.Format(
                            "No such page: {0}. Did you forget to call NavigationService.Configure?",
                            pageKey),
                        "pageKey");
                }
                CurrentFrame.Navigate(_pagesByKey[pageKey],parameter);
            }
        }

        public void Configure(string key, Type pageType)
        {
            lock (_pagesByKey)
            {
                if (_pagesByKey.ContainsKey(key))
                {
                    throw new ArgumentException("This key is already used: " + key);
                }

                if (_pagesByKey.Any(p => p.Value == pageType))
                {
                    throw new ArgumentException(
                        "This type is already configured with key " + _pagesByKey.First(p => p.Value == pageType).Key);
                }

                _pagesByKey.Add(
                    key,
                    pageType);
            }
        }

        public string GetKeyForPage(Type page)
        {
            lock (_pagesByKey)
            {
                if (_pagesByKey.ContainsValue(page))
                {
                    return _pagesByKey.FirstOrDefault(p => p.Value == page).Key;
                }
                else
                {
                    throw new ArgumentException($"The page '{page.Name}' is unknown by the NavigationService");
                }
            }
        }

        
        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            if(e.Parameter is not null)
            {
                WeakReferenceMessenger.Default.Send(new ParameterMessage(e.Parameter),e.SourcePageType.Name);
            }
        }
    }
}
