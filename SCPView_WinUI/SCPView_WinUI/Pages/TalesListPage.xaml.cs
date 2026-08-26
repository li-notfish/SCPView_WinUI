using Microsoft.UI.Xaml.Controls;
using SCPView_WinUI.Data.Model;
using SCPView_WinUI.ViewModels;

namespace SCPView_WinUI.Pages
{
    public sealed partial class TalesListPage : Page
    {
        public TalesListPage()
        {
            this.InitializeComponent();
        }

        private void TalesListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is SCPItemList item)
            {
                var vm = (TalesListPageViewModel)DataContext;
                vm.GoToContentCommand.Execute(item);
            }
        }
    }
}
