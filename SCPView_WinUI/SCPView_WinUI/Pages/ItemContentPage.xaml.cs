using Microsoft.UI.Xaml.Controls;
using SCPView_WinUI.Data.Model;
using SCPView_WinUI.ViewModels;

namespace SCPView_WinUI.Pages
{
    public sealed partial class ItemContentPage : Page
    {
        public ItemContentPage()
        {
            this.InitializeComponent();
        }

        private void HubLinksListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is SCPItemList item)
            {
                var vm = (ItemContentPageViewModel)DataContext;
                vm.GoToHubContentCommand.Execute(item);
            }
        }
    }
}
