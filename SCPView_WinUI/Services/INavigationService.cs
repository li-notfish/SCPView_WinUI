using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPView_WinUI.Services
{
    public interface INavigationService
    {
        string CurrentPage { get; }
        void NavigateTo(string pageKey);
        void NavigateTo(string pageKey, object parameter);
        void GoBack();
    }
}
