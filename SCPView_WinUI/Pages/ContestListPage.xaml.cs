using Microsoft.UI.Xaml.Controls;
using SCPView_WinUI.Data.Model;
using SCPView_WinUI.ViewModels;

namespace SCPView_WinUI.Pages
{
    public sealed partial class ContestListPage : Page
    {
        public ContestListPage()
        {
            this.InitializeComponent();
        }

        private void ContestListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is SCPContestItem item)
            {
                var vm = (ContestListPageViewModel)DataContext;
                vm.GoToContentCommand.Execute(item);
            }
        }
    }
}
