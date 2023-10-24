using SCPView_WinUI.Data;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string url = "/scp-cn-003";
            var itemList = SCPService.GetItemContent(url).Result;
            Console.WriteLine(itemList.SafeLevel + "\n" + itemList.Contents);
        }
    }
}
